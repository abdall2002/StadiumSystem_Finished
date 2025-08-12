<template>
  <div class="container mt-4">
    <h2>Register</h2>
    <form @submit.prevent="register">
      <div class="mb-3">
        <label>Name</label>
        <input v-model="name" type="text" class="form-control" required />
      </div>
      <div class="mb-3">
        <label>Email</label>
        <input v-model="email" type="email" class="form-control" required />
      </div>
      <div class="mb-3">
        <label>Password</label>
        <input v-model="password" type="password" class="form-control" required />
      </div>
      <button class="btn btn-success" type="submit">Register</button>
    </form>

    <div v-if="error" class="alert alert-danger mt-2">{{ error }}</div>
  </div>
</template>

<script setup>
import { ref } from "vue";
import api from "@/api"; 
import { useRouter } from "vue-router";

const name = ref("");
const email = ref("");
const password = ref("");
const error = ref("");
const router = useRouter();

const register = async () => {
  try {
    error.value = "";

    const response = await api.post("/api/Auth/register", {
      name: name.value,
      email: email.value,
      password: password.value,
    });

    if (response.data.token) {
      localStorage.setItem("token", response.data.token);
      router.push("/");
    } else {
      router.push("/login");
    }

  } catch (err) {
    error.value = err.response?.data?.message || "Registration failed.";
  }
};
</script>
