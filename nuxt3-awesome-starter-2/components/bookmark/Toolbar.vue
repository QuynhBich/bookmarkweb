<template>
  <ChatNote
    v-if="isOpenNotePad"
    :message="message"
    @close-note-pad="isOpenNotePad = false"
  ></ChatNote>
  <div>
    <div
      class="absolute right-0 top-0 z-50 h-screen w-0 overflow-hidden bg-white pl-0 transition-all dark:bg-neutral-900 dark:text-neutral-400"
      :style="{
        width: drawerVisible ? '40vw' : '0',
        paddingLeft: drawerVisible ? '10px' : '0',
      }"
    >
      <button
        type="button"
        class="absolute right-2.5 top-2.5 inline-flex h-8 w-8 items-center justify-center rounded-lg bg-transparent text-sm text-gray-400 hover:bg-gray-200 hover:text-gray-900 dark:hover:bg-gray-600 dark:hover:text-white"
        @click="close"
      >
        <Icon name="bytesize:close" class="w-4 h-4" />
        <span class="sr-only">Close menu</span>
      </button>
      <div class="mt-10 flex w-full justify-center py-1">
        <slot name="edit" />
      </div>
      <div class="w-full h-full">
        <Chat
          v-if="bookmark"
          :bookmark="bookmark"
          @update-list-bookmark="emit('updateBookmarks')"
          @open-note-pad="(mess: InputMessage) => openNotePad(mess)"
        ></Chat>
      </div>
    </div>
    <div
      class="absolute left-0 top-0 z-10 h-screen w-0 bg-black/30 transition-opacity"
      :style="{
        width: drawerVisible ? '100vw' : '0',
        opacity: drawerVisible ? '0.6' : '0',
      }"
      @keydown="close"
      @click="close"
    />
  </div>
</template>
<script lang="ts" setup>
import type { Bookmark } from '../../types/bookmark'
import type { InputMessage } from '../../types/conversation'

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
const emit = defineEmits(['close', 'updateBookmarks'])
const close = () => {
  emit('close')
}
watch(
  () => props.drawerVisible,
  (oldValue, newValue) => {},
)

// Note
const isOpenNotePad = ref(false)
const message = ref<InputMessage>()
const openNotePad = (mess: InputMessage) => {
  isOpenNotePad.value = true
  message.value = mess
}
</script>
