import React from "react";
import { render, screen } from "@testing-library/react";
import Properties from "@/app/properties/page";

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

// Mock del componente PropertiesPage del módulo
jest.mock("@/modules/properties/page", () => {
  return function MockPropertiesPage() {
    return (
      <div data-testid="properties-page">
        <h1>Properties Management</h1>
        <div data-testid="properties-filters">
          <input placeholder="Search properties..." />
          <select>
            <option>All Types</option>
            <option>House</option>
            <option>Apartment</option>
          </select>
        </div>
        <div data-testid="properties-grid">Properties Grid</div>
        <button data-testid="add-property-btn">Add Property</button>
      </div>
    );
  };
});

describe("Properties Page", () => {
  it("should render within ProtectedLayout", () => {
    render(<Properties />);

    // Verificar que está dentro del ProtectedLayout
    expect(screen.getByTestId("protected-layout")).toBeInTheDocument();
    expect(screen.getByTestId("navbar")).toBeInTheDocument();
    expect(screen.getByTestId("sidebar")).toBeInTheDocument();
    expect(screen.getByTestId("main-content")).toBeInTheDocument();
  });

  it("should render PropertiesPage component", () => {
    render(<Properties />);

    expect(screen.getByTestId("properties-page")).toBeInTheDocument();
    expect(screen.getByText("Properties Management")).toBeInTheDocument();
  });

  it("should display properties management elements", () => {
    render(<Properties />);

    expect(screen.getByTestId("properties-filters")).toBeInTheDocument();
    expect(screen.getByTestId("properties-grid")).toBeInTheDocument();
    expect(screen.getByTestId("add-property-btn")).toBeInTheDocument();
  });

  it("should display search and filter functionality", () => {
    render(<Properties />);

    expect(
      screen.getByPlaceholderText("Search properties...")
    ).toBeInTheDocument();
    expect(screen.getByRole("combobox")).toBeInTheDocument();
    expect(screen.getByText("All Types")).toBeInTheDocument();
  });

  it("should be a protected route", () => {
    render(<Properties />);

    // Verificar que está envuelto en ProtectedLayout
    const protectedLayout = screen.getByTestId("protected-layout");
    const propertiesPage = screen.getByTestId("properties-page");

    expect(protectedLayout).toContainElement(propertiesPage);
  });
});
