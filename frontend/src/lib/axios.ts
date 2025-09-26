import axios, {
  type AxiosInstance,
  type AxiosRequestHeaders,
  type InternalAxiosRequestConfig,
  type RawAxiosRequestHeaders,
} from "axios";
import { useUserStore } from "@/stores/user.store";

const CONTENT_TYPE = "application/json";
const ACCEPT = "application/json";

// Crear instancia de axios
const api: AxiosInstance = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL || "http://localhost:5260/api",
});

// Add request interceptor
api.interceptors.request.use((config: InternalAxiosRequestConfig) => {
  // Obtener el token del store
  const accessToken = useUserStore.getState().accessToken;

  if (accessToken) {
    config.headers = {
      ...(config.headers as RawAxiosRequestHeaders),
      Authorization: `Bearer ${accessToken}`,
    } as AxiosRequestHeaders;
  }

  return config;
});

api.interceptors.response.use(
  (response) => {
    return response.data; // Return the data directly
  },
  async (error) => {
    const clearAuth = useUserStore.getState().clearAuth;

    if (error.response?.status === 401) {
      // delete stored token if it fails
      clearAuth();
      if (typeof window !== "undefined") {
        window.location.href = "/login";
      }
    }
    return Promise.reject(error?.response?.data || error);
  }
);

// Set default headers
api.defaults.headers.post["Content-Type"] = CONTENT_TYPE;
api.defaults.headers.common["Accept"] = ACCEPT;

export { api };
