<template>
  <div class="flex flex-col gap-5 p-2 overflow-auto h-full">
    <BookmarkItem
      v-for="bookmark in bookmarks"
      :key="bookmark.id"
      :bookmark="bookmark"
      @preview="openPreview"
      @remove="removeBookmark"
    >
    </BookmarkItem>
  </div>
</template>
<script lang="ts" setup>
import type { Bookmark } from '../../types/bookmark'
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
const bookmarks = ref(props.bookmarks)
watch(
  () => props.folderId,
  (newValue, oldValue) => {
    bookmarks.value = props.bookmarks
    bookmarks.value = bookmarks.value.filter(
      (item: Bookmark) => item.folderId === newValue,
    )
  },
)
useListenBus('bookmarks:update', (val) => {
  bookmarks.value?.unshift(val)
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
    emit('remove')
  }
}
</script>
