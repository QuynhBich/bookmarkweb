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
const updateListBookmark = async () => {
  console.log('quynh test')
  listBookmarks.value = []
  const { data } = await useFetchApi<Bookmark[]>(`/bookmarks`, {
    method: 'GET',
  })
  if (data.value) listBookmarks.value = data.value
}
const { authenticated } = storeToRefs(useAuthStore())
onNuxtReady(async () => {
  await updateListBookmark()
})
watch(authenticated, async () => {
  const { data } = await useFetchApi<Bookmark[]>(`/bookmarks`, {
    method: 'GET',
  })
  if (data.value) listBookmarks.value = data.value
})

// list folder
const listFolder = ref<Folder[]>()
onNuxtReady(async () => {
  const { data: result } = await useFetchApi<Folder[]>(`/folders`, {
    method: 'GET',
  })
  if (result.value) listFolder.value = result.value
})
watch(authenticated, async () => {
  const { data } = await useFetchApi<Folder[]>(`/folders`, {
    method: 'GET',
  })
  if (data.value) listFolder.value = data.value
})

// preview
const drawerVisible = ref(false)
const selectedBookmark = ref<Bookmark>()
const showPreview = (val: Bookmark) => {
  selectedBookmark.value = val
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
        :list-folder="listFolder"
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
        v-if="listBookmarks"
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
    v-if="selectedBookmark"
    :drawer-visible="drawerVisible"
    :bookmark="selectedBookmark"
    @close="closePreview"
    @update-bookmarks="updateListBookmark"
  ></BookmarkToolbar>
</template>
