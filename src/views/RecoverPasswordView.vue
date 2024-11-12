<template>
  <v-container class="fill-height d-flex align-center justify-center" fluid>
    <v-card class="elevation-12" width="400">
      <v-toolbar color="primary" dark flat>
        <v-toolbar-title>Recuperar Senha</v-toolbar-title>
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
            v-model="newPassword"
            :type="showPassword ? 'text' : 'password'"
            :append-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
            @click:append="showPassword = !showPassword"
            :rules="[rules.required]"
            label="Nova Senha"
            prepend-icon="mdi-lock"
            outlined
            required
          ></v-text-field>

          <v-alert
            v-if="showRecoverError"
            type="error"
            class="mt-4"
            dismissible
          >
            {{ extractAxiosErrorMessage(recoverReq.errorObj) }}
          </v-alert>
        </v-form>
      </v-card-text>

      <v-card-actions>
        <v-btn
          :disabled="!validForm"
          color="primary"
          block
          @click="recoverPassword"
        >
          Recuperar Senha
        </v-btn>
      </v-card-actions>

      <v-card-actions class="d-flex flex-column align-center">
        <v-btn variant="text" color="primary" @click="goToLogin">
          Voltar para Login
        </v-btn>
        <v-btn variant="text" color="primary" @click="goToRegister">
          Registrar
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

export default {
  setup() {
    const router = useRouter();

    const validForm = ref(false);
    const username = ref("");
    const newPassword = ref("");
    const showPassword = ref(false);
    const showRecoverError = ref(false);

    const recoverReq = reactive(new Requester<string>(_chat));

    const rules = {
      required: (value: string) => !!value || "Campo obrigatório",
    };

    async function recoverPassword(): Promise<void> {
      showRecoverError.value = false;

      await recoverReq.request({
        method: "post",
        url: "auth/recoverpassword",
        data: { username: username.value, newPassword: newPassword.value },
        onSuccess: () => {
          router.push({ name: "login" });
        },
        onError: () => {
          showRecoverError.value = true;
          clearRecoverFields();
        },
      });
    }

    function clearRecoverFields() {
      username.value = "";
      newPassword.value = "";
    }

    function goToLogin() {
      router.push({ name: "login" });
    }

    function goToRegister() {
      router.push({ name: "register" });
    }

    return {
      validForm,
      username,
      newPassword,
      showPassword,
      showRecoverError,
      rules,
      recoverPassword,
      goToLogin,
      goToRegister,
      extractAxiosErrorMessage,
      recoverReq,
      clearRecoverFields,
    };
  },
};
</script>

<style scoped>
.fill-height {
  height: 100vh;
}
</style>
