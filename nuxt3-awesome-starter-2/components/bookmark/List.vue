<template>
  <div
    class="h-10 mr-5 ml-2 flex justify-center items-center bg-neutral-800 rounded-md"
  >
    <Icon
      name="material-symbols-light:search-rounded"
      class="font-black text-2xl ml-2"
    />
    <input
      id="search"
      v-model="term"
      class="w-full h-full rounded-md border-gray-200 pl-2 text-gray-700 shadow-sm outline-none focus:border-gray-300 focus:ring-0 dark:border-neutral-700 dark:bg-neutral-800 dark:text-neutral-200 focus:dark:border-neutral-600"
      type="text"
      placeholder="Type to filter"
      autocomplete="off"
    />
  </div>
  <div class="flex flex-col gap-5 p-2 overflow-auto h-full">
    <BookmarkItem
      v-for="bookmark in bookmarks"
      :key="bookmark.id"
      :bookmark="bookmark"
      @preview="openPreview"
      @remove="removeBookmark"
    >
    </BookmarkItem>
    <CommonToats ref="toatsCommon" type="success"></CommonToats>
  </div>
</template>
<script lang="ts" setup>
import type { Bookmark } from '../../types/bookmark'
import type { CommonToats } from '#build/components'
const toatsCommon = ref<InstanceType<typeof CommonToats> | null>(null)
const emit = defineEmits(['preview', 'remove'])
const props = defineProps({
  bookmarks: {
    type: Array<Bookmark>,
    default: [],
  },
  folderId: {
    type: String,
    default: null,
  },
})
const term = ref('')
const bookmarks = ref(props.bookmarks)
const filterBookmarks = () => {
  bookmarks.value = props.bookmarks.filter(
    (item: Bookmark) =>
      item.folderId === props.folderId &&
      item.domain.toLowerCase().includes(term.value.toLowerCase()),
  )
}
watch(
  () => props.folderId,
  (newValue, oldValue) => {
    filterBookmarks()
  },
)
watch(
  () => props.bookmarks,
  (newValue, oldValue) => {
    filterBookmarks()
  },
)
watch(
  () => term.value,
  (newValue, oldValue) => {
    filterBookmarks()
  },
)
useListenBus('bookmarks:update', (val) => {
  bookmarks.value?.unshift(val)
  bookmarks.value = []
  filterBookmarks()
})
const openPreview = (val: Bookmark) => {
  emit('preview', val)
}

// delete bookmark
const removeBookmark = async (id: string) => {
  const { status } = await usePostApi(`bookmarks/delete-bookmark/${id}`, {
    server: false,
  })
  if (status.value === 'success') {
    bookmarks.value = bookmarks.value.filter((x) => x.id !== id)
    toatsCommon.value?.setClose()
    setTimeout(() => {
      emit('remove')
    }, 3000)
  }
}
</script>
