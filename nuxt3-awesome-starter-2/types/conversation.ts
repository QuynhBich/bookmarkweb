export interface InputMessage {
  id: string
  content: string
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
  reply: InputMessage
  user: InputUser
  name: string
  isExpand: boolean
}
