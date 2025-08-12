<template>
  <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
    <div class="container">
      <router-link class="navbar-brand" to="/"
        >🏟️ Stadium Reservation</router-link
      >

      <button
        class="navbar-toggler"
        type="button"
        data-bs-toggle="collapse"
        data-bs-target="#navbarNav"
      >
        <span class="navbar-toggler-icon"></span>
      </button>

      <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav me-auto mb-2 mb-lg-0">
          <li class="nav-item">
            <router-link class="nav-link" to="/">All Stadiums</router-link>
          </li>

          <li
            v-if="auth.isAuthenticated && auth.role !== 'Admin'"
            class="nav-item"
          >
            <router-link class="nav-link" to="/my-reservations"
              >My Reservations</router-link
            >
          </li>

          <li
            v-if="auth.isAuthenticated && auth.role === 'Admin'"
            class="nav-item"
          >
            <router-link class="nav-link" to="/manage-stadiums"
              >Manage Stadiums</router-link
            >
          </li>

          <li
            v-if="auth.isAuthenticated && auth.role === 'Admin'"
            class="nav-item"
          >
            <router-link class="nav-link" to="/all-reservations">
              📋 All Reservations
            </router-link>
          </li>
        </ul>

        <ul class="navbar-nav mb-2 mb-lg-0">
          <template v-if="auth.isAuthenticated">
            <li class="nav-item d-flex align-items-center text-light me-2">
              Hello, {{ auth.userName || "User" }}
            </li>
            <li class="nav-item">
              <button class="btn btn-outline-light btn-sm" @click="logout">
                Logout
              </button>
            </li>
          </template>

          <template v-else>
            <li class="nav-item">
              <router-link class="nav-link" to="/login">Login</router-link>
            </li>
            <li class="nav-item">
              <router-link class="nav-link" to="/register"
                >Register</router-link
              >
            </li>
          </template>
        </ul>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { onMounted } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "@/store/authstore";

const router = useRouter();
const auth = useAuthStore();

onMounted(() => {
  auth.checkAuth();
});

const logout = () => {
  auth.logout();
  router.push("/login");
};
</script>

<style scoped>
.navbar {
  margin-bottom: 20px;
}
</style>
