/* eslint-disable require-await */
import { defineStore } from 'pinia'
import type {
  IAuthToken,
  IAuthUser,
  ILoginInput,
  ILoginResponse,
  LoginResult,
} from '../types/auth.ts'
import { jwtDescrypt } from '../utils/scrypt.js'
export const useAuthStore = defineStore('auth', {
  state: () => ({
    loading: false,
    user: ref<IAuthUser | null>(null),
    authToken: ref<IAuthToken | null>(null),
  }),
  getters: {
    authenticated: (state) => !!state.authToken,
    isPremium: (state) => state.user && state.user.isPremium === 'true',
    canRefreshToken: (state) =>
      state.authToken &&
      state.authToken.token &&
      state.authToken.refreshToken &&
      state.authToken.refreshTokenExpiryTime &&
      Date.parse(state.authToken.refreshTokenExpiryTime) > Date.now(),
    token: (state) => state?.authToken?.token ?? '',
    isTokenExpired: (state) => {
      if (!state.user) {
        return false
      }
      const exp = state.user.exp
      return exp * 1000 < new Date().valueOf()
    },
  },
  actions: {
    async authenticateUser({ email, password }: ILoginInput) {
      const { $api } = useNuxtApp()
      const input: ILoginInput = {
        email,
        password,
      }
      const config = useRuntimeConfig()
      const response: LoginResult = await $fetch('/auth/login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          accept: 'application/json',
        },
        body: {
          username: email,
          password,
        },
        baseURL: config.public.apiBaseUrl,
      })
      if (response.data) {
        this.setToken(response.data)
      } else {
        this.logUserOut()
        useEventBus('dialog:signIn', true)
      }

      // await $api.auth
      //   .login({
      //     email,
      //     password,
      //   })
      //   .then((token: IAuthToken) => this.setToken(token))
    },
    logUserOut() {
      this.setToken(null)
    },
    setUser(newUser: IAuthUser | null) {
      if (!newUser) {
        this.user = null
        return
      }
      this.user = newUser
    },
    setToken(newToken: IAuthToken | null) {
      const cookieToken = useCookie<IAuthToken | null>('authToken')
      if (!newToken) {
        this.authToken = null
        cookieToken.value = null
        this.setUser(null)
        return
      }
      this.authToken = newToken
      const tokenUser = jwtDescrypt(newToken.token) as IAuthUser
      this.setUser(tokenUser)
      cookieToken.value = newToken
    },
  },
})
