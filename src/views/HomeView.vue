<template>
  <div>Bem-vindo, {{ userName }}</div>
  <v-container
    class="fill-height d-flex flex-column align-center justify-center"
  >
    <v-card class="elevation-12" width="450">
      <v-toolbar color="primary" dark flat>
        <v-toolbar-title>Usuários Online</v-toolbar-title>
        <v-spacer></v-spacer>
        <v-btn icon @click="logout">
          <v-icon>mdi-logout</v-icon>
        </v-btn>
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
                <v-list-item-title>
                  {{ user }}
                  <span v-if="unreadMessages[user]" class="unread-badge">
                    {{ unreadMessages[user] }}
                  </span>
                </v-list-item-title>
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
    <v-card class="elevation-12" width="450">
      <v-toolbar color="primary" dark flat>
        <v-toolbar-title>Grupos</v-toolbar-title>
      </v-toolbar>
      <v-card-text>
        <v-list>
          <v-list-item-group v-if="userGroups.length">
            <v-list-item
              v-for="group in userGroups"
              :key="group"
              @click="openGroupChat(group)"
            >
              <v-list-item-content>
                <v-list-item-title>
                  {{ group }}
                  <span v-if="unreadMessages[group]" class="unread-badge">
                    {{ unreadMessages[group] }}
                  </span>
                </v-list-item-title>
              </v-list-item-content>
            </v-list-item>
          </v-list-item-group>
        </v-list>
      </v-card-text>
    </v-card>
    <v-dialog v-model="showGroupDialog" persistent max-width="500">
      <v-card>
        <v-card-title>
          <span class="headline">Criar Grupo</span>
        </v-card-title>
        <v-card-text>
          <v-text-field
            v-model="groupName"
            label="Nome do Grupo"
            required
          ></v-text-field>
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

export default defineComponent({
  setup() {
    const onlineUsers = ref<string[]>([]);
    const selectedUsers = ref<string[]>([]);
    const groupName = ref("");
    const showGroupDialog = ref(false);
    const connection = ref<any>(null);
    const router = useRouter();
    const userStore = useUsers();
    const unreadMessages = ref<{ [key: string]: number }>({});
    const userGroups = ref<string[]>([]);
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
          onlineUsers.value = users.filter((user) => user !== userName);
        });

        connection.value.on("GroupCreated", (groups: any[]) => {
          groups.forEach((group: any) => {
            if (!userGroups.value.includes(group.groupName)) {
              userGroups.value.push(group.groupName);
            }
          });
        });

        connection.value.on("NewConversationNotification", (sender: string) => {
          if (!unreadMessages.value[sender]) {
            unreadMessages.value[sender] = 0;
          }
          unreadMessages.value[sender]++;
          console.log(
            "Estado atual de unreadMessages após incremento:",
            unreadMessages.value
          );
        });

        connection.value.on(
          "NewGroupMessageNotification",
          (groupName: string) => {
            if (!unreadMessages.value[groupName]) {
              unreadMessages.value[groupName] = 0;
            }
            unreadMessages.value[groupName]++;
            console.log(
              "Estado atual de unreadMessages após incremento do grupo:",
              unreadMessages.value
            );
          }
        );
        await connection.value.invoke("GetGroups");
        await connection.value.invoke("GetOnlineUsers");
      } else {
        console.error("Token de autenticação não encontrado no localStorage.");
      }
    });

    const startPrivateChat = (user: string) => {
      unreadMessages.value[user] = 0;
      router.push({ name: "privateChat", params: { user } });
    };

    const logout = async () => {
      try {
        if (connection.value) {
          await connection.value.stop();
        }
        userStore.logout();
        await router.push({ name: "login" });
      } catch (error) {
        console.error("Erro ao deslogar:", error);
      }
    };

    const openGroupChat = (groupName: string) => {
      unreadMessages.value[groupName] = 0;
      router.push({
        name: "groupChat",
        query: {
          groupName,
        },
      });
    };

    const openGroupCreation = () => {
      showGroupDialog.value = true;
    };

    const closeGroupDialog = () => {
      showGroupDialog.value = false;
      selectedUsers.value = [];
      groupName.value = "";
    };

    const createGroup = async () => {
      if (groupName.value.trim()) {
        const members = [...selectedUsers.value, userName];
        try {
          if (members.length > 1) {
            await connection.value.invoke(
              "CreateGroup",
              groupName.value,
              members
            );
            await connection.value.invoke("GetGroups");
            router.push({
              name: "groupChat",
              query: {
                users: members.join(","),
                groupName: groupName.value,
              },
            });
            closeGroupDialog();
          } else {
            alert("O grupo deve ter pelo menos dois membros.");
          }
        } catch (error) {
          console.error("Erro ao criar grupo:", error);
          alert("Falha ao criar o grupo.");
        }
      } else {
        alert("Insira um nome para o grupo.");
      }
    };

    return {
      onlineUsers,
      userName,
      startPrivateChat,
      showGroupDialog,
      selectedUsers,
      groupName,
      openGroupCreation,
      closeGroupDialog,
      createGroup,
      logout,
      openGroupChat,
      userGroups,
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
.unread-badge {
  color: white;
  background-color: #3091e0;
  padding: 0 5px;
  border-radius: 12px;
  font-size: 12px;
  margin-left: 5px;
}
</style>
