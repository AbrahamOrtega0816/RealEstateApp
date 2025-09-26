import React from "react";
import { render, screen } from "@testing-library/react";
import { useRouter } from "next/navigation";
import ProtectedRoute from "@/components/ProtectedRoute";
import * as useAuth from "@/modules/login/hooks/useAuth";
import * as userStore from "@/stores/user.store";

// Mock del router de Next.js
jest.mock("next/navigation");
const mockRouter = {
  push: jest.fn(),
  replace: jest.fn(),
  prefetch: jest.fn(),
};

// Mock del hook useAuth
jest.mock("@/modules/login/hooks/useAuth");

// Mock del store de usuario
jest.mock("@/stores/user.store", () => ({
  useIsHydrated: jest.fn(),
}));

const mockUseRouter = useRouter as jest.MockedFunction<typeof useRouter>;
const mockUseAuth = useAuth.useAuth as jest.MockedFunction<
  typeof useAuth.useAuth
>;
const mockUseIsHydrated = userStore.useIsHydrated as jest.MockedFunction<
  typeof userStore.useIsHydrated
>;

describe("ProtectedRoute", () => {
  beforeEach(() => {
    mockUseRouter.mockReturnValue(mockRouter);
    jest.clearAllMocks();
  });

  it("should show loading fallback when not hydrated", () => {
    mockUseIsHydrated.mockReturnValue(false);
    mockUseAuth.mockReturnValue({
      isAuthenticated: true,
      user: { id: "1", name: "Test User" },
      accessToken: "token",
    } as any);

    render(
      <ProtectedRoute>
        <div>Protected Content</div>
      </ProtectedRoute>
    );

    expect(screen.getByText("Loading...")).toBeInTheDocument();
    expect(screen.queryByText("Protected Content")).not.toBeInTheDocument();
  });

  it("should show loading fallback when user is not authenticated", () => {
    mockUseIsHydrated.mockReturnValue(true);
    mockUseAuth.mockReturnValue({
      isAuthenticated: false,
      user: null,
      accessToken: null,
    } as any);

    render(
      <ProtectedRoute>
        <div>Protected Content</div>
      </ProtectedRoute>
    );

    expect(screen.getByText("Loading...")).toBeInTheDocument();
    expect(screen.queryByText("Protected Content")).not.toBeInTheDocument();
  });

  it("should redirect to login when user is not authenticated", () => {
    mockUseIsHydrated.mockReturnValue(true);
    mockUseAuth.mockReturnValue({
      isAuthenticated: false,
      user: null,
      accessToken: null,
    } as any);

    render(
      <ProtectedRoute>
        <div>Protected Content</div>
      </ProtectedRoute>
    );

    expect(mockRouter.replace).toHaveBeenCalledWith("/login");
  });

  it("should redirect to login when user is missing", () => {
    mockUseIsHydrated.mockReturnValue(true);
    mockUseAuth.mockReturnValue({
      isAuthenticated: true,
      user: null,
      accessToken: "token",
    } as any);

    render(
      <ProtectedRoute>
        <div>Protected Content</div>
      </ProtectedRoute>
    );

    expect(mockRouter.replace).toHaveBeenCalledWith("/login");
  });

  it("should redirect to login when access token is missing", () => {
    mockUseIsHydrated.mockReturnValue(true);
    mockUseAuth.mockReturnValue({
      isAuthenticated: true,
      user: { id: "1", name: "Test User" },
      accessToken: null,
    } as any);

    render(
      <ProtectedRoute>
        <div>Protected Content</div>
      </ProtectedRoute>
    );

    expect(mockRouter.replace).toHaveBeenCalledWith("/login");
  });

  it("should render children when user is authenticated", () => {
    mockUseIsHydrated.mockReturnValue(true);
    mockUseAuth.mockReturnValue({
      isAuthenticated: true,
      user: { id: "1", name: "Test User" },
      accessToken: "valid-token",
    } as any);

    render(
      <ProtectedRoute>
        <div>Protected Content</div>
      </ProtectedRoute>
    );

    expect(screen.getByText("Protected Content")).toBeInTheDocument();
    expect(screen.queryByText("Loading...")).not.toBeInTheDocument();
  });

  it("should render custom fallback when provided", () => {
    mockUseIsHydrated.mockReturnValue(false);
    mockUseAuth.mockReturnValue({
      isAuthenticated: false,
      user: null,
      accessToken: null,
    } as any);

    const customFallback = <div>Custom Loading...</div>;

    render(
      <ProtectedRoute fallback={customFallback}>
        <div>Protected Content</div>
      </ProtectedRoute>
    );

    expect(screen.getByText("Custom Loading...")).toBeInTheDocument();
    expect(screen.queryByText("Loading...")).not.toBeInTheDocument();
  });
});
