<template>
  <LayoutPageWrapper class="overflow-hidden">
    <div
      v-for="(group, updatedAt) in groupedData"
      :key="updatedAt"
      class="overflow-y-auto"
    >
      <h2 class="p-1 font-bold text-sky-200">{{ updatedAt }}</h2>
      <div class="flex flex-wrap gap-2">
        <div v-for="mess in group" :key="mess.id">
          <ChatStorage
            :message="mess"
            @delete="deleteStorageMessage"
          ></ChatStorage>
        </div>
      </div>
    </div>
  </LayoutPageWrapper>
</template>
<script lang="ts" setup>
import type { InputMessage } from '../../types/conversation'

definePageMeta({ layout: 'page' })
useHead({ title: 'Storage' })
const storageMessages = ref<InputMessage[]>([])
const groupedData = ref<{ [key: string]: InputMessage[] }>({})
onNuxtReady(async () => {
  await getData()
})
const getData = async () => {
  const { data } = await useGetApi<InputMessage[]>(
    `/chats/get-storage-messages`,
    { server: false },
  )
  if (data.value) {
    storageMessages.value = data.value
  }
  groupData()
}
const groupData = () => {
  groupedData.value = storageMessages.value.reduce((acc, item) => {
    const updatedAt = item.updatedAt?.toString().split('T')[0]
    if (!acc[updatedAt!]) {
      acc[updatedAt!] = []
    }
    acc[updatedAt!].push(item)
    return acc
  }, {})
}

// delete
const deleteStorageMessage = async (id: string) => {
  const { status } = await usePostApi(`/chats/delete/${id}`, { server: false })
  if (status.value === 'success') {
    storageMessages.value = storageMessages.value.filter((x) => x.id !== id)
    groupData()
  }
}
</script>
