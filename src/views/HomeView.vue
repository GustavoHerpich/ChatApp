<template>
  <v-container
    class="fill-height d-flex flex-column align-center justify-center"
  >
    <v-card class="elevation-12" width="400">
      <v-toolbar color="primary" dark flat>
        <v-toolbar-title>Usuários Online</v-toolbar-title>
      </v-toolbar>
      <v-card-text>
        <v-list>
          <v-list-item-group v-if="onlineUsers.length">
            <v-list-item v-for="user in onlineUsers" :key="user">
              <v-list-item-content>
                <v-list-item-title>{{ user }}</v-list-item-title>
              </v-list-item-content>
              <v-list-item-action>
                <v-btn @click="startChat(user)">Iniciar Conversa</v-btn>
              </v-list-item-action>
            </v-list-item>
          </v-list-item-group>
          <v-list-item v-else>
            <v-list-item-content>
              <v-list-item-title>Nenhum usuário online</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </v-list>
      </v-card-text>
    </v-card>
  </v-container>
</template>

<script lang="ts">
import { defineComponent, ref, onMounted } from "vue";
import { HubConnectionBuilder } from "@aspnet/signalr";

export default defineComponent({
  setup() {
    const onlineUsers = ref<string[]>([]);
    const connection = ref<any>(null);

    onMounted(async () => {
      const token = localStorage.getItem("token");
      console.log(token);
      if (token) {
        connection.value = new HubConnectionBuilder()
          .withUrl("http://localhost:8080/chathub", {
            accessTokenFactory: () => token,
          })
          .build();

        await connection.value.start();
        connection.value.on("OnlineUsers", (users: string[]) => {
          onlineUsers.value = users;
        });

        await connection.value.invoke("GetOnlineUsers");
      } else {
        console.error("Token de autenticação não encontrado no localStorage.");
      }
    });

    function startChat(user: string) {
      console.log(`Iniciar conversa com ${user}`);
    }

    return {
      onlineUsers,
      startChat,
    };
  },
});
</script>

<style scoped>
.fill-height {
  height: 100vh;
}
</style>
