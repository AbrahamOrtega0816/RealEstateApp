import { useRouter } from "next/navigation";
import {
  useLoginMutation,
  useRegisterMutation,
  useRefreshTokenMutation,
  logout,
} from "../services/authService";
import { LoginDto, RegisterDto } from "@/modules/login/types/auth";
import {
  useUser,
  useIsAuthenticated,
  useAccessToken,
  useRefreshToken,
} from "@/stores/user.store";

export const useAuth = () => {
  const router = useRouter();

  // Estado desde el store de Zustand
  const user = useUser();
  const isAuthenticated = useIsAuthenticated();
  const accessToken = useAccessToken();
  const refreshToken = useRefreshToken();

  // Mutations
  const loginMutation = useLoginMutation();
  const registerMutation = useRegisterMutation();
  const refreshTokenMutation = useRefreshTokenMutation();

  // Función de login
  const handleLogin = async (loginData: LoginDto) => {
    try {
      const result = await loginMutation.mutateAsync(loginData);
      // El store se actualiza automáticamente en el onSuccess del mutation
      return result;
    } catch (error) {
      console.error("Error en login:", error);
      throw error;
    }
  };

  // Función de registro
  const handleRegister = async (registerData: RegisterDto) => {
    try {
      const result = await registerMutation.mutateAsync(registerData);
      // El store se actualiza automáticamente en el onSuccess del mutation
      return result;
    } catch (error) {
      console.error("Error en registro:", error);
      throw error;
    }
  };

  // Función de logout
  const handleLogout = () => {
    logout(); // Esto llama a clearAuth() internamente
    router.push("/login");
  };

  // Función para renovar token
  const handleRefreshToken = async () => {
    if (refreshToken) {
      try {
        const result = await refreshTokenMutation.mutateAsync({ refreshToken });
        // El store se actualiza automáticamente en el onSuccess del mutation
        return result;
      } catch (error) {
        handleLogout();
        throw error;
      }
    } else {
      handleLogout();
      throw new Error("No refresh token available");
    }
  };

  return {
    // Estado
    user,
    isAuthenticated,
    accessToken,
    refreshToken: refreshToken,

    // Estados de las mutations
    isLoginLoading: loginMutation.isPending,
    isRegisterLoading: registerMutation.isPending,
    isRefreshLoading: refreshTokenMutation.isPending,

    // Errores
    loginError: loginMutation.error,
    registerError: registerMutation.error,
    refreshError: refreshTokenMutation.error,

    // Funciones
    login: handleLogin,
    register: handleRegister,
    logout: handleLogout,
    refreshTokenFn: handleRefreshToken,

    // Mutations para uso directo si es necesario
    loginMutation,
    registerMutation,
    refreshTokenMutation,
  };
};
