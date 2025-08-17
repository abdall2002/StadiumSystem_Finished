<template>
  <div class="container mt-4">
    <h2>📋 My Reservations</h2>

    <div v-if="loading" class="alert alert-info">Loading reservations...</div>
    <div v-if="error" class="alert alert-danger">{{ error }}</div>

    <table v-if="!loading && reservations.length" class="table table-striped table-hover">
      <thead>
        <tr>
          <th>Stadium</th>
          <th>Date</th>
          <th>Time</th>
          <th>Status</th>
          <th>ِAction</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="res in reservations" :key="res.id">
          <td>{{ res.stadiumName }}</td>
          <td>{{ new Date(res.reservationDate).toLocaleDateString() }}</td>
          <td>{{ formatTime(res.startTime) }} - {{ formatTime(res.endTime) }}</td>
          <td>
            <span
              :class="{
                'badge bg-warning text-dark': res.status === 'Pending',
                'badge bg-success': res.status === 'Accepted',
                'badge bg-danger': res.status === 'Rejected',
              }"
            >
              {{ res.status }}
            </span>
          </td>
          <td>
            <button 
              v-if="res.status === 'Pending'" 
              class="btn btn-sm btn-danger"
              @click="cancelReservation(res.id)"
            >
              Cancel
            </button>
          </td>
        </tr>
      </tbody>
    </table>

    <div v-if="!loading && !reservations.length" class="alert alert-secondary">
      No reservations found.
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import axios from "axios";
import { useAuthStore } from "@/store/authstore";

const reservations = ref([]);
const loading = ref(true);
const error = ref("");
const auth = useAuthStore();

const fetchMyReservations = async () => {
  try {
    const res = await axios.get(
      "https://localhost:7279/api/ReservationsApi/MyReservations",
      {
        headers: {
          Authorization: `Bearer ${auth.token}`,
        },
      }
    );
    reservations.value = res.data;
  } catch (err) {
    error.value = "Failed to load reservations";
    console.error(err);
  } finally {
    loading.value = false;
  }
};
const cancelReservation = async (id) => {
  if (!confirm("Are you sure you want to cancel this reservation?")) return;

  try {
    await axios.put(
      `https://localhost:7050/api/ReservationsApi/Cancel/${id}`,
      {},
      {
        headers: {
          Authorization: `Bearer ${auth.token}`,
        },
      }
    );
    const reservation = reservations.value.find(r => r.id === id);
    if (reservation) reservation.status = "Cancelled";
  } catch (err) {
    alert("Failed to cancel reservation");
    console.error(err);
  }
};
const formatTime = (timeString) => {
  if (!timeString) return "";
  return timeString.substring(0, 5); 
};

const listenForStatusChanges = () => {
  setInterval(fetchMyReservations, 5000);
};

onMounted(() => {
  fetchMyReservations();
  listenForStatusChanges();
});
</script>
