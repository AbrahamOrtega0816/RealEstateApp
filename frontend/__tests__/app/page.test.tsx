import React from "react";
import { render, screen } from "@testing-library/react";
import { useRouter } from "next/navigation";
import Home from "@/app/page";
import * as userStore from "@/stores/user.store";

// Mock del router de Next.js
jest.mock("next/navigation");
const mockRouter = {
  push: jest.fn(),
  replace: jest.fn(),
  prefetch: jest.fn(),
  back: jest.fn(),
  forward: jest.fn(),
  refresh: jest.fn(),
};

// Mock del store de usuario
jest.mock("@/stores/user.store", () => ({
  useIsAuthenticated: jest.fn(),
  useAccessToken: jest.fn(),
  useUserStore: jest.fn(),
}));

const mockUseRouter = useRouter as jest.MockedFunction<typeof useRouter>;
const mockUseIsAuthenticated =
  userStore.useIsAuthenticated as jest.MockedFunction<
    typeof userStore.useIsAuthenticated
  >;
const mockUseAccessToken = userStore.useAccessToken as jest.MockedFunction<
  typeof userStore.useAccessToken
>;
const mockUseUserStore = userStore.useUserStore as jest.MockedFunction<
  typeof userStore.useUserStore
>;

describe("Home Page", () => {
  beforeEach(() => {
    mockUseRouter.mockReturnValue(mockRouter);
    jest.clearAllMocks();
  });

  it("should render loading spinner and text", () => {
    // Mock de usuario no autenticado
    mockUseIsAuthenticated.mockReturnValue(false);
    mockUseAccessToken.mockReturnValue(null);
    mockUseUserStore.mockReturnValue(jest.fn(() => false));

    render(<Home />);

    // Verificar que se muestra el loading spinner
    expect(screen.getByText("Checking authentication...")).toBeInTheDocument();
    expect(screen.getByRole("status")).toBeInTheDocument(); // loading spinner
  });

  it("should redirect to login when user is not authenticated", () => {
    // Mock de usuario no autenticado
    mockUseIsAuthenticated.mockReturnValue(false);
    mockUseAccessToken.mockReturnValue(null);
    mockUseUserStore.mockReturnValue(jest.fn(() => false));

    render(<Home />);

    // Verificar que se redirige al login
    expect(mockRouter.push).toHaveBeenCalledWith("/login");
  });

  it("should redirect to login when token is expired", () => {
    // Mock de usuario autenticado pero token expirado
    mockUseIsAuthenticated.mockReturnValue(true);
    mockUseAccessToken.mockReturnValue("expired-token");
    mockUseUserStore.mockReturnValue(jest.fn(() => true)); // isTokenExpired returns true

    render(<Home />);

    // Verificar que se redirige al login cuando el token está expirado
    expect(mockRouter.push).toHaveBeenCalledWith("/login");
  });

  it("should redirect to properties when user is authenticated with valid token", () => {
    // Mock de usuario autenticado con token válido
    mockUseIsAuthenticated.mockReturnValue(true);
    mockUseAccessToken.mockReturnValue("valid-token");
    mockUseUserStore.mockReturnValue(jest.fn(() => false)); // isTokenExpired returns false

    render(<Home />);

    // Verificar que se redirige a properties
    expect(mockRouter.push).toHaveBeenCalledWith("/properties");
  });

  it("should have correct CSS classes for styling", () => {
    mockUseIsAuthenticated.mockReturnValue(false);
    mockUseAccessToken.mockReturnValue(null);
    mockUseUserStore.mockReturnValue(jest.fn(() => false));

    render(<Home />);

    // Verificar las clases CSS principales
    const container = screen
      .getByText("Checking authentication...")
      .closest("div");
    expect(container?.parentElement).toHaveClass(
      "min-h-screen",
      "flex",
      "items-center",
      "justify-center",
      "bg-base-200"
    );
  });
});
