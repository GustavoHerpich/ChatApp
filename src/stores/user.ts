import { ref } from "vue";
import { defineStore } from "pinia";
import Token from "@/models/user/Token";
import UserAuthentication from "@/models/user/UserAuthentication";

export const useUsers = defineStore("users", () => {
  const user = ref<UserAuthentication | null>(
    JSON.parse(localStorage.getItem("user") || "null")
  );
  const token = ref<string | null>(localStorage.getItem("token"));

  function saveUser(userAuth: UserAuthentication): Promise<void> {
    return new Promise((resolve) => {
      console.log("Salvando usuario:", userAuth);
      user.value = userAuth;
      localStorage.setItem("user", JSON.stringify(userAuth));
      resolve();
    });
  }

  function saveToken(token: Token): void {
    localStorage.setItem("token", token.token);
  }

  function logout(): void {
    user.value = null;
    token.value = null;
    localStorage.removeItem("user");
    localStorage.removeItem("token");
  }

  function getUserName(): string {
    console.log(user.value?.username);
    return user.value?.username || "";
  }

  return { user, token, saveUser, saveToken, logout, getUserName };
});
