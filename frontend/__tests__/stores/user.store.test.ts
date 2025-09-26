import { renderHook, act } from "@testing-library/react";
import {
  useUserStore,
  useUser,
  useIsAuthenticated,
  useAccessToken,
} from "@/stores/user.store";

// Mock localStorage
const localStorageMock = {
  getItem: jest.fn(),
  setItem: jest.fn(),
  removeItem: jest.fn(),
  clear: jest.fn(),
};
Object.defineProperty(window, "localStorage", {
  value: localStorageMock,
});

describe("User Store", () => {
  beforeEach(() => {
    // Reset the store state before each test
    useUserStore.getState().clearAuth();
    useUserStore.getState().setHydrated(false);
    jest.clearAllMocks();
  });

  describe("Initial State", () => {
    it("should have correct initial state", () => {
      const { result } = renderHook(() => useUserStore());
      const state = result.current;

      expect(state.user).toBeNull();
      expect(state.accessToken).toBeNull();
      expect(state.refreshToken).toBeNull();
      expect(state.tokenExpiresAt).toBeNull();
      expect(state.isAuthenticated).toBe(false);
      expect(state.isHydrated).toBe(false);
    });
  });

  describe("Authentication Actions", () => {
    it("should set authentication data correctly", () => {
      const { result } = renderHook(() => useUserStore());

      const authData = {
        user: { id: "1", name: "Test User", email: "test@example.com" },
        accessToken: "access-token",
        refreshToken: "refresh-token",
        tokenExpiresAt: "2025-12-31T23:59:59Z",
      };

      act(() => {
        result.current.setAuth(authData);
      });

      const state = result.current;
      expect(state.user).toEqual(authData.user);
      expect(state.accessToken).toBe(authData.accessToken);
      expect(state.refreshToken).toBe(authData.refreshToken);
      expect(state.tokenExpiresAt).toBe(authData.tokenExpiresAt);
      expect(state.isAuthenticated).toBe(true);
    });

    it("should update user data only", () => {
      const { result } = renderHook(() => useUserStore());

      // First set auth data
      act(() => {
        result.current.setAuth({
          user: { id: "1", name: "Test User", email: "test@example.com" },
          accessToken: "access-token",
          refreshToken: "refresh-token",
          tokenExpiresAt: "2025-12-31T23:59:59Z",
        });
      });

      // Then update user
      const updatedUser = {
        id: "1",
        name: "Updated User",
        email: "updated@example.com",
      };

      act(() => {
        result.current.updateUser(updatedUser);
      });

      const state = result.current;
      expect(state.user).toEqual(updatedUser);
      expect(state.accessToken).toBe("access-token"); // Should remain unchanged
      expect(state.isAuthenticated).toBe(true);
    });

    it("should update tokens only", () => {
      const { result } = renderHook(() => useUserStore());

      // First set auth data
      act(() => {
        result.current.setAuth({
          user: { id: "1", name: "Test User", email: "test@example.com" },
          accessToken: "old-access-token",
          refreshToken: "old-refresh-token",
          tokenExpiresAt: "2025-06-30T23:59:59Z",
        });
      });

      // Then update tokens
      const newTokens = {
        accessToken: "new-access-token",
        refreshToken: "new-refresh-token",
        tokenExpiresAt: "2025-12-31T23:59:59Z",
      };

      act(() => {
        result.current.updateTokens(newTokens);
      });

      const state = result.current;
      expect(state.accessToken).toBe(newTokens.accessToken);
      expect(state.refreshToken).toBe(newTokens.refreshToken);
      expect(state.tokenExpiresAt).toBe(newTokens.tokenExpiresAt);
      expect(state.user?.name).toBe("Test User"); // Should remain unchanged
      expect(state.isAuthenticated).toBe(true);
    });

    it("should clear authentication data", () => {
      const { result } = renderHook(() => useUserStore());

      // First set auth data
      act(() => {
        result.current.setAuth({
          user: { id: "1", name: "Test User", email: "test@example.com" },
          accessToken: "access-token",
          refreshToken: "refresh-token",
          tokenExpiresAt: "2025-12-31T23:59:59Z",
        });
      });

      // Then clear auth
      act(() => {
        result.current.clearAuth();
      });

      const state = result.current;
      expect(state.user).toBeNull();
      expect(state.accessToken).toBeNull();
      expect(state.refreshToken).toBeNull();
      expect(state.tokenExpiresAt).toBeNull();
      expect(state.isAuthenticated).toBe(false);
    });
  });

  describe("Token Expiration", () => {
    it("should detect expired token", () => {
      const { result } = renderHook(() => useUserStore());

      // Set expired token
      const pastDate = new Date();
      pastDate.setHours(pastDate.getHours() - 1); // 1 hour ago

      act(() => {
        result.current.setAuth({
          user: { id: "1", name: "Test User", email: "test@example.com" },
          accessToken: "access-token",
          refreshToken: "refresh-token",
          tokenExpiresAt: pastDate.toISOString(),
        });
      });

      expect(result.current.isTokenExpired()).toBe(true);
    });

    it("should detect valid token", () => {
      const { result } = renderHook(() => useUserStore());

      // Set valid token
      const futureDate = new Date();
      futureDate.setHours(futureDate.getHours() + 1); // 1 hour from now

      act(() => {
        result.current.setAuth({
          user: { id: "1", name: "Test User", email: "test@example.com" },
          accessToken: "access-token",
          refreshToken: "refresh-token",
          tokenExpiresAt: futureDate.toISOString(),
        });
      });

      expect(result.current.isTokenExpired()).toBe(false);
    });

    it("should return true for missing token expiration date", () => {
      const { result } = renderHook(() => useUserStore());

      act(() => {
        result.current.setAuth({
          user: { id: "1", name: "Test User", email: "test@example.com" },
          accessToken: "access-token",
          refreshToken: "refresh-token",
          tokenExpiresAt: "",
        });
      });

      expect(result.current.isTokenExpired()).toBe(true);
    });
  });

  describe("Selectors", () => {
    it("should provide correct selector values", () => {
      const userData = {
        id: "1",
        name: "Test User",
        email: "test@example.com",
      };

      // Set auth data
      act(() => {
        useUserStore.getState().setAuth({
          user: userData,
          accessToken: "access-token",
          refreshToken: "refresh-token",
          tokenExpiresAt: "2025-12-31T23:59:59Z",
        });
      });

      const { result: userResult } = renderHook(() => useUser());
      const { result: authResult } = renderHook(() => useIsAuthenticated());
      const { result: tokenResult } = renderHook(() => useAccessToken());

      expect(userResult.current).toEqual(userData);
      expect(authResult.current).toBe(true);
      expect(tokenResult.current).toBe("access-token");
    });
  });

  describe("Hydration", () => {
    it("should handle hydration state", () => {
      const { result } = renderHook(() => useUserStore());

      expect(result.current.isHydrated).toBe(false);

      act(() => {
        result.current.setHydrated(true);
      });

      expect(result.current.isHydrated).toBe(true);
    });
  });
});
