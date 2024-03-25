export interface ILoginInput {
  email: string
  password: string
}

export interface ILoginResponse {
  token: string
  refreshToken: string
  refreshTokenExpiryTime: Date
}

export interface IRegisterInput {
  email: string
  password: string
}

export interface IForgotPassword {
  email: string
}
export interface ILoginGoogleInput {
  oAuthCode: string
}

export interface IRefreshTokenInput {
  token: string
  refreshToken: string
}

export interface IResetPassword {
  email: string
  password: string
  code: string
}

export interface IAuthUser {
  userId: string
  username: string
  name: string
  surname: string
  fullName: string
  image_url: string
  isAdmin: string
  isPremium: string
  exp: number
}

export interface IAuthToken {
  token: string
  refreshToken: string
  refreshTokenExpiryTime: string
}
export interface LoginResult {
  data: IAuthToken
}
