<script lang="ts" setup>
const { awesome } = useAppConfig()
definePageMeta({ layout: 'page' })
useHead({ titleTemplate: '', title: awesome?.name || 'Nuxt 3 Awesome Starter' })
</script>

<template>
  <div class="w-full h-[calc(100vh-100px)] flex">
    <div
      class="w-1/6 h-full border-solid border-r-2 border-sky-500 bg-slate-900"
    >
      Folder here
    </div>
    <div class="w-5/6 h-full bg-slate-900">Bookmark here</div>
  </div>
</template>
