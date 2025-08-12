<template>
  <div class="container mt-4">
    <h2 class="mb-3">Manage Stadiums</h2>

    <button class="btn btn-primary mb-3" @click="openCreateModal">➕ Add New Stadium</button>

    <table class="table table-striped">
      <thead>
        <tr>
          <th>Image</th>
          <th>Name</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="stadium in stadiums" :key="stadium.id">
          <td>
            <img :src="stadium.imageUrl" alt="Stadium Image" style="width: 120px; height: auto;" />
          </td>
          <td>{{ stadium.name }}</td>
          <td>
            <button class="btn btn-sm btn-warning me-2" @click="openEditModal(stadium)">✏️ Edit</button>
            <button class="btn btn-sm btn-danger" @click="deleteStadium(stadium.id)">🗑️ Delete</button>
          </td>
        </tr>
      </tbody>
    </table>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ isEditing ? 'Edit' : 'Add' }} Stadium</h5>
            <button type="button" class="btn-close" @click="closeModal"></button>
          </div>
          <div class="modal-body">
            <input v-model="form.name" placeholder="Name" class="form-control mb-2" />
            <input v-model="form.imageUrl" placeholder="Image URL" class="form-control mb-2" />
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="closeModal">Cancel</button>
            <button class="btn btn-success" @click="saveStadium">{{ isEditing ? 'Update' : 'Create' }}</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import { useAuthStore } from '@/store/authstore'
import { useRouter } from 'vue-router'

const stadiums = ref([])
const showModal = ref(false)
const isEditing = ref(false)
const form = ref({ id: 0, name: '', imageUrl: '' })

const auth = useAuthStore()
const router = useRouter()

onMounted(() => {
  if (!auth.isAuthenticated || auth.role !== 'Admin') {
    router.push('/')
    return
  }
  fetchStadiums()
})

const fetchStadiums = async () => {
  const res = await axios.get('https://localhost:7279/api/StadiumsApi', {
    headers: {
      Authorization: `Bearer ${auth.token}`
    }
  })
  stadiums.value = res.data
}

const openCreateModal = () => {
  isEditing.value = false
  form.value = { id: 0, name: '', imageUrl: '' }
  showModal.value = true
}

const openEditModal = (stadium) => {
  isEditing.value = true
  form.value = { ...stadium }
  showModal.value = true
}

const closeModal = () => {
  showModal.value = false
}

const saveStadium = async () => {
  debugger
  const token = localStorage.getItem('token')
  if (isEditing.value) {
    await axios.put(`https://localhost:7279/api/StadiumsApi/${form.value.id}`, form.value, {
      headers: { Authorization: `Bearer ${token}` }
    })
  } else {
    debugger
    console.log("Token:",token)
  try {
    await axios.post('https://localhost:7279/api/StadiumsApi', form.value, {
      headers: { Authorization: `Bearer ${token}` }
    })
    closeModal()
    fetchStadiums()
  } catch (err) {
    console.error("Error saving stadium:", err.response?.status, err.response?.data)
  }
  }

  closeModal()
  fetchStadiums()
}

const deleteStadium = async (id) => {
  if (!confirm('Are you sure?')) return
  await axios.delete(`https://localhost:7279/api/StadiumsApi/${id}`, {
    headers: { Authorization: `Bearer ${auth.token}` }
  })
  fetchStadiums()
}
</script>