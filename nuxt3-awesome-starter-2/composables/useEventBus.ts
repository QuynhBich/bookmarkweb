import mitt from 'mitt'

type ApplicationEvents = {
  // vevent name: payload type
  'dialog:signIn': boolean
  'dialog:signUp': boolean
}

const emitter = mitt<ApplicationEvents>()

export const useEventBus = emitter.emit
export const useListenBus = emitter.on
export const removeEventBus = emitter.off
