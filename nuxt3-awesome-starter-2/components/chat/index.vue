<template>
  <div class="flex-1 p:2 sm:p-6 justify-between flex flex-col h-full">
    <div
      class="flex sm:items-center justify-between py-1 border-b-2 border-gray-200"
    >
      <div class="relative flex items-center space-x-4">
        <div class="relative">
          <span class="absolute text-green-500 right-0 bottom-0">
            <svg width="20" height="20">
              <circle cx="8" cy="8" r="8" fill="currentColor"></circle>
            </svg>
          </span>
          <Icon
            name="fxemoji:anguish"
            class="font-black text-4xl font-mono mr-2 inline-block w-10 sm:w-16 h-10 sm:h-16"
          />
        </div>
        <div class="flex flex-col leading-tight">
          <div class="text-2xl mt-1 flex items-center">
            <span class="text-gray-700 mr-3">Quinn.AI</span>
          </div>
          <span class="text-lg text-gray-600">{{ bookmark.domain }}</span>
        </div>
      </div>
      <div class="flex items-center space-x-2">
        <button
          type="button"
          class="inline-flex items-center justify-center rounded-lg border h-10 w-10 transition duration-500 ease-in-out text-gray-500 hover:bg-gray-300 focus:outline-none"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
            class="h-6 w-6"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"
            ></path>
          </svg>
        </button>
        <button
          type="button"
          class="inline-flex items-center justify-center rounded-lg border h-10 w-10 transition duration-500 ease-in-out text-gray-500 hover:bg-gray-300 focus:outline-none"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
            class="h-6 w-6"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z"
            ></path>
          </svg>
        </button>
        <button
          type="button"
          class="inline-flex items-center justify-center rounded-lg border h-10 w-10 transition duration-500 ease-in-out text-gray-500 hover:bg-gray-300 focus:outline-none"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
            class="h-6 w-6"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9"
            ></path>
          </svg>
        </button>
      </div>
    </div>
    <div
      id="messages"
      class="flex flex-col space-y-4 p-3 overflow-y-auto scrollbar-thumb-blue scrollbar-thumb-rounded scrollbar-track-blue-lighter scrollbar-w-2 scrolling-touch h-full justify-start"
    >
      <div class="chat-message">
        <ChatMessage
          v-for="mess in messages"
          :key="mess.id"
          :message="mess"
        ></ChatMessage>
      </div>
    </div>
    <div class="border-t-2 border-gray-200 px-4 pt-4 mb-10">
      <div class="relative flex">
        <input
          v-model="message"
          type="text"
          placeholder="Write your message!"
          class="w-full focus:outline-none focus:placeholder-gray-400 text-gray-600 placeholder-gray-600 pl-4 bg-gray-200 rounded-md py-3"
        />
        <div
          class="absolute right-0 items-center inset-y-0 hidden sm:flex"
          @click="handleSendMessage"
        >
          <button
            type="button"
            class="inline-flex items-center justify-center rounded-lg px-4 py-3 transition duration-500 ease-in-out text-white bg-blue-500 hover:bg-blue-400 focus:outline-none"
          >
            <span class="font-bold">Send</span>
            <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 20 20"
              fill="currentColor"
              class="h-6 w-6 ml-2 transform rotate-90"
            >
              <path
                d="M10.894 2.553a1 1 0 00-1.788 0l-7 14a1 1 0 001.169 1.409l5-1.429A1 1 0 009 15.571V11a1 1 0 112 0v4.571a1 1 0 00.725.962l5 1.428a1 1 0 001.17-1.408l-7-14z"
              ></path>
            </svg>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
// chat
import { HubConnection, HubConnectionBuilder, LogLevel } from '@aspnet/signalr'
import {
  type InputMessage,
  type Conversation,
  type InputBookmark,
  type InputUser,
} from '../../types/conversation'
import type { Bookmark } from '../../types/bookmark'
const props = defineProps({
  drawerVisible: {
    type: Boolean,
    default: false,
  },
  bookmark: {
    type: Object as PropType<Bookmark>,
    default: null,
  },
})
const getMessages = async (id: string) => {
  const { data } = await useGetApi<InputMessage[]>(
    `/chats/get-messages/${id}`,
    { server: false },
  )
  if (data.value) {
    messages.value = data.value
  }
}
const currentBookmark = ref<Bookmark>()
onMounted(async () => {
  currentBookmark.value = props.bookmark
  if (currentBookmark.value.conversationId) {
    conversationId.value = currentBookmark.value.conversationId
    await getMessages(conversationId.value)
  } else {
    messages.value = []
  }
})
const uuidv4 = (): string => {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(
    /[xy]/g,
    (c: string) => {
      const r = (crypto.getRandomValues(new Uint8Array(1))[0] & 15) | 0
      const v = c === 'x' ? r : (r & 0x3) | 0x8
      return v.toString(16)
    },
  )
}
const conversationId = ref('')
const inputBookmark = ref<InputBookmark>()
const userInput = ref<InputUser>()
const inputMessage = ref<InputMessage>()
const getInput = () => {
  conversationId.value = !props.bookmark.conversationId
    ? uuidv4()
    : props.bookmark.conversationId
  inputBookmark.value = {
    bookmarkId: props.bookmark?.id,
    bookmarkUrl: props.bookmark?.url,
  }
  inputMessage.value = {
    id: uuidv4(),
    content: message.value,
    isMy: true,
  }
  if (authStore.user?.Username)
    userInput.value = {
      userName: authStore.user?.Username,
    }
}
const emit = defineEmits(['close', 'updateListBookmark'])
const close = () => {
  emit('close')
}
watch(
  () => props.drawerVisible,
  (oldValue, newValue) => {},
)
watch(
  () => props.bookmark,
  async (oldValue, newValue) => {
    currentBookmark.value = props.bookmark
    if (!props.bookmark.conversationId) {
      const { data } = await useGetApi<Bookmark>(
        `/bookmarks/get-bookmark/${props.bookmark.id}`,
        {
          server: false,
        },
      )
      if (data.value) {
        currentBookmark.value = data.value
      }
    }
    if (currentBookmark.value.conversationId) {
      conversationId.value = currentBookmark.value.conversationId
      await getMessages(conversationId.value)
    } else {
      messages.value = []
    }
  },
)

