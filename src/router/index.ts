import { createRouter, createWebHistory } from "vue-router";
import Login from "@/views/LoginView.vue";
import Register from "@/views/RegisterView.vue";
import Home from "@/views/HomeView.vue";
import RecoverPassword from "@/views/RecoverPasswordView.vue";

const routes = [
  {
    path: "/login",
    name: "login",
    component: Login,
  },
  {
    path: "/home",
    name: "home",
    component: Home,
  },
  {
    path: "/register",
    name: "register",
    component: Register,
  },
  {
    path: "/recover-password",
    name: "recoverPassword",
    component: RecoverPassword,
  },
  {
    path: "/",
    redirect: "/login",
  },
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
});

export default router;
