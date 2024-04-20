<template>
  <div
    class="w-80 h-72 flex flex-col gap-3 items-center justify-center bg-red-200 rounded-3xl rounded-br-none border border-red-300 mb-6 py-5 px-4 absolute left-1/3 z-50 top-1/4"
  >
    <button
      type="button"
      class="w-full inline-flex h-8 items-center justify-end rounded-lg bg-transparent text-sm text-white"
      @click="emit('closeNotePad')"
    >
      <Icon name="bytesize:close" class="w-6 h-6 rounded-full bg-red-400 p-1" />
    </button>
    <div class="w-full flex items-center justify-center">
      <textarea
        v-model="note"
        class="text-gray-800 leading-7 font-semibold bg-transparent overflow-hidden border border-red-300 p-1 w-full outline-none"
        rows="6"
      ></textarea>
    </div>
    <div class="px-2 w-full">
      <div class="flex items-center justify-between text-gray-800">
        <button
          class="w-14 h-8 bg-red text-white p-1 rounded-md"
          :class="[
            { 'bg-slate-500': isDisableBtn },
            { 'bg-sky-500': !isDisableBtn },
          ]"
          :disabled="isDisableBtn"
          @click="updateNotePad"
        >
          <span>Done</span>
        </button>
        <button
          class="w-8 h-8 rounded-full bg-gray-800 text-white flex items-center justify-center focus:outline-none focus:ring-2 focus:ring-offset-2 ring-offset-red-300 focus:ring-black"
          aria-label="edit note"
        >
          <Icon
            class="h-6 w-6"
            name="material-symbols-light:edit-outline"
            @click="isDisableBtn = false"
          ></Icon>
        </button>
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
  isShowTool: {
    type: Boolean,
    default: false,
  },
})
// const isDisableBtn = ref(true)
const note = ref(props.message.note)
const isDisableBtn = computed(() => {
  return note.value === ''
})
const emit = defineEmits(['closeNotePad'])
const isShowTool = ref(props.isShowTool)
const updateNotePad = async () => {
  const message = ref(props.message)
  message.value.note = note.value
  const { status } = await usePostApi(
    '/chats/update-notepad',
    JSON.stringify(message.value),
    { server: false },
  )
  if (status.value === 'success') {
    isShowTool.value = false
    emit('closeNotePad')
  }
}
</script>
