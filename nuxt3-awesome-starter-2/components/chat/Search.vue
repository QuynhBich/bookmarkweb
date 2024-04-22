<template>
  <div
    class="absolute w-80 h-10 bg-black opacity-75 top-1/2 text-white z-50 rounded-lg flex justify-center"
  >
    <input
      v-model="keySearch"
      type="text"
      class="bg-transparent outline-none text-lg"
      placeholder="Search here"
      @input="emit('searching')"
    />
  </div>
</template>
<script lang="ts" setup>
const keySearch = ref('')
const emit = defineEmits(['searching'])
defineExpose({ keySearch })
</script>
