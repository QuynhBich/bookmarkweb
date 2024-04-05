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
const bookmarks = ref(props.bookmarks)
watch(
  () => props.folderId,
  (newValue, oldValue) => {
    bookmarks.value = props.bookmarks.filter(
      (item: Bookmark) => item.folderId === props.folderId,
    )
  },
)
watch(
  () => props.bookmarks,
  (newValue, oldValue) => {
    bookmarks.value = props.bookmarks.filter(
      (item: Bookmark) => item.folderId === props.folderId,
    )
  },
)
useListenBus('bookmarks:update', (val) => {
  bookmarks.value?.unshift(val)
  bookmarks.value = []
  bookmarks.value = props.bookmarks.filter(
    (item: Bookmark) => item.folderId === props.folderId,
  )
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
    }, 1000)
  }
}
</script>
