<template>
  <div
    class="group relative flex h-fit w-full rounded-md border border-solid bg-white p-3 shadow-sm dark:border-neutral-700 dark:bg-neutral-800"
  >
    <a :href="currentBookmark.url" target="_blank" class="w-full">
      <div
        class="mb-2 inline-flex items-center text-sm font-semibold text-gray-900 dark:text-neutral-100"
      >
        <img :src="currentBookmark.image" alt="favicon" class="w-4 h-4 m-2" />
        {{ currentBookmark.domain }}
      </div>
      <div class="flex flex-col">
        <span class="text-xs text-gray-900 dark:text-neutral-100">{{
          currentBookmark.domain
        }}</span>
        <p class="my-2 break-words text-gray-700 dark:text-neutral-300">
          What is facebook?
          {{ bookmark.description }}
        </p>
        <div class="flex space-x-2"></div>
      </div>
    </a>
    <div class="absolute right-2 top-2 invisible group-hover:visible">
      <button
        class="m-1 rounded-full bg-rose-400 p-1.5 text-white shadow-lg transition duration-150 ease-in-out hover:shadow-lg focus:bg-rose-400 focus:shadow-lg focus:ring-0"
        @click="emit('remove', currentBookmark.id)"
      >
        <Icon name="solar:trash-bin-2-bold" class="w-4 h-4" />
      </button>
      <button
        class="m-1 rounded-full bg-gray-800 p-1.5 text-white shadow-lg transition duration-150 ease-in-out hover:bg-gray-900 hover:shadow-lg focus:bg-gray-900 focus:ring-0"
        @click="emit('edit', currentBookmark)"
      >
        <Icon name="ic:baseline-edit" class="w-4 h-4" />
      </button>
      <button
        class="m-1 rounded-full bg-gray-800 p-1.5 text-white shadow-lg transition duration-150 ease-in-out hover:bg-gray-900 hover:shadow-lg focus:bg-gray-900 focus:ring-0"
        @click="emit('preview', currentBookmark)"
      >
        <Icon name="mdi:eye" class="w-4 h-4"></Icon>
      </button>
    </div>
  </div>
</template>
<script lang="ts" setup>
import type { Bookmark } from '../../types/bookmark'
const emit = defineEmits(['preview', 'edit', 'remove'])
const props = defineProps({
  bookmark: {
    type: Object as PropType<Bookmark>,
    default: null,
  },
})
const currentBookmark = computed(() => {
  return props.bookmark
})
</script>
