<template>
  <div>
    <div
      class="absolute right-0 top-0 z-50 h-screen w-0 overflow-hidden bg-white pl-0 transition-all dark:bg-neutral-900 dark:text-neutral-400"
      :style="{
        width: drawerVisible ? '40vw' : '0',
        paddingLeft: drawerVisible ? '10px' : '0',
      }"
    >
      <button
        type="button"
        class="absolute right-2.5 top-2.5 inline-flex h-8 w-8 items-center justify-center rounded-lg bg-transparent text-sm text-gray-400 hover:bg-gray-200 hover:text-gray-900 dark:hover:bg-gray-600 dark:hover:text-white"
        @click="close"
      >
        <Icon name="bytesize:close" class="w-4 h-4" />
        <span class="sr-only">Close menu</span>
      </button>
      <div class="mt-10 flex w-full justify-center py-1">
        <slot name="edit" />
      </div>
      <!-- <div v-for="(text, index) in result" :key="index" class="w-full h-full">
        {{ text }}
      </div> -->
      <!-- <div class="w-full h-full overflow-auto">{{ textElements }}</div> -->
      <!-- <iframe src="https://kenh14.vn/" class="w-full h-full"></iframe> -->
      <div class="w-full h-full">
        <Chat></Chat>
      </div>
    </div>
    <div
      class="absolute left-0 top-0 z-10 h-screen w-0 bg-black/30 transition-opacity"
      :style="{
        width: drawerVisible ? '100vw' : '0',
        opacity: drawerVisible ? '0.6' : '0',
      }"
      @keydown="close"
      @click="close"
    />
  </div>
</template>
<script lang="ts" setup>
const props = defineProps({
  drawerVisible: {
    type: Boolean,
    default: false,
  },
})
const emit = defineEmits(['close'])
const close = () => {
  emit('close')
}
watch(
  () => props.drawerVisible,
  (oldValue, newValue) => {},
)
</script>
