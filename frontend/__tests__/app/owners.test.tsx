import React from "react";
import { render, screen } from "@testing-library/react";
import Owners from "@/app/owners/page";

// Mock del componente ProtectedLayout
jest.mock("@/components", () => ({
  ProtectedLayout: ({ children }: { children: React.ReactNode }) => (
    <div data-testid="protected-layout">
      <div data-testid="navbar">Navbar</div>
      <div data-testid="sidebar">Sidebar</div>
      <main data-testid="main-content">{children}</main>
    </div>
  ),
}));

// Mock del componente OwnerPage del módulo
jest.mock("@/modules/owners/page", () => {
  return function MockOwnerPage() {
    return (
      <div data-testid="owner-page">
        <h1>Owners Management</h1>
        <div data-testid="owners-table">Owners Table</div>
        <button data-testid="add-owner-btn">Add Owner</button>
      </div>
    );
  };
});

describe("Owners Page", () => {
  it("should render within ProtectedLayout", () => {
    render(<Owners />);

    // Verificar que está dentro del ProtectedLayout
    expect(screen.getByTestId("protected-layout")).toBeInTheDocument();
    expect(screen.getByTestId("navbar")).toBeInTheDocument();
    expect(screen.getByTestId("sidebar")).toBeInTheDocument();
    expect(screen.getByTestId("main-content")).toBeInTheDocument();
  });

  it("should render OwnerPage component", () => {
    render(<Owners />);

    expect(screen.getByTestId("owner-page")).toBeInTheDocument();
    expect(screen.getByText("Owners Management")).toBeInTheDocument();
  });

  it("should display owners management elements", () => {
    render(<Owners />);

    expect(screen.getByTestId("owners-table")).toBeInTheDocument();
    expect(screen.getByTestId("add-owner-btn")).toBeInTheDocument();
  });

  it("should be a protected route", () => {
    render(<Owners />);

    // Verificar que está envuelto en ProtectedLayout
    const protectedLayout = screen.getByTestId("protected-layout");
    const ownerPage = screen.getByTestId("owner-page");

    expect(protectedLayout).toContainElement(ownerPage);
  });
});
