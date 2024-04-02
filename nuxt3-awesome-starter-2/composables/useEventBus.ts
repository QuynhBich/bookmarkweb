import mitt from 'mitt'
import type { Bookmark } from '../types/bookmark'
import type { Folder } from '../types/folder'

type ApplicationEvents = {
  // vevent name: payload type
  'dialog:signIn': boolean
  'dialog:signUp': boolean
  'bookmarks:update': Bookmark
  'folder:update': Folder
}

const emitter = mitt<ApplicationEvents>()

export const useEventBus = emitter.emit
export const useListenBus = emitter.on
export const removeEventBus = emitter.off
