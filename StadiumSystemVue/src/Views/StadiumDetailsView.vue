<template>
  <div class="container py-4">
    <h2 class="text-center mb-4">🏟️ Stadium Details</h2>

    <div v-if="loading" class="text-center">
      <div class="spinner-border text-primary" role="status"></div>
    </div>

    <div v-else-if="stadium" class="card mx-auto shadow p-4" style="max-width: 700px">
      <img :src="stadium.imageUrl || defaultImage" :alt="stadium.name" class="img-fluid mb-3" />
      <h3>{{ stadium.name }}</h3>
      <p><strong>ID:</strong> {{ stadium.id }}</p>
      <p><strong>Description:</strong> {{ stadium.description || "No description available." }}</p>

      <div class="mt-3 d-flex gap-2">
        <button class="btn btn-success" @click="goToReserve">Reserve This Stadium</button>
        <router-link :to="{ name: 'ManageStadiums' }" v-if="auth.isAdmin" class="btn btn-secondary">
          Manage (Admin)
        </router-link>
      </div>
    </div>

    <div v-else class="alert alert-danger">Stadium not found.</div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import api from "@/api"; 
import { useAuthStore } from "@/store/authstore";

const route = useRoute();
const router = useRouter();
const auth = useAuthStore();

const stadium = ref(null);
const loading = ref(true);
const defaultImage = "https://via.placeholder.com/600x300.png?text=No+Image";

const fetchStadium = async () => {
  loading.value = true;
  try {
    const res = await api.get(`/api/StadiumsApi/${route.params.id}`);
    stadium.value = res.data;
  } catch (err) {
    console.error("Error fetching stadium:", err);
    stadium.value = null;
  } finally {
    loading.value = false;
  }
};

const goToReserve = () => {
  if (!auth.isAuthenticated) {
    alert("Please login first to make a reservation.");
    router.push({ name: "Login" });
    return;
  }
  router.push({ name: "ReserveStadium", params: { id: stadium.value.id } });
};

onMounted(fetchStadium);
</script>

<style scoped>
img { object-fit: cover; max-height: 350px; width: 100%; }
</style>
