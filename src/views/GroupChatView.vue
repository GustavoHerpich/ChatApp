<template>
  <v-container
    class="fill-height d-flex flex-column align-center justify-center"
  >
    <v-card class="elevation-12" width="500">
      <v-toolbar color="primary" dark flat>
        <v-toolbar-title>Chat em Grupo</v-toolbar-title>
        <v-spacer></v-spacer>
        <v-btn icon @click="goBack">
          <v-icon>mdi-arrow-left</v-icon>
        </v-btn>
      </v-toolbar>
      <v-card-text>
        <v-list>
          <v-list-item v-for="(msg, index) in messages" :key="index">
            <v-list-item-content>
              <v-list-item-title>
                <strong>{{ msg.sender }}:</strong> {{ msg.content }}
              </v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </v-list>
        <v-text-field
          v-model="message"
          label="Digite sua mensagem"
          @keyup.enter="sendMessage"
        ></v-text-field>
        <v-btn color="primary" @click="sendMessage">Enviar</v-btn>
      </v-card-text>
    </v-card>
  </v-container>
</template>

<script lang="ts">
import { defineComponent, ref, onMounted } from "vue";
import { HubConnectionBuilder, HubConnection } from "@aspnet/signalr";
import { useRouter } from "vue-router";

export default defineComponent({
  setup() {
    const message = ref("");
    const messages = ref<{ sender: string; content: string }[]>([]);
    const connection = ref<HubConnection | null>(null);
    const router = useRouter();

    onMounted(async () => {
      const token = localStorage.getItem("token");

      if (token) {
        connection.value = new HubConnectionBuilder()
          .withUrl("http://localhost:8080/chathub", {
            accessTokenFactory: () => token,
          })
          .build();

        await connection.value.start();

        connection.value.on("ReceiveMessage", (sender, content) => {
          messages.value.push({ sender, content });
        });
      }
    });

    const sendMessage = async () => {
      if (message.value.trim() && connection.value) {
        await connection.value.invoke("SendMessage", message.value);
        messages.value.push({ sender: "Você", content: message.value });
        message.value = "";
      }
    };

    const goBack = () => {
      router.push({ name: "Home" });
    };

    return {
      message,
      messages,
      sendMessage,
      goBack,
    };
  },
});
</script>

<style scoped>
.fill-height {
  height: 100vh;
}
</style>
