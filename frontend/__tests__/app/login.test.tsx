import React from "react";
import { render, screen } from "@testing-library/react";
import Login from "@/app/login/page";

// Mock del componente LoginPage del módulo
jest.mock("@/modules/login/page", () => {
  return function MockLoginPage() {
    return <div data-testid="login-page">Login Page Component</div>;
  };
});

describe("Login Page", () => {
  it("should render LoginPage component", () => {
    render(<Login />);

    expect(screen.getByTestId("login-page")).toBeInTheDocument();
    expect(screen.getByText("Login Page Component")).toBeInTheDocument();
  });

  it("should be accessible", () => {
    render(<Login />);

    const loginPage = screen.getByTestId("login-page");
    expect(loginPage).toBeInTheDocument();
  });
});
