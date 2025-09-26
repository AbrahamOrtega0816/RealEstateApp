"use client";

import React, { useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/modules/login/hooks/useAuth";
import { useIsHydrated } from "@/stores/user.store";

interface ProtectedRouteProps {
  children: React.ReactNode;
  fallback?: React.ReactNode;
}

/**
 * Componente que protege rutas verificando la autenticación del usuario
 * Redirige al login si el usuario no está autenticado
 */
const ProtectedRoute: React.FC<ProtectedRouteProps> = ({
  children,
  fallback = (
    <div className="min-h-screen flex items-center justify-center bg-base-200">
      <div className="text-center">
        <div className="loading loading-spinner loading-lg text-primary"></div>
        <p className="mt-4 text-base-content/70">Loading...</p>
      </div>
    </div>
  ),
}) => {
  const router = useRouter();
  const { isAuthenticated, user, accessToken } = useAuth();
  const isHydrated = useIsHydrated();

  useEffect(() => {
    // Esperar a que el store esté hidratado antes de decidir
    if (!isHydrated) return;
    // Si no está autenticado, redirigir al login
    if (!isAuthenticated || !user || !accessToken) {
      router.replace("/login");
    }
  }, [isHydrated, isAuthenticated, user, accessToken, router]);

  // Mostrar loading mientras se verifica la autenticación
  if (!isHydrated || !isAuthenticated || !user || !accessToken) {
    return <>{fallback}</>;
  }

  // Usuario autenticado, mostrar contenido protegido
  return <>{children}</>;
};

export default ProtectedRoute;
