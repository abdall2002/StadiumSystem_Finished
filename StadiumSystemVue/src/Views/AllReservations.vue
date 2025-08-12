<template>
  <div class="container mt-4">
    <h2 class="mb-4">📋 All Reservations</h2>

    <div v-if="loading" class="alert alert-info">Loading...</div>
    <div v-else-if="error" class="alert alert-danger">{{ error }}</div>

    <table v-else class="table table-bordered table-striped">
      <thead>
        <tr>
          <th>Stadium</th>
          <th>User Name</th>
          <th>Reservation Date</th>
          <th>Time</th>
          <th>Status</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="res in reservations" :key="res.id">
          <td>{{ res.stadiumName }}</td>
          <td>{{ res.userName }}</td>
          <td>{{ formatDate(res.reservationDate) }}</td>
          <td>{{ res.startTime }} - {{ res.endTime }}</td>
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
              class="btn btn-success btn-sm me-2"
              @click="updateStatus(res.id, 'Accepted')"
              :disabled="res.status !== 'Pending'"
            >
              ✅ Accept
            </button>
            <button
              class="btn btn-danger btn-sm"
              @click="updateStatus(res.id, 'Rejected')"
              :disabled="res.status !== 'Pending'"
            >
              ❌ Reject
            </button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
import axios from "axios";
import { useAuthStore } from "@/store/auth";

export default {
  name: "AdminReservations",
  data() {
    return {
      reservations: [],
      loading: true,
      error: null,
    };
  },
  methods: {
    async fetchReservations() {
      try {
        const authStore = useAuthStore();
        const res = await axios.get(
          "https://localhost:7279/api/ReservationsApi/AllReservations",
          {
            headers: {
              Authorization: `Bearer ${authStore.token}`,
            },
          }
        );
        this.reservations = res.data;
      } catch (err) {
        this.error = "Failed to load reservations";
        console.error("Error fetching reservations:", err);
      } finally {
        this.loading = false;
      }
    },
    async updateStatus(id, status) {
      try {
        const authStore = useAuthStore();
        await axios.put(
          `https://localhost:7279/api/ReservationsApi/UpdateStatus/${id}`,
          { status },
          {
            headers: {
              Authorization: `Bearer ${authStore.token}`,
            },
          }
        );
        const res = this.reservations.find((r) => r.id === id);
        if (res) res.status = status;
      } catch (err) {
        console.error("Error updating status:", err);
      }
    },
    formatDate(dateStr) {
      return new Date(dateStr).toLocaleDateString("en-US");
    },
  },
  mounted() {
    this.fetchReservations();
  },
};
</script>

<style>
.container {
  direction: ltr;
  text-align: left;
}
</style>