// chat

const authStore = useAuthStore()
const config = useRuntimeConfig()

const maxAttempt = 5
const currentAttempt = ref<number>(0)
const hub = ref<HubConnection | null>(null)

const cursor = ref<string | null>(null)
const folderId = ref<string | null>(null)
const HUB = {
  ERROR: 'Error',
  CONNECTED: 'Connected',
  DISCONNECTED: 'Disconnected',
  ADDED_TO_GROUP: 'AddedToGroup',
  REMOVED_FROM_GROUP: 'RemovedFromGroup',
  GPT_REPLY: 'GptReply',
  GPT_WRITE_REPLY: 'GptWriteReply',
  SAVE_MESSAGE: 'SaveMessage',
  UPDATE_MESSAGE: 'UpdateMessage',
}
const message = ref('')
const isDisableSendBtn = ref(false)
const messages = ref<InputMessage[]>([])
const addMessageToConversation = (mess: InputMessage) => {
  messages.value.push(mess)
}
const handleSendMessage = async () => {
  // if (isDisableSendBtn) return
  getInput()
  if (inputMessage.value?.id)
    addMessageToConversation({
      id: inputMessage.value?.id,
      content: inputMessage.value.content,
      isMy: true,
    })
  await hub.value?.invoke('OnSendMessage', {
    conversationId: conversationId.value,
    bookmark: inputBookmark.value,
    message: inputMessage.value,
    reply: {
      id: uuidv4(),
      content: '',
      isMy: false,
    },
    user: userInput.value,
    name: '',
    isExpand: false,
  } as Conversation)
  message.value = ''
}
const onSaveMessage = async (mess: Conversation) => {
  const data = await usePostApi(`/chats/save-message`, JSON.stringify(mess), {
    server: false,
  })
  if (!props.bookmark.conversationId) {
    emit('updateListBookmark')
  }
}
onNuxtReady(async () => {
  // const initConversation = await fetchConversations()
  // conversations.value = initConversation.conversations

  hub.value = configHub()
  if (hub.value) {
    try {
      await hub.value.start()
    } catch (e) {
      console.log(e)
    }
  }
})
const configHub = (): HubConnection | null => {
  const { token } = authStore
  if (!token) return null
  const hub = new HubConnectionBuilder()
    .withUrl(`${config.public.apiBaseUrl}/hub/notification`, {
      accessTokenFactory: function () {
        return token
      },
    })
    .configureLogging(LogLevel.Debug)
    .build()

  hub.on(HUB.CONNECTED, (message: string) => {
    console.log('Hub connected')
  })

  hub.on(HUB.ERROR, async (message: string) => {
    console.log('Hub error')
    await hub.stop()
  })

  hub.on(HUB.GPT_REPLY, async (message: Conversation) => {
    if (message.conversationId === conversationId.value) {
      addMessageToConversation({
        id: message.reply?.id,
        content: message.reply?.content,
        isMy: false,
      } as InputMessage)
    }
    await onSaveMessage(message)
  })

  async function start() {
    try {
      await hub.start()
    } catch (err) {
      if (currentAttempt.value > maxAttempt) {
        return
      }
      currentAttempt.value++
      setTimeout(() => start(), 5000)
    }
  }

  hub.onclose(async () => {
    await start()
  })

  return hub
}
</script>

<style>
.scrollbar-w-2::-webkit-scrollbar {
  width: 0.25rem;
  height: 0.25rem;
}

.scrollbar-track-blue-lighter::-webkit-scrollbar-track {
  --bg-opacity: 1;
  background-color: #f7fafc;
  background-color: rgba(247, 250, 252, var(--bg-opacity));
}

.scrollbar-thumb-blue::-webkit-scrollbar-thumb {
  --bg-opacity: 1;
  background-color: #edf2f7;
  background-color: rgba(237, 242, 247, var(--bg-opacity));
}

.scrollbar-thumb-rounded::-webkit-scrollbar-thumb {
  border-radius: 0.25rem;
}
</style>
