import { ref } from "vue";
import { defineStore } from "pinia";
import UserAuthentication from "@/models/user/UserAuthentication";
import Token from "@/models/user/Token";

export const useUsers = defineStore("users", () => {
  const user = ref<UserAuthentication | null>(null);

  function logout(): void {
    user.value = null;
    localStorage.removeItem("token");
  }

  function saveUser(token: Token): void {
    localStorage.setItem("token", token.token);
  }

  function getUserName(): string {
    return user.value ? user.value.username : "";
  }

  return {
    user,
    logout,
    saveUser,
    getUserName,
  };
});
