export const useRefreshToken = async () => {
  const config = useRuntimeConfig()
  const auth = useAuthStore()
  if (auth.canRefreshToken) {
    await $fetch('/api/identity/refresh', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        accept: 'application/json',
      },
      body: {
        token: auth.authToken?.token,
        refreshToken: auth.authToken?.refreshToken,
      },
      baseURL: config.public.content.api.baseURL,
    })
      .then((token: any) => auth.setToken(token))
      .catch(() => {
        auth.logUserOut()
        // TODO
        useEventBus('dialog:signIn', true)
      })
  } else auth.logUserOut()
}
