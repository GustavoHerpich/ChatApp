import { AxiosError } from "axios";
import { ResponseData } from "@/models/response/ResponseData";

function extractAxiosErrorMessage(axiosError: AxiosError | null): string {
  if (!axiosError) {
    return "";
  }

  if (!axiosError.response) {
    return "Não foi possível recuperar as informações no momento.";
  }

  if ((axiosError.response?.data as ResponseData).message) {
    return (axiosError.response.data as ResponseData).message;
  }

  if (axiosError.response?.data) {
    return axiosError.response.data as string;
  }

  return axiosError.message;
}

export { extractAxiosErrorMessage };
