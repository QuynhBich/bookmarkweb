<template>
  <div class="flex justify-center h-full w-full items-center mt-20">
    <div
      class="rounded-md bg-[url('https://cdn.statically.io/img/timelinecovers.pro/facebook-cover/download/moon-on-the-blue-sky-facebook-cover.jpg')] bg-cover bg-center bg-no-repeat items-center p-5"
    >
      <h1
        class="lg:text-3xl md:text-2xl sm:text-xl xs:text-xl font-serif font-extrabold mb-2 dark:text-white"
      >
        Profile
      </h1>
      <form>
        <!-- Cover Image -->
        <div class="w-full rounded-sm">
          <!-- Profile Image -->
          <div
            class="mx-auto flex justify-center w-[141px] h-[141px] rounded-full bg-cover bg-center bg-no-repeat"
          >
            <Icon
              name="fxemoji:koala"
              class="font-black text-4xl font-mono mr-2 inline-block w-full h-full"
            />
          </div>
        </div>
        <div
          class="flex lg:flex-row md:flex-col sm:flex-col xs:flex-col gap-2 justify-center w-full"
        >
          <div class="w-full mb-4 mt-6">
            <label for="" class="mb-2 dark:text-gray-300">Username</label>
            <input
              v-model="username"
              type="text"
              class="mt-2 p-4 w-full border-2 rounded-lg dark:text-gray-200 dark:border-gray-600 dark:bg-gray-800"
            />
          </div>
          <div class="w-full mb-4 lg:mt-6">
            <label for="" class="dark:text-gray-300">Password</label>
            <input
              v-model="password"
              type="password"
              class="mt-2 p-4 w-full border-2 rounded-lg dark:text-gray-200 dark:border-gray-600 dark:bg-gray-800"
            />
          </div>
        </div>
        <div class="w-32 rounded-lg bg-sky-500 text-white float-end">
          <button class="w-full p-1">Save</button>
        </div>
      </form>
    </div>
  </div>
</template>
<script lang="ts" setup>
definePageMeta({ layout: 'page' })
const { user } = useAuthStore()
const username = ref('')
const password = ref('')
onNuxtReady(() => {
  if (user?.Username) username.value = user.Username
})
</script>
