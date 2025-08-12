<template>
  <main class="container py-4">
    <h2 class="text-center mb-4">🏟️ All Stadiums</h2>

    <div v-if="loading" class="text-center">
      <div class="spinner-border" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>

    <div v-else>
      <div v-if="stadiums.length === 0" class="alert alert-info text-center">
        No stadiums found.
      </div>

      <div class="row g-4">
        <div
          v-for="stadium in stadiums"
          :key="stadium.id"
          class="col-12 col-sm-6 col-md-4 col-lg-3"
        >
          <div class="card h-100 shadow-sm">
            <img
              :src="stadium.imageUrl || defaultImage"
              class="card-img-top"
              :alt="stadium.name"
              style="object-fit: cover; height: 200px"
            />
            <div class="card-body text-center">
              <h5 class="card-title">{{ stadium.name }}</h5>
              <router-link
                :to="`/stadiums/${stadium.id}`"
                class="btn btn-primary btn-sm mt-2"
              >
                View Details
              </router-link>
            </div>
          </div>
        </div>
      </div>
    </div>
  </main>
</template>

<script setup>
import { ref, onMounted } from "vue";
import api from "../api";

const stadiums = ref([]);
const loading = ref(true);
const defaultImage = "https://via.placeholder.com/400x200.png?text=No+Image";

onMounted(async () => {
  try {
    console.log("Fetching stadiums...");
    const response = await api.get("/api/StadiumsApi");
    console.log("Response:", response.data);
    stadiums.value = response.data;
  } catch (error) {
    console.error("Error fetching stadiums:", error);
  } finally {
    loading.value = false;
  }
});
</script>
