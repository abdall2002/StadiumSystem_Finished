import axios from 'axios';
import { useAuthStore } from '@/store/auth'

const api = axios.create({
    baseURL: 'https://localhost:7279',
  withCredentials: true, 
});

api.interceptors.request.use(config => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

export async function getSecureData() {
  const auth = useAuthStore()

  const res = await fetch('https://localhost:5001/api/secure-endpoint', {
    method: 'GET',
    headers: {
      Authorization: `Bearer ${auth.token}`,
      'Content-Type': 'application/json'
    }
  })

  if (!res.ok) throw new Error('Unauthorized or error occurred')

  return await res.json()
}

export default api;
