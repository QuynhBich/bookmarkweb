import { storeToRefs } from 'pinia'
import type { IAuthToken, LoginResult } from '../types/auth.ts'
export default defineNuxtRouteMiddleware(async (to) => {
  const auth = useAuthStore()
  const config = useRuntimeConfig()

  const cookieToken = useCookie<IAuthToken | null>('authToken')
  if (cookieToken.value) {
    const authToken = cookieToken.value
    if (authToken) {
      auth.setToken(authToken)
    }
  }

  const { authenticated, isTokenExpired, canRefreshToken } =
    storeToRefs(useAuthStore())
  if (!authenticated.value && to.path !== '/') {
    return navigateTo('/')
  }

  if (authenticated.value && isTokenExpired.value && canRefreshToken.value) {
    try {
      const response: LoginResult = await $fetch('/identity/refresh', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          accept: 'application/json',
        },
        body: {
          token: auth.authToken?.token,
          refreshToken: auth.authToken?.refreshToken,
        },
        baseURL: config.public.apiBaseUrl,
      })

      if (response.data) {
        auth.setToken(response.data)
      } else {
        auth.logUserOut()
        useEventBus('dialog:signIn', true)
      }
    } catch (e) {
      auth.logUserOut()
      useEventBus('dialog:signIn', true)
    }
  }
})
