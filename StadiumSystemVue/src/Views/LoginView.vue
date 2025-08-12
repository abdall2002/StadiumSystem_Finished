<template>
  <div class="container mt-4">
    <h2>Login</h2>
    <form @submit.prevent="login">
      <div class="mb-3">
        <label>Email</label>
        <input v-model="email" type="email" class="form-control" required />
      </div>
      <div class="mb-3">
        <label>Password</label>
        <input
          v-model="password"
          type="password"
          class="form-control"
          required
        />
      </div>
      <button class="btn btn-primary" type="submit">Login</button>
    </form>
    <div v-if="error" class="alert alert-danger mt-2">{{ error }}</div>
  </div>
</template>

<script setup>
import { ref } from "vue";
import api from "../api";
import { useRouter } from "vue-router";
import { useAuthStore } from "@/store/authstore";

const email = ref("");
const password = ref("");
const error = ref("");
const router = useRouter();
const authStore = useAuthStore();

const login = async () => {
  try {
    const response = await api.post("/api/Auth/login", {
      email: email.value,
      password: password.value,
    });

    const token = response.data.token;
    const role = response.data.role;

    authStore.token = token;
    authStore.role = role;
    authStore.isAuthenticated = true;

    localStorage.setItem("token", token);
    localStorage.setItem("role", role);

    router.push("/");
  } catch (err) {
    error.value = err.response?.data || "Login failed";
  }
};
</script>
