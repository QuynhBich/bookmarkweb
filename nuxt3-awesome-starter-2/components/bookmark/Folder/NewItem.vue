<template>
  <div
    class="fixed inset-0 flex justify-center items-center z-[60] bg-black bg-opacity-80"
  >
    <div class="relative w-full h-full">
      <div class="flex justify-center items-center h-full">
        <div
          class="bg-white shadow-md rounded px-8 pt-6 pb-8 mb-4 w-full max-w-md flex flex-col gap-4"
        >
          <div class="flex items-center border-b border-sky-500 py-2">
            <input
              v-model="name"
              class="appearance-none bg-transparent border-none w-full text-gray-700 mr-3 py-1 px-2 leading-tight focus:outline-none"
              type="text"
              placeholder="Name"
            />
          </div>
          <div class="flex items-center border-b border-sky-500 py-2">
            <input
              v-model="description"
              class="appearance-none bg-transparent border-none w-full text-gray-700 mr-3 py-1 px-2 leading-tight focus:outline-none"
              type="text"
              placeholder="Description"
            />
          </div>
          <div>
            <button
              class="flex-shrink-0 bg-sky-500 hover:bg-sky-700 border-sky-500 hover:border-sky-700 text-sm border-4 text-white py-1 px-2 rounded"
              type="button"
              @click="createNewFolder"
            >
              Create
            </button>
            <button
              class="flex-shrink-0 border-transparent border-4 text-sky-500 hover:text-sky-800 text-sm py-1 px-2 rounded"
              type="button"
              @click="emits('cancel')"
            >
              Cancel
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script lang="ts" setup>
import type { Folder } from '../../../types/folder'
const name = ref('')
const description = ref('')
const emits = defineEmits(['cancel'])
const createNewFolder = async () => {
  const formData: FormData = new FormData()
  formData.append('name', name.value)
  formData.append('description', description.value)
  const { data, status } = await useFetchApi<Folder>('/folders/create-folder', {
    method: 'POST',
    body: formData,
  })
  if (status.value === 'success' && data.value) {
    emits('cancel')
    useEventBus('folder:update', data.value)
  }
}
</script>
