import { createApp, ref } from "vue";
import App from "./App.vue";
import { createPinia } from "pinia";
import router from "./router/index";
import vuetify from "./plugins/vuetify";
import { _chat } from "./plugins/axios";
import { useUsers } from "./stores/user";

const app = createApp(App);
const pinia = createPinia();

app.config.globalProperties.$axios = _chat;

app.use(vuetify);
app.use(pinia);
app.use(router);

app.mount("#app");

const logoff = ref(false);
const userStore = useUsers();

_chat.interceptors.response.use(
  async (response) => response,
  async (error) => {
    if (!error.response) {
      return Promise.reject(error);
    }

    if (
      userStore.user &&
      (error.response.status === 401 || error.response.status === 403)
    ) {
      logoff.value = true;
      router.push({ name: "login" });
    }

    return Promise.reject(error);
  }
);

export { logoff };
