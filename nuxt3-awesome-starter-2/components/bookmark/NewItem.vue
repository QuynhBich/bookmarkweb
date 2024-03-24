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
              v-model="link"
              class="appearance-none bg-transparent border-none w-full text-gray-700 mr-3 py-1 px-2 leading-tight focus:outline-none"
              type="text"
              placeholder="Link"
            />
          </div>
          <div class="flex items-center border-b border-sky-500 py-2">
            <input
              v-model="name"
              class="appearance-none bg-transparent border-none w-full text-gray-700 mr-3 py-1 px-2 leading-tight focus:outline-none"
              type="text"
              placeholder="Name"
            />
          </div>
          <div class="flex items-center py-2">
            <input
              v-model="note"
              class="bg-gray-200 appearance-none border-2 border-sky-200 rounded w-full py-2 px-4 leading-tight focus:outline-none focus:bg-white focus:border-sky-500"
              type="text"
              placeholder="Note"
            />
          </div>
          <div>
            <button
              class="flex-shrink-0 bg-sky-500 hover:bg-sky-700 border-sky-500 hover:border-sky-700 text-sm border-4 text-white py-1 px-2 rounded"
              type="button"
              @click="createNewBookmark"
            >
              Create
            </button>
            <button
              class="flex-shrink-0 border-transparent border-4 text-sky-500 hover:text-sky-800 text-sm py-1 px-2 rounded"
              type="button"
              @click="emits('updateDialogStates')"
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
import type { Bookmark } from '../../types/bookmark'
import { getFaviconUrl, getDomain } from '../../utils/parser'
const props = defineProps({
  folderId: {
    type: String,
    default: null,
  },
})
const link = ref('')
const name = ref('')
const note = ref('')
const emits = defineEmits(['updateDialogStates'])
const createNewBookmark = async () => {
  const formData: FormData = new FormData()
  formData.append('url', link.value)
  formData.append('name', name.value)
  formData.append('note', note.value)
  formData.append('folderId', props.folderId)
  const icon = getFaviconUrl(link.value)
  formData.append('image', icon)
  const domain = getDomain(link.value)
  formData.append('domain', domain)
  const { data, status } = await useFetchApi<Bookmark>(
    '/bookmarks/create-bookmark',
    {
      method: 'POST',
      body: formData,
    },
  )
  if (status.value === 'success' && data.value) {
    emits('updateDialogStates')
    useEventBus('bookmarks:update', data.value)
  }
}
</script>
