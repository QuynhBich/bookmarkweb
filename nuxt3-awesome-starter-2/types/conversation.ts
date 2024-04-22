export interface InputMessage {
  id: string
  content: string
  isMy: boolean
  isNote: boolean
  note: string
  isNoted: boolean
  updatedAt: Date | null
}

export interface InputBookmark {
  bookmarkId: string
  bookmarkUrl: string
}

export interface InputUser {
  userName: string
}

export interface Conversation {
  conversationId: string
  bookmark: InputBookmark
  message: InputMessage
  reply: InputMessage | null
  user: InputUser
  name: string
  isExpand: boolean
}
