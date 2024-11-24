<template>
  <v-container class="fill-height d-flex align-center justify-center" fluid>
    <v-card class="elevation-12" width="400">
      <v-toolbar color="primary" dark flat>
        <v-toolbar-title>Bem-vindo ao Chat</v-toolbar-title>
      </v-toolbar>
      <v-card-text>
        <v-form v-model="validForm">
          <v-text-field
            v-model="login"
            :rules="[rules.required]"
            label="Usuário"
            prepend-icon="mdi-account"
            outlined
            required
          ></v-text-field>

          <v-text-field
            v-model="password"
            :type="showPassword ? 'text' : 'password'"
            :append-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
            @click:append="showPassword = !showPassword"
            :rules="[rules.required]"
            label="Senha"
            prepend-icon="mdi-lock"
            outlined
            required
          ></v-text-field>

          <v-alert v-if="showLoginError" type="error" class="mt-4" dismissible>
            {{ extractAxiosErrorMessage(loginReq.errorObj) }}
          </v-alert>
        </v-form>
      </v-card-text>

      <v-card-actions>
        <v-btn
          :disabled="!validForm"
          color="primary"
          block
          @click="authenticate(login, password)"
        >
          Entrar
        </v-btn>
      </v-card-actions>

      <v-card-actions class="d-flex flex-column align-center">
        <v-btn variant="text" color="primary" @click="goToRegister">
          Registrar
        </v-btn>
        <v-btn variant="text" color="primary" @click="goToRecoverPassword">
          Esqueci minha senha
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-container>
</template>

<script lang="ts">
import { reactive, ref } from "vue";
import { useRouter } from "vue-router";
import { useUsers } from "@/stores/user";
import { _chat } from "@/plugins/axios";
import Token from "@/models/user/Token";
import Requester from "@/utils/requests/Requester";
import { extractAxiosErrorMessage } from "@/utils/requests/Request";

export default {
  setup() {
    const router = useRouter();
    const userStore = useUsers();

    const validForm = ref(false);
    const login = ref("");
    const password = ref("");
    const showPassword = ref(false);
    const showLoginError = ref(false);

    const loginReq = reactive(new Requester<Token>(_chat));

    const rules = {
      required: (value: string) => !!value || "Campo obrigatório",
    };

    async function authenticate(
      login: string,
      password: string
    ): Promise<void> {
      showLoginError.value = false;

      await loginReq.request({
        method: "post",
        url: "auth/login",
        data: { username: login, password: password },
        onSuccess: () => {
          if (loginReq.response) {
            const token = loginReq.response;
            userStore.saveToken(token);
            router.push({ name: "home" });
          }
        },
        onError: () => {
          showLoginError.value = true;
          clearLoginFields();
        },
      });
    }

    function clearLoginFields() {
      login.value = "";
      password.value = "";
    }

    function goToRegister() {
      router.push({ name: "register" });
    }

    function goToRecoverPassword() {
      router.push({ name: "recoverPassword" });
    }

    return {
      validForm,
      login,
      password,
      showPassword,
      showLoginError,
      rules,
      authenticate,
      goToRegister,
      goToRecoverPassword,
      extractAxiosErrorMessage,
      loginReq,
      clearLoginFields,
    };
  },
};
</script>

<style scoped>
.fill-height {
  height: 100vh;
}
</style>
