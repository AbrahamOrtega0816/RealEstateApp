"use client";

import React from "react";
import { useRouter } from "next/navigation";
import { toast } from "react-hot-toast";
import LoginLayout from "../layout";
import { LoginCard, LoginForm } from "../components";
import { useAuth } from "../hooks/useAuth";
import { LoginDto } from "../types/auth";

const LoginPage: React.FC = () => {
  const router = useRouter();
  const { login, isLoginLoading, isAuthenticated } = useAuth();

  // Redirect if already authenticated
  React.useEffect(() => {
    if (isAuthenticated) {
      router.push("/"); // or your main application route
    }
  }, [isAuthenticated, router]);

  const handleLogin = async (data: LoginDto) => {
    try {
      await toast.promise(login(data), {
        loading: "Logging in...",
        success: "Login successful",
      });
      router.push("/properties");
    } catch (error: unknown) {
      const errorMessage =
        error instanceof Error ? error.message : "Login failed";
      toast.error(errorMessage);
    }
  };

  return (
    <LoginLayout>
      <LoginCard
        title="Welcome back"
        subtitle="Enter your credentials to access your account"
      >
        <LoginForm onSubmit={handleLogin} isLoading={isLoginLoading} />
      </LoginCard>
    </LoginLayout>
  );
};

export default LoginPage;
