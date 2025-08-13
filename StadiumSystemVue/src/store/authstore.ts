import { defineStore } from 'pinia'
import axios from 'axios'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('token') || '',
    role: localStorage.getItem('role') || '',
    userName: localStorage.getItem('userName') || '',
    isAuthenticated: !!localStorage.getItem('token')
  }),

  actions: {
    async login(email: string, password: string) {
      try {
        const response = await axios.post('https://localhost:7279/api/Auth/login', {
          userName: email,
          password: password
        })

        const token = response.data.token
        const role = response.data.role
        const userName = response.data.userName || email 

        this.token = token
        this.role = role
        this.userName = userName
        this.isAuthenticated = true

        localStorage.setItem('token', token)
        localStorage.setItem('role', role)
        localStorage.setItem('userName', userName)
      } catch (error) {
        console.error('Login failed:', error)
        throw error
      }
    },

    logout() {
      this.token = ''
      this.role = ''
      this.userName = ''
      this.isAuthenticated = false
      localStorage.removeItem('token')
      localStorage.removeItem('role')
      localStorage.removeItem('userName')
    },

    checkAuth() {
      const token = localStorage.getItem('token')
      const role = localStorage.getItem('role')
      const userName = localStorage.getItem('userName')

      this.token = token || ''
      this.role = role || ''
      this.userName = userName || ''
      this.isAuthenticated = !!token
    }
  },

  getters: {
    isAdmin: (state) => state.role === 'Admin'
  }
})
