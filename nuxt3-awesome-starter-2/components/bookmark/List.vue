<template>
  <div class="grid gap-5 grid-cols-1 p-2 overflow-auto h-full">
    <BookmarkItem
      v-for="bookmark in bookmarks"
      :key="bookmark.id"
      :bookmark="bookmark"
      @preview="openPreview"
    >
    </BookmarkItem>
  </div>
</template>
<script lang="ts" setup>
import type { Bookmark } from '../../types/bookmark'
const emit = defineEmits(['preview'])
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
</script>
