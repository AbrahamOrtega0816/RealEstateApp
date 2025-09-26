"use client";

import React, { useState } from "react";
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
 * Proporciona una estructura consistente para todas las páginas
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
    <div className="min-h-screen bg-base-200">
      {/* Navbar fijo en la parte superior */}
      <div className="sticky top-0 z-30">
        <Navbar
          onToggleSidebar={toggleSidebar}
          showSidebarToggle={showSidebar}
          className={navbarClassName}
        />
      </div>

      {/* Contenedor principal con sidebar y contenido */}
      <div className="flex">
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
            flex-1 transition-all duration-300 ease-in-out
            ${showSidebar ? "lg:ml-0" : ""}
            ${contentClassName || ""}
          `}
        >
          <div className="p-6">{children}</div>
        </main>
      </div>
    </div>
  );
};

export default AppLayout;
