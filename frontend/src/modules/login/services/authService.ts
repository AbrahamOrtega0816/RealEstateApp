import { useApiMutation } from "@/hooks/useApi";
import {
  LoginDto,
  AuthResponseDto,
  RegisterDto,
  RefreshTokenDto,
} from "@/modules/login/types/auth";
import { useUserStore } from "@/stores/user.store";

// Hook para la mutation de login
export const useLoginMutation = () => {
  const setAuth = useUserStore((state) => state.setAuth);

  return useApiMutation<AuthResponseDto, LoginDto>("/auth/login", "POST", {
    onSuccess: (data) => {
      // Guardar la información de autenticación en el store
      setAuth({
        user: data.user,
        accessToken: data.accessToken,
        refreshToken: data.refreshToken,
        tokenExpiresAt: data.expiresAt,
      });
    },
    onError: (error) => {
      console.error("Error en login:", error.message);
    },
  });
};

// Hook para la mutation de registro
export const useRegisterMutation = () => {
  const setAuth = useUserStore((state) => state.setAuth);

  return useApiMutation<AuthResponseDto, RegisterDto>(
    "/auth/register",
    "POST",
    {
      onSuccess: (data) => {
        // Guardar la información de autenticación en el store
        setAuth({
          user: data.user,
          accessToken: data.accessToken,
          refreshToken: data.refreshToken,
          tokenExpiresAt: data.expiresAt,
        });
      },
      onError: (error) => {
        console.error("Error en registro:", error.message);
      },
    }
  );
};

// Hook para la mutation de refresh token
export const useRefreshTokenMutation = () => {
  const setAuth = useUserStore((state) => state.setAuth);
  const clearAuth = useUserStore((state) => state.clearAuth);

  return useApiMutation<AuthResponseDto, RefreshTokenDto>(
    "/auth/refresh",
    "POST",
    {
      onSuccess: (data) => {
        // Actualizar la información de autenticación en el store
        setAuth({
          user: data.user,
          accessToken: data.accessToken,
          refreshToken: data.refreshToken,
          tokenExpiresAt: data.expiresAt,
        });
      },
      onError: (error) => {
        console.error("Error al renovar token:", error.message);
        // Si falla el refresh, limpiar el store
        clearAuth();
      },
    }
  );
};

// Función para hacer logout
export const logout = () => {
  const clearAuth = useUserStore.getState().clearAuth;
  clearAuth();
};

// Función para obtener el token del store
export const getAccessToken = (): string | null => {
  return useUserStore.getState().accessToken;
};

// Función para obtener el refresh token del store
export const getRefreshToken = (): string | null => {
  return useUserStore.getState().refreshToken;
};

// Función para obtener el usuario del store
export const getUser = () => {
  return useUserStore.getState().user;
};

// Función para verificar si el token está expirado
export const isTokenExpired = (): boolean => {
  return useUserStore.getState().isTokenExpired();
};
