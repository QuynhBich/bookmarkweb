<script lang="ts" setup>
import type { Bookmark } from '../types/bookmark'
import type { Folder } from '../types/folder'
import type CommonToats from '../components/common/Toats/index.vue'
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
const updateListBookmark = async () => {
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
useListenBus('bookmarks:update', (val) => {
  updateListBookmark()
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
// import file
const links = ref<(string | null)[]>([])
interface BookmarkParam {
  folderId: string
  url: string
  domain: string
  image: string
}
const toatsCommon = ref<InstanceType<typeof CommonToats> | null>(null)
const importFile = (e: Event) => {
  const fileInput = e.target as HTMLInputElement
  const file = fileInput.files && fileInput!.files[0]
  if (!file) return
  const reader = new FileReader()
  reader.onload = async () => {
    const result = reader.result
    if (typeof result === 'string') {
      const parser = new DOMParser()
      const htmlDoc = parser.parseFromString(result, 'text/html')
      const linksArray = Array.from(htmlDoc.querySelectorAll('dl dt a')).map(
        (link) => link.getAttribute('href'),
      )
      links.value = linksArray.filter((link) => link!)
      const listBookmarks: BookmarkParam[] = []
      links.value.forEach((element) => {
        const icon = getFaviconUrl(element!)
        const domain = getDomain(element!)
        const bookmark = {
          folderId: folderId.value,
          url: element,
          domain: domain!,
          image: icon,
        } as BookmarkParam
        listBookmarks.push(bookmark)
      })
      const param = { list: listBookmarks }
      const { data } = await usePostApi<Bookmark[]>(
        '/bookmarks/create-bunk-of-bookmark',
        JSON.stringify(param),
        { server: false },
      )
      if (data.value) {
        await updateListBookmark()
        toatsCommon.value?.setClose('Import data successfully')
      }
    }
  }
  reader.readAsText(file)
  fileInput.value = ''
}
</script>

<template>
  <div class="w-full h-[calc(100vh-110px)] flex overflow-hidden">
    <div
      class="w-1/6 h-full border-solid border-r-2 border-sky-500 bg-slate-900"
    >
      <BookmarkFolderList
        v-if="listFolder"
        :list-folder="listFolder"
        @update-selected-folder="selectedFolder"
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
        <div
          class="flex gap-1 flex-shrink-0 border-transparent border-4 text-sky-500 hover:text-teal-800 text-sm py-1 px-2 rounded mr-2"
          type="button"
        >
          <label for="fileInput" class="upload-label flex gap-1 cursor-pointer"
            ><Icon class="w-4 h-4" name="fa6-solid:file-import"></Icon>
            <span>Import File</span>
          </label>
          <input
            id="fileInput"
            ref="fileInput"
            type="file"
            class="hidden"
            @change="importFile"
          />
        </div>
      </div>
      <BookmarkList
        v-if="listBookmarks?.length"
        :folder-id="folderId"
        :bookmarks="listBookmarks"
        @preview="showPreview"
        @remove="updateListBookmark"
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
  <CommonToats ref="toatsCommon" type="success"></CommonToats>
</template>
