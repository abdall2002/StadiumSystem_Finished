<!-- AdminCheck.vue -->
<template>
  <div v-if="auth.role === 'Admin'">
    <p>مرحبًا أيها الأدمن</p>
    <!-- Admin UI here -->
  </div>
  <div v-else>
    <p>أنت مستخدم عادي</p>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/store/authstore'
const auth = useAuthStore()
</script>
