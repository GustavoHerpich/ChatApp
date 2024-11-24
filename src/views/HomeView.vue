<template>
  <div>Bem-vindo, {{ userName }}</div>
  <v-container
    class="fill-height d-flex flex-column align-center justify-center"
  >
    <v-card class="elevation-12" width="400">
      <v-toolbar color="primary" dark flat>
        <v-toolbar-title>Usuários Online</v-toolbar-title>
        <v-spacer></v-spacer>
        <v-btn icon @click="openGroupCreation">
          <v-icon>mdi-account-multiple-plus</v-icon>
        </v-btn>
      </v-toolbar>
      <v-card-text>
        <v-list>
          <v-list-item-group v-if="onlineUsers.length">
            <v-list-item
              v-for="user in onlineUsers"
              :key="user"
              @click="startPrivateChat(user)"
              class="online-user-item"
            >
              <v-list-item-content>
                <v-list-item-title>{{ user }}</v-list-item-title>
                <v-badge
                  v-if="unreadMessages[user]"
                  color="primary"
                  dot
                ></v-badge>
              </v-list-item-content>
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

    <v-dialog v-model="showGroupDialog" persistent max-width="500">
      <v-card>
        <v-card-title>
          <span class="headline">Criar Grupo</span>
        </v-card-title>
        <v-card-text>
          <v-autocomplete
            v-model="selectedUsers"
            :items="onlineUsers"
            label="Adicionar usuários ao grupo"
            multiple
            chips
            clearable
          ></v-autocomplete>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="primary" @click="createGroup">Criar Grupo</v-btn>
          <v-btn color="grey" @click="closeGroupDialog">Cancelar</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script lang="ts">
import { defineComponent, ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import { HubConnectionBuilder } from "@aspnet/signalr";
import { useUsers } from "@/stores/user";
import { notify } from "notiwind";

export default defineComponent({
  setup() {
    const onlineUsers = ref<string[]>([]);
    const selectedUsers = ref<string[]>([]);
    const showGroupDialog = ref(false);
    const connection = ref<any>(null);
    const router = useRouter();
    const userStore = useUsers();
    const unreadMessages = ref<{ [key: string]: number }>({});

    const userName = userStore.getUserName();

    onMounted(async () => {
      const token = localStorage.getItem("token");
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

        connection.value.on(
          "NewConversationNotification",
          (receiver: string | number) => {
            notify({
              title: "Nova Mensagem",
              text: `Você recebeu uma nova mensagem de ${receiver}`,
            });
            if (!unreadMessages.value[receiver]) {
              unreadMessages.value[receiver] = 0;
            }
            unreadMessages.value[receiver]++;
          }
        );

        await connection.value.invoke("GetOnlineUsers");
      } else {
        console.error("Token de autenticação não encontrado no localStorage.");
      }
    });

    const startPrivateChat = (user: string) => {
      unreadMessages.value[user] = 0;
      router.push({ name: "privateChat", params: { user } });
    };

    const openGroupCreation = () => {
      showGroupDialog.value = true;
    };

    const closeGroupDialog = () => {
      showGroupDialog.value = false;
      selectedUsers.value = [];
    };

    const createGroup = () => {
      if (selectedUsers.value.length > 1) {
        router.push({
          name: "groupChat",
          params: { users: selectedUsers.value },
        });
      } else {
        alert("Selecione ao menos dois usuários para criar um grupo.");
      }
      closeGroupDialog();
    };

    return {
      onlineUsers,
      userName,
      startPrivateChat,
      showGroupDialog,
      selectedUsers,
      openGroupCreation,
      closeGroupDialog,
      createGroup,
      unreadMessages,
    };
  },
});
</script>

<style scoped>
.fill-height {
  height: 100vh;
}
.online-user-item {
  cursor: pointer;
  transition: background-color 0.2s ease;
}
.online-user-item:hover {
  background-color: rgba(0, 0, 0, 0.1);
}
</style>
