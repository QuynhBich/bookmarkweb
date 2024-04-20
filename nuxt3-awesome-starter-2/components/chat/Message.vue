<template>
  <div class="relative">
    <div
      v-if="showNote && message.note"
      class="w-80 h-auto bg-lime-400 absolute top-[-35px] left-10 text-white opacity-80 rounded-lg p-1"
    >
      <span class="bg-lime-800 rounded-sm">{{ message.note }}</span>
      <Icon
        name="mdi:arrow-left-bottom-bold"
        class="absolute right-1 text-lime-800 w-6 h-6"
      ></Icon>
    </div>
    <div
      v-if="message.isMy"
      class="flex items-end justify-end"
      @mouseenter="showToolBar(true)"
      @mouseleave="showToolBar(false)"
    >
      <div
        class="flex flex-col space-y-2 text-xs max-w-xs mx-2 order-1 items-end"
      >
        <div>
          <span
            class="px-4 py-2 rounded-lg inline-block rounded-br-none bg-blue-600 text-white"
            >{{ message.content }}</span
          >
        </div>
      </div>
      <Icon
        name="fxemoji:koala"
        class="font-black text-4xl font-mono mr-2 inline-block w-6 h-6 order-2"
      />
      <div
        v-if="isShowToolBar"
        class="absolute z-50 h-20 w-6 order-3 left-14 top-0 bg-black bg-opacity-50 flex flex-col justify-center items-center rounded-sm p-1 gap-3"
      >
        <ChatToolBar
          :message="message"
          @fill-color="fillColor"
          @open-note-pad="openNotePad"
        ></ChatToolBar>
      </div>
    </div>
    <div v-else>
      <div
        v-if="message"
        class="flex items-end"
        @mouseenter="showToolBar(true)"
        @mouseleave="showToolBar(false)"
      >
        <div
          class="flex flex-col space-y-2 text-xs max-w-xs mx-2 order-2 items-start"
        >
          <div
            @mouseenter="showTextNote(true)"
            @mouseleave="showTextNote(false)"
          >
            <span
              ref="highlightText"
              class="px-4 py-2 rounded-lg inline-block rounded-bl-none bg-gray-300 text-gray-600 whitespace-pre-wrap"
              @mouseup="highlightSelectedText"
              ><span
                :class="[
                  { 'bg-yellow-300': isNoted || message.isNoted },
                  { 'bg-gray-300': !isNoted },
                ]"
                >{{ message.content }}</span
              ></span
            >
          </div>
        </div>
        <Icon
          name="fxemoji:anguish"
          class="font-black text-4xl font-mono mr-2 inline-block w-6 h-6"
        />
        <div
          v-if="isShowToolBar"
          class="absolute z-50 h-20 w-6 bg-transparent order-3 right-12 top-0 bg-white bg-opacity-10 flex flex-col justify-center items-center rounded-sm p-1"
        >
          <ChatToolBar
            :message="message"
            @fill-color="fillColor"
            @open-note-pad="openNotePad"
          ></ChatToolBar>
        </div>
      </div>
      <div v-else>
        <Icon name="svg-spinners:3-dots-scale" class="w-6 h-6" />
      </div>
    </div>
  </div>
</template>
<script lang="ts" setup>
import type { InputMessage } from '../../types/conversation'
const props = defineProps({
  message: {
    type: Object as PropType<InputMessage>,
    default: null,
  },
})
console.log(props.message)
const emits = defineEmits(['openNotePad', 'highlight'])
const isShowToolBar = ref(false)
const showNote = ref(false)
const showToolBar = (x: boolean) => {
  isShowToolBar.value = x
}
const showTextNote = (x: boolean) => {
  showNote.value = x
}

// fill color
const isNoted = ref(false)
const message = ref(props.message)
message.value.isNote = isNoted.value
const fillColor = async () => {
  isNoted.value = !isNoted.value
  message.value.isNoted = isNoted.value
  const { status } = await usePostApi(
    '/chats/highlight-message',
    JSON.stringify(message.value),
    { server: false },
  )
  if (status.value === 'success') {
    emits('highlight')
  }
}
// open note pad
const openNotePad = (mess: InputMessage) => {
  emits('openNotePad', mess)
}

// highlight text
const highlightSelectedText = () => {
  const selection = window.getSelection()
  if (selection && selection.toString().length > 0) {
    const range = selection.getRangeAt(0)
    const span = document.createElement('span')
    span.className = 'highlighted-text'
    range.surroundContents(span)
  }
}
</script>
<style>
.highlighted-text {
  background-color: yellow;
}

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
