<template>
  <div v-if="message.isMy" class="flex items-end justify-end">
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
  </div>
  <div v-else class="flex items-end">
    <div
      class="flex flex-col space-y-2 text-xs max-w-xs mx-2 order-2 items-start"
    >
      <div>
        <span
          class="px-4 py-2 rounded-lg inline-block rounded-bl-none bg-gray-300 text-gray-600 whitespace-pre-wrap"
          >{{ message.content }}</span
        >
      </div>
    </div>
    <Icon
      name="fxemoji:anguish"
      class="font-black text-4xl font-mono mr-2 inline-block w-6 h-6"
    />
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
