<template>
  <div v-if="message" class="flex items-end relative">
    <div
      class="absolute top-[-15px] w-6 h-6 right-[-15px] cursor-pointer rounded-full bg-red-700"
      @click="emits('delete', message.id)"
    >
      <Icon name="typcn:delete-outline" class="w-full h-full text-black"></Icon>
    </div>
    <div
      class="flex flex-col space-y-2 text-xs max-w-xs mx-2 order-2 items-start"
    >
      <div>
        <span
          ref="highlightText"
          class="px-4 py-2 rounded-lg inline-block rounded-bl-none bg-orange-400 text-black whitespace-pre-wrap"
          >{{ message.content }}</span
        >
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
const emits = defineEmits(['delete'])
</script>
