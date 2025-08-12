<template>
  <div>
    <Navbar />
    <main class="container py-4">
      <router-view />
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import api from "../src/api";
import Navbar from "./components/Navbar.vue";

const stadiums = ref([]);
const loading = ref(true);

onMounted(async () => {
  try {
    const response = await api.get("/api/StadiumsApi");
    stadiums.value = response.data;
  } catch (error) {
    console.error("Error fetching stadiums:", error);
  } finally {
    loading.value = false;
  }
});
</script>

<style scoped>
.card-img-top {
  border-bottom: 1px solid #ddd;
}
</style>
