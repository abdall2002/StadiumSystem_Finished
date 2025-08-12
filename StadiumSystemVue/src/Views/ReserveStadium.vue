<template>
  <div class="container py-4" style="max-width: 720px;">
    <h2 class="mb-4">Reserve Stadium</h2>

    <div v-if="loading" class="text-center mb-3">
      <div class="spinner-border text-primary" role="status"></div>
    </div>

    <div v-else>
      <div class="card p-4 shadow-sm mb-3">
        <h5 class="mb-3">{{ stadium?.name || 'Stadium' }}</h5>

        <form @submit.prevent="submitReservation">
          <div class="mb-3">
            <label class="form-label">Date</label>
            <input v-model="reservationDate" type="date" class="form-control" required />
          </div>

          <div class="row">
            <div class="col-md-6 mb-3">
              <label>Start Time</label>
              <input v-model="startTime" type="time" class="form-control" required />
            </div>
            <div class="col-md-6 mb-3">
              <label>End Time</label>
              <input v-model="endTime" type="time" class="form-control" required />
            </div>
          </div>

          <div v-if="serverErrors.length" class="alert alert-danger">
            <ul class="mb-0">
              <li v-for="(e, i) in serverErrors" :key="i">{{ e }}</li>
            </ul>
          </div>

          <button class="btn btn-primary" :disabled="submitting">
            <span v-if="submitting" class="spinner-border spinner-border-sm me-2"></span>
            Confirm Reservation
          </button>
        </form>
      </div>
    </div>
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
const submitting = ref(false);
const reservationDate = ref("");
const startTime = ref("");
const endTime = ref("");
const serverErrors = ref([]);

const fetchStadium = async () => {
  loading.value = true;
  try {
    const res = await api.get(`/api/StadiumApi/${route.params.id}`);
    stadium.value = res.data;
  } catch (err) {
    console.error("Error fetching stadium:", err);
  } finally {
    loading.value = false;
  }
};

const submitReservation = async () => {
  serverErrors.value = [];

  if (!auth.token) {
    alert("Please login first.");
    router.push({ name: "Login" });
    return;
  }

  if (!reservationDate.value || !startTime.value || !endTime.value) {
    serverErrors.value.push("Please fill all fields.");
    return;
  }
  if (startTime.value >= endTime.value) {
    serverErrors.value.push("Start time must be before end time.");
    return;
  }


  const payload = {
    stadiumId: parseInt(route.params.id),
    reservationDate: `${reservationDate.value}T00:00:00`,
    startTime: `${startTime.value}:00`,
    endTime: `${endTime.value}:00`
  };

  submitting.value = true;
  try {
    const res = await api.post("/api/ReservationsApi", payload, {
      headers: { Authorization: `Bearer ${auth.token}` }
    });

    alert("Reservation successful!");
    router.push({ name: "MyReservations" }); 
  } catch (err) {
    console.error("Error making reservation:", err);
    const data = err.response?.data;
    if (data) {
      if (data.errors) {
        const arr = [];
        for (const key in data.errors) {
          arr.push(...data.errors[key]);
        }
        serverErrors.value = arr;
      } else if (data.title || data.message) {
        serverErrors.value = [data.title || data.message];
      } else {
        serverErrors.value = [err.message || "Failed to create reservation."];
      }
    } else {
      serverErrors.value = [err.message || "Failed to create reservation."];
    }
  } finally {
    submitting.value = false;
  }
};

onMounted(fetchStadium);
</script>

<style scoped>
img { max-height: 200px; object-fit: cover; }
</style>
