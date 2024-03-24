import { useRefreshToken } from './useRefreshToken'

export const useGetApi = async <T>(url: string, options: any = {}) =>
  await useFetchApi<T>(url, {
    method: 'GET',
    ...options,
  })

export const usePostApi = async <T>(
  url: string,
  body: any,
  options: any = {},
) =>
  await useFetchApi<T>(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      accept: 'application/json',
    },
    body,
    ...options,
  })

export const usePutApi = async <T>(url: string, body: any, options: any = {}) =>
  await useFetchApi<T>(url, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      accept: 'application/json',
    },
    body,
    ...options,
  })

export const usePatchApi = async <T>(
  url: string,
  body: any,
  options: any = {},
) =>
  await useFetchApi<T>(url, {
    method: 'PATCH',
    headers: {
      'Content-Type': 'application/json',
      accept: 'application/json',
    },
    body,
    ...options,
  })

export const useDeleteApi = async <T>(url: string, options: any = {}) =>
  await useFetchApi<T>(url, {
    method: 'DELETE',
    ...options,
  })

/**
 * It takes a URL and options, and returns a response object
 * @param url - The URL to fetch.
 * @param [options] - The options object that will be passed to the fetch function.
 * @returns The return value of the useFetch hook.
 */
export const useFetchApi = async <T>(url: string, options: any = {}) => {
  const config = useRuntimeConfig()
  const auth = useAuthStore()
  return await useFetch<T>(url, {
    ...options,
    headers: {
      Authorization: auth.authenticated
        ? `Bearer ${auth.authToken?.token}`
        : '',
      ...options?.headers,
    },
    baseURL: config.public.apiBaseUrl,
    async onResponseError({ response }) {
      if (response.status === 401) {
        if (auth.authenticated) {
          await useRefreshToken()
          await useFetchApi(url, options)
        } else {
          auth.logUserOut()
          // TODO
          useEventBus('dialog:signIn', true)
        }
      }
    },
  })
}
