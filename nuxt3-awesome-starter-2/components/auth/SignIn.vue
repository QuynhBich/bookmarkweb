<template>
  <div
    v-if="openDialog"
    class="fixed inset-0 flex justify-center items-center z-[60] bg-black bg-opacity-80"
  >
    <div class="relative w-full h-full">
      <div class="flex justify-center items-center h-full">
        <div
          class="bg-white shadow-md rounded px-8 pt-6 pb-8 mb-4 w-full max-w-md"
        >
          <!-- Login -->
          <h2 class="text-3xl font-bold mb-6 text-center text-white">
            <span
              class="bg-gradient-to-r text-transparent from-blue-500 to-purple-500 bg-clip-text"
            >
              {{ !isRegister ? 'LogIn' : 'Register' }}
            </span>
          </h2>
          <form v-if="!isRegister">
            <div class="mb-6">
              <label
                for="email"
                class="block text-gray-700 text-sm font-bold mb-2"
              >
                <i class="fas fa-envelope mr-2"></i>Email/Username
              </label>
              <div>
                <input
                  v-model="email"
                  type="email"
                  class="shadow appearance-none border rounded w-full py-3 px-4 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                  placeholder="Enter your email/username"
                />
              </div>
            </div>
            <div class="mb-6">
              <label
                for="password"
                class="block text-gray-700 text-sm font-bold mb-2"
              >
                <i class="fas fa-lock mr-2"></i>Password
              </label>
              <div>
                <input
                  v-model="password"
                  type="password"
                  class="shadow appearance-none border rounded w-full py-3 px-4 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                  placeholder="Enter your password"
                />
              </div>
            </div>
            <div class="flex items-center justify-center">
              <div
                class="bg-gradient-to-r from-blue-500 to-purple-500 hover:from-blue-700 hover:to-purple-700 text-white font-bold py-3 px-4 rounded focus:outline-none focus:shadow-outline w-full"
                @click="signIn"
              >
                LogIn
              </div>
            </div>
            <div class="text-center mt-4">
              <a href="#" class="text-gray-600 hover:underline"
                >Forgot password?</a
              >
            </div>
          </form>
          <!-- Register -->
          <form v-else>
            <div class="mb-6">
              <label
                for="email"
                class="block text-gray-700 text-sm font-bold mb-2"
              >
                <i class="fas fa-envelope mr-2"></i>Username
              </label>
              <div>
                <input
                  v-model="username"
                  type="email"
                  class="shadow appearance-none border rounded w-full py-3 px-4 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                  placeholder="Enter your email"
                />
              </div>
            </div>
            <div class="mb-6">
              <label
                for="email"
                class="block text-gray-700 text-sm font-bold mb-2"
              >
                <i class="fas fa-envelope mr-2"></i>Email
              </label>
              <div>
                <input
                  v-model="email"
                  type="email"
                  class="shadow appearance-none border rounded w-full py-3 px-4 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                  placeholder="Enter your email"
                />
              </div>
            </div>
            <div class="mb-6">
              <label
                for="password"
                class="block text-gray-700 text-sm font-bold mb-2"
              >
                <i class="fas fa-lock mr-2"></i>Password
              </label>
              <div>
                <input
                  v-model="password"
                  type="password"
                  class="shadow appearance-none border rounded w-full py-3 px-4 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                  placeholder="Enter your password"
                />
              </div>
            </div>
            <div class="flex items-center justify-center">
              <div
                class="bg-gradient-to-r from-blue-500 to-purple-500 hover:from-blue-700 hover:to-purple-700 text-white font-bold py-3 px-4 rounded focus:outline-none focus:shadow-outline w-full"
                @click="register"
              >
                Register
              </div>
            </div>
          </form>
          <!-- Don't have an account? -->
          <p class="text-center text-gray-600 mt-6">
            {{
              !isRegister ? "Don't have an account?" : 'Already have an account'
            }}
            <a
              href="#"
              class="text-blue-500 hover:underline"
              @click="isRegister = !isRegister"
              >{{ !isRegister ? 'Sign up' : 'Login' }}</a
            >
          </p>
        </div>
      </div>
    </div>
  </div>
</template>
<script lang="ts" setup>
import type { ILoginInput } from '../../types/auth'
const openDialog = ref(false)
const email = ref('')
const password = ref('')
const username = ref('')
onMounted(() => {
  useListenBus('dialog:signIn', (val) => {
    openDialog.value = val
  })
})
const signIn = async () => {
  const input: ILoginInput = {
    email: email.value,
    password: password.value,
  }
  const { authenticateUser } = useAuthStore()

  await authenticateUser(input)
  const { authenticated } = storeToRefs(useAuthStore())

  if (authenticated) {
    openDialog.value = false
  }
}

const refreshData = () => {
  email.value = ''
  password.value = ''
  username.value = ''
}
// Register
const isRegister = ref(false)
watch(isRegister, () => {
  refreshData()
})
const register = async () => {
  const formData: FormData = new FormData()
  formData.append('username', username.value)
  formData.append('email', email.value)
  formData.append('password', password.value)

  const { data, status } = await useFetchApi('/auth/register', {
    method: 'POST',
    body: formData,
  })

  if (status.value === 'success') {
    signIn()
    refreshData()
  }
}
</script>
