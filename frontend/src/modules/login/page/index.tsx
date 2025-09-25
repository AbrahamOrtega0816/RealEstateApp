"use client";

import React, { useState } from "react";
import LoginLayout from "../layout";
import { LoginCard, LoginForm } from "../components";

interface LoginData {
  email: string;
  password: string;
}

const LoginPage: React.FC = () => {
  const [isLoading, setIsLoading] = useState(false);

  const handleLogin = async (data: LoginData) => {
    try {
      setIsLoading(true);

      // Here will go the authentication logic
      console.log("Login data:", data);

      // Simulate API call
      await new Promise((resolve) => setTimeout(resolve, 2000));

      // Here you can add the logic for:
      // - Call authentication API
      // - Save token
      // - Redirect user

      alert("Login successful! (this is just for demonstration)");
    } catch (error) {
      console.error("Login error:", error);
      alert("Error signing in");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <LoginLayout>
      <LoginCard
        title="Welcome back"
        subtitle="Enter your credentials to access your account"
      >
        <LoginForm onSubmit={handleLogin} isLoading={isLoading} />
      </LoginCard>
    </LoginLayout>
  );
};

export default LoginPage;
