<template>
  <div class="grid gap-5 grid-cols-1 p-2 overflow-auto h-full">
    <BookmarkItem
      v-for="(bookmark, key) in bookmarks"
      :key="key"
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
  console.log('list')
  emit('preview', val)
}
</script>
