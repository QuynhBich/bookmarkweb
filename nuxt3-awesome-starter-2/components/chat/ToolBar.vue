<template>
  <div class="flex flex-col justify-center items-center gap-1">
    <Icon v-if="isCopy" class="w-5 h-5 cursor-pointer" name="charm:tick"></Icon>
    <Icon
      v-else
      class="w-5 h-5 cursor-pointer"
      name="material-symbols:content-copy"
      @click="copyToClipboard(message.content)"
    ></Icon>
    <Icon
      class="w-5 h-5 cursor-pointer"
      name="material-symbols-light:colors"
      @click="fillColor"
    ></Icon>
    <Icon
      class="w-5 h-5 cursor-pointer"
      name="material-symbols:speaker-notes-outline"
      @click="openNotePad"
    ></Icon>
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
const emits = defineEmits(['fillColor', 'openNotePad'])

// Copy to clipboard
const isCopy = ref(false)
const copyToClipboard = (text: string) => {
  document.addEventListener('copy', (e: ClipboardEvent) => {
    e.clipboardData?.setData('text/plain', text)
    e.preventDefault()
  })
  document.execCommand('copy')
  isCopy.value = true
}

// fill color
const isNoted = ref(false)
const fillColor = () => {
  isNoted.value = !isNoted.value
  emits('fillColor')
}

// open note pad
const openNotePad = () => {
  emits('openNotePad', props.message)
  console.log(props.message)
}
</script>
