const getFaviconUrl = (baseURL: string) => {
  const parsedURL = new URL(baseURL)
  return `${parsedURL.protocol}//${parsedURL.host}/favicon.ico`
}
const getDomain = (baseURL: string) => {
  const parsedURL = new URL(baseURL)
  return `${parsedURL.hostname}`
}

export { getFaviconUrl, getDomain }
