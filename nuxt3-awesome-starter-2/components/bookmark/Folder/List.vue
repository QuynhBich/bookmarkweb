<template>
  <div
    class="flex h-full w-full flex-col border-r border-neutral-100 bg-[#FBFBFB] dark:border-neutral-800 dark:bg-neutral-900"
  >
    <div class="p-2">
      <label for="search">
        <div class="relative">
          <div
            class="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3"
          >
            <!-- <Icon
              name="lucide:baby"
              class="font-black text-4xl font-mono mr-2 inline-block"
            /> -->
          </div>
          <input
            id="search"
            v-model="term"
            class="w-full rounded-md border-gray-200 pl-10 text-gray-700 shadow-sm outline-none focus:border-gray-300 focus:ring-0 dark:border-neutral-700 dark:bg-neutral-800 dark:text-neutral-200 focus:dark:border-neutral-600 sm:text-sm"
            type="text"
            placeholder="Type to filter"
            autocomplete="off"
          />
        </div>
      </label>
      <transition-group
        tag="ul"
        name="fade"
        class="flex h-screen scroll-p-0.5 flex-col overflow-y-auto"
      >
        <li v-for="(item, key) in list" :key="key">
          <label
            :key="key"
            class="my-1 flex cursor-pointer place-items-end items-center rounded-md p-2 text-gray-700 hover:bg-neutral-100 dark:text-neutral-400 dark:hover:bg-neutral-800"
            @click="clickFolder(item)"
          >
            <span class="truncate">{{ item.name }}</span>
          </label>
        </li>
      </transition-group>
    </div>
  </div>
</template>
<script lang="ts" setup>
import type { Folder } from '../../../types/folder'
const emit = defineEmits(['update:selectedFolder'])
// const props = defineProps({
//   modelValue: {
//     type: String,
//     default: '',
//   },
//   items: {
//     type: Array<Folder>,
//     default: [],
//   },
// })
const selectedFolder = ref('')
const clickFolder = (item: Folder) => {
  selectedFolder.value = item.id
  emit('update:selectedFolder', selectedFolder.value)
}

const listFolder = ref<Folder[]>()
const { data } = await useFetchApi<Folder[]>(`/folders`, {
  method: 'GET',
})
if (data.value) listFolder.value = data.value
const { authenticated } = storeToRefs(useAuthStore())
watch(authenticated, async () => {
  const { data } = await useFetchApi<Folder[]>(`/folders`, {
    method: 'GET',
  })
  if (data.value) listFolder.value = data.value
})
const term = ref('')
const list = computed(() => {
  if (term.value.length === 0) {
    return listFolder.value
  }
  if (listFolder.value)
    return listFolder.value.filter((item: Folder) =>
      item.name.toLowerCase().includes(term.value.toLowerCase()),
    )
})

if (
  selectedFolder.value === '' &&
  list.value &&
  list.value.length > 0 &&
  list.value[0].id
) {
  selectedFolder.value = list.value[0].id
  emit('update:selectedFolder', selectedFolder.value)
}
</script>
