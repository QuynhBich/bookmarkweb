import mitt from 'mitt'
import type { Bookmark } from '../types/bookmark'

type ApplicationEvents = {
  // vevent name: payload type
  'dialog:signIn': boolean
  'dialog:signUp': boolean
  'bookmarks:update': Bookmark
}

const emitter = mitt<ApplicationEvents>()

export const useEventBus = emitter.emit
export const useListenBus = emitter.on
export const removeEventBus = emitter.off
