<template>
  <v-container
    class="fill-height d-flex flex-column align-center justify-center"
  >
    <v-card class="elevation-12" width="500">
      <v-toolbar color="primary" dark flat>
        <v-toolbar-title>{{ groupName }}</v-toolbar-title>
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
                    'message-left': msg.sender !== groupParticipants[0],
                    'message-right': msg.sender === groupParticipants[0],
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
import { defineComponent, ref, onMounted } from "vue";
import { HubConnectionBuilder, HubConnection } from "@aspnet/signalr";
import { useRouter } from "vue-router";

export default defineComponent({
  setup() {
    const message = ref("");
    const messages = ref<{ sender: string; content: string }[]>([]);
    const connection = ref<HubConnection | null>(null);
    const groupName = ref("");
    const groupParticipants = ref<string[]>([]);
    const router = useRouter();

    const route = router.currentRoute.value;
    groupName.value = (route.query.groupName as string) || "Nome do Grupo";
    groupParticipants.value = ((route.query.users as string) || "").split(",");

    const createGroup = async () => {
      const token = localStorage.getItem("token");
      if (token) {
        connection.value = new HubConnectionBuilder()
          .withUrl("http://localhost:8080/chathub", {
            accessTokenFactory: () => token,
          })
          .build();

        await connection.value.start();

        if (connection.value) {
          connection.value.on(
            "ReceiveMessage",
            (sender: string, content: string) => {
              messages.value.push({ sender, content });
            }
          );
        }

        await loadGroupHistory();
      }
    };
    const loadGroupHistory = async () => {
      if (connection.value) {
        const history = await connection.value.invoke(
          "GetMessagesForGroup",
          groupName.value
        );
        messages.value = history;
      }
    };

    const sendMessage = async () => {
      if (message.value.trim() && connection.value) {
        if (groupParticipants.value.length === 1) {
          await loadGroupParticipants();
        }

        await connection.value?.invoke(
          "SendMessageToGroup",
          message.value,
          groupParticipants.value,
          groupName.value
        );
        message.value = "";
      }
    };

    const loadGroupParticipants = async () => {
      if (connection.value) {
        const participants = await connection.value.invoke(
          "GetParticipantsForGroup",
          groupName.value
        );
        groupParticipants.value = participants;
      }
    };

    const goBack = () => {
      router.push({ name: "home" });
    };

    onMounted(() => {
      createGroup();
    });

    return {
      message,
      messages,
      sendMessage,
      groupName,
      groupParticipants,
      loadGroupParticipants,
      goBack,
    };
  },
});
</script>
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
