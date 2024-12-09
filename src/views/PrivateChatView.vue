<template>
  <v-container
    class="fill-height d-flex flex-column align-center justify-center"
  >
    <v-card class="elevation-12" width="600">
      <v-toolbar color="primary" dark flat>
        <v-toolbar-title>Chat Privado com {{ receiver }}</v-toolbar-title>
        <v-spacer></v-spacer>
        <v-btn icon @click="goBack">
          <v-icon>mdi-arrow-left</v-icon>
        </v-btn>
      </v-toolbar>
      <v-card-text>
        <v-list>
          <v-list-item v-for="(msg, index) in messages" :key="index">
            <v-list-item-content>
              <div
                :class="[
                  'message-container',
                  {
                    'message-left': msg.sender !== receiver,
                    'message-right': msg.sender === receiver,
                  },
                ]"
              >
                <div class="message-content">
                  <strong>{{ msg.sender }}:</strong> {{ msg.content }}
                </div>
              </div>
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
import { defineComponent, ref, onMounted, computed } from "vue";
import { useRoute, useRouter } from "vue-router";
import { HubConnectionBuilder, HubConnection } from "@aspnet/signalr";

export default defineComponent({
  setup() {
    const route = useRoute();
    const router = useRouter();
    const receiver = computed(() => route.params.user as string);
    const message = ref("");
    const messages = ref<{ sender: string; content: string }[]>([]);
    const connection = ref<HubConnection | null>(null);

    onMounted(async () => {
      const token = localStorage.getItem("token");
      if (token) {
        connection.value = new HubConnectionBuilder()
          .withUrl("http://localhost:8080/chathub", {
            accessTokenFactory: () => token,
          })
          .build();

        await connection.value.start();

        connection.value.on("ReceivePrivateMessage", (sender, content) => {
          messages.value.push({ sender, content });
        });

        loadChatHistory();
      }
    });

    const sendMessage = async () => {
      if (message.value.trim() && connection.value) {
        await connection.value.invoke(
          "SendPrivateMessage",
          receiver.value,
          message.value
        );
        message.value = "";
      }
    };

    const loadChatHistory = async () => {
      if (connection.value) {
        const history = await connection.value.invoke(
          "GetMessagesForChat",
          receiver.value
        );
        messages.value = history;
      }
    };

    const goBack = () => {
      router.push({ name: "home" });
    };

    return {
      receiver,
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
<style>
.message-container {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
}

.message-left {
  justify-content: flex-start;
}

.message-right {
  justify-content: flex-end;
}

.message-content {
  max-width: 60%;
  word-wrap: break-word;
  padding: 8px;
  border-radius: 10px;
}

.message-left .message-content {
  background-color: #e0e0e0;
}

.message-right .message-content {
  background-color: #007bff;
  color: white;
}
</style>
