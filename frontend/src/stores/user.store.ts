import { create } from "zustand";
import { persist, createJSONStorage } from "zustand/middleware";
import { UserDto } from "@/modules/login/types/auth";

interface UserState {
  // Estado
  user: UserDto | null;
  accessToken: string | null;
  refreshToken: string | null;
  tokenExpiresAt: string | null;
  isAuthenticated: boolean;
  isHydrated: boolean;

  // Acciones
  setAuth: (data: {
    user: UserDto;
    accessToken: string;
    refreshToken: string;
    tokenExpiresAt: string;
  }) => void;
  updateUser: (user: UserDto) => void;
  updateTokens: (data: {
    accessToken: string;
    refreshToken: string;
    tokenExpiresAt: string;
  }) => void;
  clearAuth: () => void;

  // Utilidades
  isTokenExpired: () => boolean;
  setHydrated: (value: boolean) => void;
}

export const useUserStore = create<UserState>()(
  persist(
    (set, get) => ({
      // Estado inicial
      user: null,
      accessToken: null,
      refreshToken: null,
      tokenExpiresAt: null,
      isAuthenticated: false,
      isHydrated: false,

      // Acción para establecer toda la información de autenticación
      setAuth: (data) =>
        set({
          user: data.user,
          accessToken: data.accessToken,
          refreshToken: data.refreshToken,
          tokenExpiresAt: data.tokenExpiresAt,
          isAuthenticated: true,
        }),

      // Acción para actualizar solo la información del usuario
      updateUser: (user) =>
        set({
          user,
        }),

      // Acción para actualizar solo los tokens
      updateTokens: (data) =>
        set({
          accessToken: data.accessToken,
          refreshToken: data.refreshToken,
          tokenExpiresAt: data.tokenExpiresAt,
          isAuthenticated: true,
        }),

      // Acción para limpiar toda la información de autenticación
      clearAuth: () =>
        set({
          user: null,
          accessToken: null,
          refreshToken: null,
          tokenExpiresAt: null,
          isAuthenticated: false,
        }),

      // Utilidad para verificar si el token está expirado
      isTokenExpired: () => {
        const { tokenExpiresAt } = get();
        if (!tokenExpiresAt) return true;
        return new Date() >= new Date(tokenExpiresAt);
      },

      // Marcar cuando la persistencia haya rehidratado el estado
      setHydrated: (value: boolean) => set({ isHydrated: value }),
    }),
    {
      name: "user-store", // nombre único para el localStorage
      storage: createJSONStorage(() => localStorage),
      // Solo persistir los campos necesarios
      partialize: (state) => ({
        user: state.user,
        accessToken: state.accessToken,
        refreshToken: state.refreshToken,
        tokenExpiresAt: state.tokenExpiresAt,
        isAuthenticated: state.isAuthenticated,
      }),
      // Controlar el ciclo de hidratación para evitar condiciones de carrera
      onRehydrateStorage: () => (state, error) => {
        // Se llama después de intentar hidratar (exitoso o con error)
        try {
          state?.setHydrated(true);
        } catch (_) {
          // no-op
        }
      },
    }
  )
);

// Selectores para facilitar el uso
export const useUser = () => useUserStore((state) => state.user);
export const useIsAuthenticated = () =>
  useUserStore((state) => state.isAuthenticated);
export const useAccessToken = () => useUserStore((state) => state.accessToken);
export const useRefreshToken = () =>
  useUserStore((state) => state.refreshToken);
export const useIsHydrated = () => useUserStore((state) => state.isHydrated);
