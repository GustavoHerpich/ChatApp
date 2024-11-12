import axios, { AxiosInstance } from "axios";

const _chat: AxiosInstance = axios.create({
  baseURL: process.env.VUE_APP_CHAT_URI,
  withCredentials: true,
});

_chat.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers["Authorization"] = token;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export { _chat };
