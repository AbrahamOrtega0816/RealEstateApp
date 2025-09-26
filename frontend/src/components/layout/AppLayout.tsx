"use client";

import React, { useState, useEffect } from "react";
import Navbar from "./Navbar";
import Sidebar from "./Sidebar";

interface AppLayoutProps {
  children: React.ReactNode;
  showSidebar?: boolean;
  sidebarClassName?: string;
  navbarClassName?: string;
  contentClassName?: string;
}

/**
 * Layout principal de la aplicación que integra Navbar y Sidebar
 * Proporciona una estructura consistente para todas las páginas con soporte completo de temas
 */
const AppLayout: React.FC<AppLayoutProps> = ({
  children,
  showSidebar = true,
  sidebarClassName,
  navbarClassName,
  contentClassName,
}) => {
  const [isSidebarOpen, setIsSidebarOpen] = useState(false);
  const toggleSidebar = () => {
    setIsSidebarOpen(!isSidebarOpen);
  };

  const closeSidebar = () => {
    setIsSidebarOpen(false);
  };

  return (
    <div className="min-h-screen bg-base-200 theme-transition flex flex-col">
      {/* Navbar fijo en la parte superior */}
      <div className="sticky top-0 z-30 backdrop-blur-md bg-base-100/80 border-b border-base-300/50 shadow-sm flex-shrink-0">
        <Navbar
          onToggleSidebar={toggleSidebar}
          showSidebarToggle={showSidebar}
          className={`bg-transparent ${navbarClassName || ""}`}
        />
      </div>

      {/* Contenedor principal con sidebar y contenido */}
      <div className="flex-1 min-h-0 grid grid-cols-1 lg:grid-cols-[16rem_minmax(0,1fr)]">
        {/* Sidebar */}
        {showSidebar && (
          <Sidebar
            isOpen={isSidebarOpen}
            onClose={closeSidebar}
            className={sidebarClassName}
          />
        )}

        {/* Contenido principal */}
        <main
          className={`
            overflow-auto transition-all duration-500 ease-out theme-transition min-h-0
            ${contentClassName || ""}
          `}
        >
          <div className="container mx-auto px-4 sm:px-6 lg:px-8 py-6 max-w-7xl">
            {children}
          </div>
        </main>
      </div>
    </div>
  );
};

export default AppLayout;
