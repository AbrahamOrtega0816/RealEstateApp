"use client";

import React from "react";
import ProtectedRoute from "../ProtectedRoute";
import AppLayout from "./AppLayout";

interface ProtectedLayoutProps {
  children: React.ReactNode;
  showSidebar?: boolean;
  sidebarClassName?: string;
  navbarClassName?: string;
  contentClassName?: string;
  fallback?: React.ReactNode;
}

/**
 * Componente que combina ProtectedRoute con AppLayout
 * Proporciona protección de rutas y layout consistente en un solo componente
 */
const ProtectedLayout: React.FC<ProtectedLayoutProps> = ({
  children,
  showSidebar = true,
  sidebarClassName,
  navbarClassName,
  contentClassName,
  fallback,
}) => {
  return (
    <ProtectedRoute fallback={fallback}>
      <AppLayout
        showSidebar={showSidebar}
        sidebarClassName={sidebarClassName}
        navbarClassName={navbarClassName}
        contentClassName={contentClassName}
      >
        {children}
      </AppLayout>
    </ProtectedRoute>
  );
};

export default ProtectedLayout;
