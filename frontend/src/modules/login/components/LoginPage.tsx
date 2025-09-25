"use client";

import React from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "../hooks/useAuth";
import LoginForm from "./LoginForm";
import LoginCard from "./LoginCard";
import { LoginDto } from "@/modules/login/types/auth";

const LoginPage: React.FC = () => {
  const router = useRouter();
  const { login, isLoginLoading, loginError, isAuthenticated } = useAuth();

  // Redireccionar si ya está autenticado
  React.useEffect(() => {
    if (isAuthenticated) {
      router.push("/dashboard"); // O la ruta que corresponda
    }
  }, [isAuthenticated, router]);

  const handleLogin = async (data: LoginDto) => {
    try {
      await login(data);
      // El useAuth ya maneja la redirección al actualizar isAuthenticated
    } catch (error) {
      // El error ya se maneja en el hook useAuth
      console.error("Error en login:", error);
    }
  };

  if (isAuthenticated) {
    return <div>Redirecting...</div>;
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full space-y-8">
        <LoginCard>
          <LoginForm onSubmit={handleLogin} isLoading={isLoginLoading} />
          {loginError && (
            <div className="mt-4 p-3 bg-red-50 border border-red-200 rounded-md">
              <p className="text-sm text-red-600">
                {loginError.message || "Error al iniciar sesión"}
              </p>
            </div>
          )}
        </LoginCard>
      </div>
    </div>
  );
};

export default LoginPage;
