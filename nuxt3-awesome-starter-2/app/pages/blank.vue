<template>
  <LayoutPageWrapper class="flex">
    <div v-for="mess in storageMessages" :key="mess.id">
      <ChatMessage :message="mess"></ChatMessage>
    </div>
  </LayoutPageWrapper>
</template>
<script lang="ts" setup>
import type { InputMessage } from '../../types/conversation'

definePageMeta({ layout: 'page' })
useHead({ title: 'Storage' })
const storageMessages = ref<InputMessage[]>([])
onNuxtReady(async () => {
  const { data } = await useGetApi<InputMessage[]>(
    `/chats/get-storage-messages`,
    { server: false },
  )
  if (data.value) {
    storageMessages.value = data.value
  }
})
</script>
