<template>
  <div
    class="flex h-full w-full flex-col border-r border-neutral-100 bg-[#FBFBFB] dark:border-neutral-800 dark:bg-neutral-900"
  >
    <div class="p-2">
      <label for="search">
        <div class="relative h-8 flex justify-center items-center gap-3">
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
            class="w-4/5 h-full rounded-md border-gray-200 pl-10 text-gray-700 shadow-sm outline-none focus:border-gray-300 focus:ring-0 dark:border-neutral-700 dark:bg-neutral-800 dark:text-neutral-200 focus:dark:border-neutral-600 sm:text-sm"
            type="text"
            placeholder="Type to filter"
            autocomplete="off"
          />
          <button class="h-6 w-6 bg-sky-500 rounded-full" @click="openDialog">
            +
          </button>
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
            :class="{ ['bg-neutral-800']: item.id === selectedFolder }"
            class="my-1 flex cursor-pointer place-items-end items-center rounded-md p-2 text-gray-700 hover:bg-neutral-100 dark:text-neutral-400 dark:hover:bg-neutral-800"
            @click="clickFolder(item)"
          >
            <span class="truncate">{{ item.name }}</span>
          </label>
        </li>
      </transition-group>
    </div>
  </div>
  <bookmark-folder-new-item
    v-if="isOpenDialog"
    @cancel="openDialog"
  ></bookmark-folder-new-item>
</template>
<script lang="ts" setup>
import type { Folder } from '../../../types/folder'
const emit = defineEmits(['updateSelectedFolder'])
const props = defineProps({
  listFolder: {
    type: Array<Folder>,
    default: [],
  },
})
const selectedFolder = ref('')
const clickFolder = (item: Folder) => {
  selectedFolder.value = item.id
  emit('updateSelectedFolder', selectedFolder.value)
}
const { authenticated } = storeToRefs(useAuthStore())

const term = ref('')
const list = computed(() => {
  if (term.value === '') {
    return props.listFolder
  }
  if (props.listFolder)
    return props.listFolder.filter((item: Folder) =>
      item.name.toLowerCase().includes(term.value.toLowerCase()),
    )
})
onMounted(() => {
  if (!selectedFolder.value) {
    selectedFolder.value = props.listFolder[0]?.id
    emit('updateSelectedFolder', selectedFolder.value)
  }
})

// Create new folder
const isOpenDialog = ref(false)
const openDialog = () => {
  isOpenDialog.value = !isOpenDialog.value
}
useListenBus('folder:update', (val) => {
  list.value?.unshift(val)
})
</script>
