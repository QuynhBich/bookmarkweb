<script lang="ts" setup>
import type { Bookmark } from '../types/bookmark'
import type { Folder } from '../types/folder'
const { awesome } = useAppConfig()
definePageMeta({ layout: 'page' })
useHead({ titleTemplate: '', title: awesome?.name || 'Nuxt 3 Awesome Starter' })
const folderId = ref('')
const selectedFolder = (id: string) => {
  folderId.value = id
}
const openNewBookmarkDialog = ref(false)

// list book mark
const listBookmarks = ref<Bookmark[]>()
const { data } = await useFetchApi<Bookmark[]>(`/bookmarks`, {
  method: 'GET',
})
if (data.value) listBookmarks.value = data.value
const { authenticated } = storeToRefs(useAuthStore())
watch(authenticated, async () => {
  const { data } = await useFetchApi<Bookmark[]>(`/bookmarks`, {
    method: 'GET',
  })
  if (data.value) listBookmarks.value = data.value
})

// preview
const drawerVisible = ref(false)
const showPreview = (val: Bookmark) => {
  console.log(val)
  drawerVisible.value = true
}
const closePreview = () => {
  drawerVisible.value = false
}
</script>

<template>
  <div class="w-full h-[calc(100vh-110px)] flex overflow-hidden">
    <div
      class="w-1/6 h-full border-solid border-r-2 border-sky-500 bg-slate-900"
    >
      <BookmarkFolderList
        @update:selected-folder="selectedFolder"
      ></BookmarkFolderList>
    </div>
    <div class="w-5/6 h-full bg-slate-900">
      <div class="h-fit w-full flex justify-end">
        <button
          class="flex-shrink-0 border-transparent border-4 text-sky-500 hover:text-teal-800 text-sm py-1 px-2 rounded"
          type="button"
          @click="openNewBookmarkDialog = true"
        >
          + Add Bookmark
        </button>
      </div>
      <BookmarkList
        :folder-id="folderId"
        :bookmarks="listBookmarks"
        @preview="showPreview"
      ></BookmarkList>
    </div>
  </div>
  <BookmarkNewItem
    v-if="openNewBookmarkDialog"
    :folder-id="folderId"
    @update-dialog-states="openNewBookmarkDialog = !openNewBookmarkDialog"
  ></BookmarkNewItem>
  <BookmarkToolbar
    :drawer-visible="drawerVisible"
    @close="closePreview"
  ></BookmarkToolbar>
</template>
