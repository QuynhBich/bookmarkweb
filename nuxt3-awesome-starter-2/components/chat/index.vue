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
          class="inline-flex items-center justify-center rounded-lg border border-gray-500 h-10 w-10 transition duration-500 ease-in-out text-gray-500 hover:bg-gray-300 focus:outline-none"
          @click="download()"
        >
          <Icon name="material-symbols:download-2" class="w-5 h-5" />
        </button>
        <button
          class="inline-flex items-center justify-center rounded-lg border border-gray-500 h-10 w-10 transition duration-500 ease-in-out text-gray-500 hover:bg-gray-300 focus:outline-none"
        >
          <Icon name="material-symbols:search" class="w-5 h-5" />
        </button>
        <button
          class="inline-flex items-center justify-center rounded-lg border border-gray-500 h-10 w-10 transition duration-500 ease-in-out text-gray-500 hover:bg-gray-300 focus:outline-none"
          @click="deleteConversation"
        >
          <Icon name="material-symbols:delete-outline" class="w-5 h-5" />
        </button>
      </div>
    </div>
    <div
      id="messages"
      class="flex flex-col space-y-4 p-3 overflow-y-auto scrollbar-thumb-blue scrollbar-thumb-rounded scrollbar-track-blue-lighter scrollbar-w-2 scrolling-touch h-full justify-start"
    >
      <div class="chat-message gap-4 flex flex-col">
        <ChatSummary
          :list-question="listQuestion"
          @send-question="sendQuestion"
        ></ChatSummary>
        <div v-for="(mess, i) in messages" :key="mess.id">
          <ChatMessage v-if="i !== 0 && mess" :message="mess"></ChatMessage>
        </div>
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
          @click="handleSendMessage()"
        >
          <button
            type="button"
            class="inline-flex items-center justify-center rounded-md rounded-l-none px-4 py-3 transition duration-500 ease-in-out text-white bg-blue-500 hover:bg-blue-400 focus:outline-none h-full"
          >
            <Icon
              name="material-symbols-light:send-rounded"
              class="w-10 h-10 pl-1"
            />
          </button>
        </div>
      </div>
    </div>
  </div>
  <CommonToats ref="toatsCommon" type="success"></CommonToats>
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
import type { CommonToats } from '#build/components'
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
const listQuestion = ref<string[]>()
const getMessages = async (id: string) => {
  const { data } = await useGetApi<InputMessage[]>(
    `/chats/get-messages/${id}`,
    { server: false },
  )
  if (data.value) {
    messages.value = data.value
    if (data.value[0]) {
      listQuestion.value = data.value[0].content.split('[*]')
    }
  } else {
    listQuestion.value = []
    messages.value = []
  }
}
const currentBookmark = ref<Bookmark>()
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
const inputMessage = ref<InputMessage | null>(null)
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
    listQuestion.value = []
    currentBookmark.value = props.bookmark
    if (!props.bookmark.conversationId) {
      await getCurrentBookmark()
    }
    if (currentBookmark.value.conversationId) {
      conversationId.value = currentBookmark.value.conversationId
      await getMessages(conversationId.value)
    } else {
      messages.value = []
      listQuestion.value = []
      await handleSendMessage(true)
    }
  },
)

const getCurrentBookmark = async () => {
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

// download conversation
const download = () => {
  const messageText = ref('')
  if (messages.value.length > 0) {
    messages.value.forEach((e) => {
      if (e.isMy) messageText.value += `You: ${e.content}\n\n`
      else messageText.value += `Quinn.AI: ${e.content}\n\n`
    })
  }
  const a = document.createElement('a')
  a.href = `data:text/plain,${messageText.value}`
  a.download = 'file.txt'
  document.body.appendChild(a)
  a.click()
}

// Delete conversation
const toatsCommon = ref<InstanceType<typeof CommonToats> | null>(null)
const deleteConversation = async () => {
  const { data, status } = await usePostApi(
    `chats/delete-conversation/${conversationId.value}`,
    { server: false },
  )
  if (status.value === 'success') {
    messages.value = []
    listQuestion.value = []
    toatsCommon.value?.setClose()
    setTimeout(async () => {
      await getCurrentBookmark()
    }, 100)
  }
}
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
  listQuestion.value = messages.value[0].content.split('[*]')
}
const handleSendMessage = async (summary: boolean = false) => {
  if (message.value === '' && !summary) return
  getInput()
  if (summary) inputMessage.value = null
  if (inputMessage.value?.id && !summary)
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
  if (!conversationId.value) {
    emit('updateListBookmark')
  }
}
onNuxtReady(async () => {
  console.log('onNuxtReady in chat')
  hub.value = configHub()
  if (hub.value) {
    try {
      await hub.value.start()
    } catch (e) {
      console.log(e)
    }
  }

  listQuestion.value = []
  currentBookmark.value = props.bookmark
  if (currentBookmark.value.conversationId) {
    conversationId.value = currentBookmark.value.conversationId
    await getMessages(conversationId.value)
  } else {
    messages.value = []
    listQuestion.value = []
    console.log('aaa')
    await handleSendMessage(true)
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

// send suggested question
const sendQuestion = (question: string) => {
  message.value = question
  handleSendMessage()
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
