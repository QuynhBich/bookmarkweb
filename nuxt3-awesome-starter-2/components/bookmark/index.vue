<template>
  <div class="grid gap-5 grid-cols-1 p-2">
    <BookmarkItem
      v-for="(bookmark, key) in listBookmarks"
      :key="key"
      :bookmark="bookmark"
    >
    </BookmarkItem>
  </div>
</template>
<script lang="ts" setup>
import type { Bookmark } from '../../types/bookmark'

const listBookmarks = ref<Bookmark[]>()
onMounted(async () => {
  const { data } = await useFetchApi<Bookmark[]>(`/bookmarks`, {
    method: 'GET',
  })
  if (data.value) listBookmarks.value = data.value
})
</script>
