import { createRouter, createWebHistory } from 'vue-router';
import StadiumsView from '../Views/StadiumsView.vue';
import StadiumDetailsView from '../Views/StadiumDetailsView.vue';
import LoginView from '../Views/LoginView.vue';
import RegisterView from '../Views/RegisterView .vue';
import AdminCheck from '@/Views/AdminCheck.vue';
import ManageStadiums from '@/Views/ManageStadiums.vue';
import { useAuthStore } from "@/store/authstore";
import ReserveStadium from "../Views/ReserveStadium.vue";
import MyReservations from '@/Views/MyReservations.vue'; // اختياري
import AllReservations from '@/Views/AllReservations.vue';

const routes = [
  {
    path: '/',
    name: 'Stadiums',
    component: StadiumsView
  },
  { 
    path: '/stadiums/:id', 
    name: 'StadiumDetails', 
    component: StadiumDetailsView 
  },
  { 
    path: '/login', 
    component: LoginView 
  },
  { 
    path: '/register', 
    component: RegisterView 
  },
  {
    path: '/AdminCheck', 
    component: AdminCheck 
  },
  {
  path: '/manage-stadiums',
  name: 'ManageStadiums',
  component: ManageStadiums
  },
  {
    path: "/reserve/:id",
    name: "ReserveStadium",
    component: ReserveStadium
  },
  { 
    name: 'MyReservations', 
    path: '/my-reservations', 
    component: MyReservations 
  }, // optional
{ 
    name: 'AllReservations', 
    path: '/All-reservations', 
    component: AllReservations 
  } 

];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach((to, from, next) => {
  const auth = useAuthStore()
  if (to.meta.requiresAdmin && auth.role !== 'Admin') {
    next('/unauthorized')
  } else {
    next()
  }
})
export default router;
