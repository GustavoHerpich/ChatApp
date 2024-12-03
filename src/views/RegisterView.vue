<template>
  <v-container class="fill-height d-flex align-center justify-center" fluid>
    <v-card class="elevation-12" width="400">
      <v-toolbar color="primary" dark flat>
        <v-toolbar-title>Registrar Novo Usuário</v-toolbar-title>
      </v-toolbar>
      <v-card-text>
        <v-form v-model="validForm">
          <v-text-field
            v-model="username"
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

          <v-alert
            v-if="showRegisterError"
            type="error"
            class="mt-4"
            dismissible
          >
            {{ extractAxiosErrorMessage(registerReq.errorObj) }}
          </v-alert>
        </v-form>
      </v-card-text>

      <v-card-actions>
        <v-btn
          :disabled="!validForm"
          color="primary"
          block
          @click="registerUser"
        >
          Registrar
        </v-btn>
      </v-card-actions>

      <v-card-actions class="d-flex flex-column align-center">
        <v-btn variant="text" color="primary" @click="goToLogin">
          Já tenho uma conta
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
import { _chat } from "@/plugins/axios";
import Requester from "@/utils/requests/Requester";
import { extractAxiosErrorMessage } from "@/utils/requests/Request";
import UserAuthentication from "@/models/user/UserAuthentication";

export default {
  setup() {
    const router = useRouter();

    const validForm = ref(false);
    const username = ref("");
    const password = ref("");
    const showPassword = ref(false);
    const showRegisterError = ref(false);

    const registerReq = reactive(new Requester<UserAuthentication>(_chat));

    const rules = {
      required: (value: string) => !!value || "Campo obrigatório",
    };

    async function registerUser(): Promise<void> {
      showRegisterError.value = false;

      await registerReq.request({
        method: "post",
        url: "auth/register",
        data: {
          username: username.value,
          password: password.value,
        },
        onSuccess: () => {
          if (registerReq.response) {
            router.push({ name: "login" });
          }
        },
        onError: () => {
          showRegisterError.value = true;
          clearRegisterFields();
        },
      });
    }

    function clearRegisterFields() {
      username.value = "";
      password.value = "";
    }

    function goToLogin() {
      router.push({ name: "login" });
    }

    function goToRecoverPassword() {
      router.push({ name: "recoverPassword" });
    }

    return {
      validForm,
      username,
      password,
      showPassword,
      showRegisterError,
      rules,
      registerUser,
      goToLogin,
      goToRecoverPassword,
      extractAxiosErrorMessage,
      registerReq,
      clearRegisterFields,
    };
  },
};
</script>

<style scoped>
.fill-height {
  height: 100vh;
}
</style>
