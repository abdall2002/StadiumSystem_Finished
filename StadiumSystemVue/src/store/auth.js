import { ref } from 'vue'
import { defineStore } from 'pinia'
import router from '@/router'

export const useAuthStore = defineStore('auth', () => {
  const isAuthenticated = ref(false)
  const userName = ref('')
  const isAdmin = ref(false)
  const token = ref('')

  const login = async (credentials) => {
    try {
      const res = await fetch('https://localhost:7050/api/Auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(credentials)
      })

      if (!res.ok) throw new Error('Invalid credentials')
      const data = await res.json()

      token.value = data.token
      userName.value = data.userName
      isAdmin.value = data.isAdmin
      isAuthenticated.value = true

      localStorage.setItem('token', token.value)
      router.push('/')
    } catch (err) {
      console.error(err)
      throw err
    }
  }

  const logout = () => {
    token.value = ''
    userName.value = ''
    isAdmin.value = false
    isAuthenticated.value = false
    localStorage.removeItem('token')
    router.push('/login')
  }

  const checkAuth = () => {
    const savedToken = localStorage.getItem('token')
    if (savedToken) {
      token.value = savedToken
      isAuthenticated.value = true
    }
  }

  return {
    isAuthenticated,
    userName,
    isAdmin,
    token,
    login,
    logout,
    checkAuth
  }
})
