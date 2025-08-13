<template>
  <div class="notification-bell position-relative">
    <button class="btn btn-outline-light btn-sm" @click="toggleDropdown">
      🔔
      <span
        v-if="count > 0"
        class="badge bg-danger position-absolute top-0 start-100 translate-middle"
      >
        {{ count }}
      </span>
    </button>

    <div v-if="showDropdown" class="dropdown-menu dropdown-menu-end show p-2">
      <div v-if="loading" class="text-center">Loading...</div>
      <div v-else-if="notifications.length === 0" class="text-center">No notifications</div>
      <div v-else>
        <div v-for="notif in notifications" :key="notif.id" class="notif-item p-2 mb-1 border rounded">
          <strong>{{ notif.StadiumName }}</strong>
          <span class="d-block">
            {{ formatDateTime(notif.ReservationDate, notif.StartTime) }} - Status: {{ notif.Status }}
          </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import axios from "axios";
import { useAuthStore } from "@/store/authstore";

const auth = useAuthStore();
const notifications = ref([]);
const count = ref(0);
const loading = ref(true);
const showDropdown = ref(false);

const fetchNotifications = async () => {
  loading.value = true;
  try {
    const endpoint = auth.role === "Admin"
      ? "/api/ReservationsApi/admin/pending-reservations"
      : "/api/ReservationsApi/user/notifications";

    const res = await axios.get(`https://localhost:7279${endpoint}`, {
      headers: { Authorization: `Bearer ${auth.token}` },
    });

    notifications.value = res.data;
    count.value = notifications.value.length;
  } catch (err) {
    console.error("Failed to fetch notifications", err);
  } finally {
    loading.value = false;
  }
};

const toggleDropdown = () => {
  showDropdown.value = !showDropdown.value;
  if (showDropdown.value || auth.role === "Admin") {
    count.value = 0;
  }
};

const formatDateTime = (dateString, timeString) => {
  if (!dateString || !timeString) return "";
  const dt = new Date(`${dateString}T${timeString}`);
  return dt.toLocaleDateString() + " " + dt.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
};

onMounted(() => {
  fetchNotifications();
  setInterval(fetchNotifications, 5000); 
});
</script>

<style scoped>
.notification-bell {
  cursor: pointer;
}

.notif-item {
  background-color: #f8f9fa;
}

.badge {
  font-size: 0.75rem;
}
</style>
