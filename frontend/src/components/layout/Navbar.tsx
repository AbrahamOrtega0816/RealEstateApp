"use client";

import React from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useAuth } from "@/modules/login/hooks/useAuth";
import { ThemeSelector } from "../ui/ThemeSelector";

interface NavbarProps {
  onToggleSidebar?: () => void;
  showSidebarToggle?: boolean;
  className?: string;
}

/**
 * Componente de barra de navegación superior
 * Incluye logo, información del usuario, y controles de tema
 */
const Navbar: React.FC<NavbarProps> = ({
  onToggleSidebar,
  showSidebarToggle = true,
  className,
}) => {
  const router = useRouter();
  const { user, logout } = useAuth();

  const handleLogout = () => {
    logout();
    router.push("/login");
  };

  return (
    <nav
      className={`navbar h-16 px-4 lg:px-6 theme-transition rounded-none
        bg-gradient-to-r from-base-100/90 via-base-100/70 to-base-100/90
        backdrop-blur supports-[backdrop-filter]:bg-base-100/70 border-b border-base-300/60
        ${className || ""}`}
    >
      {/* Lado izquierdo - Toggle sidebar y logo */}
      <div className="navbar-start">
        <div className="flex items-center gap-3">
          {showSidebarToggle && (
            <button
              onClick={onToggleSidebar}
              className="btn btn-ghost btn-sm lg:hidden hover:bg-primary/10 hover:text-primary transition-all duration-200 rounded-xl"
              aria-label="Toggle sidebar"
            >
              <svg
                className="w-5 h-5 transition-transform hover:scale-110"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M4 6h16M4 12h16M4 18h16"
                />
              </svg>
            </button>
          )}
          <Link
            href="/"
            className="btn btn-ghost text-xl font-bold hover:bg-primary/10 hover:text-primary transition-all duration-200 rounded-xl group"
          >
            <span className="text-2xl group-hover:scale-110 transition-transform duration-200">
              🏠
            </span>
            <span className="bg-gradient-to-r from-primary to-secondary bg-clip-text text-transparent">
              RealEstate
            </span>
          </Link>
        </div>
      </div>

      {/* Centro - Breadcrumbs o título de página (opcional) */}
      <div className="navbar-center hidden lg:flex">
        <div className="flex items-center gap-2 text-sm text-base-content/60">
          {/* Aquí se pueden agregar breadcrumbs dinámicos basados en la ruta */}
        </div>
      </div>

      {/* Lado derecho - Usuario y tema */}
      <div className="navbar-end">
        <div className="flex items-center gap-3">
          {/* Selector de tema mejorado */}
          <div className="hidden sm:block">
            <ThemeSelector />
          </div>

          {/* Dropdown del usuario mejorado */}
          {user && (
            <div className="dropdown dropdown-end">
              <div
                tabIndex={0}
                role="button"
                className="btn btn-ghost btn-circle avatar placeholder hover:bg-primary/10 transition-all duration-200 group"
              >
                <div className="bg-gradient-to-br from-primary to-secondary text-primary-content rounded-full w-10 h-10 relative group-hover:scale-105 transition-transform duration-200 shadow-lg select-none">
                  <span className="absolute inset-0 inline-flex items-center justify-center text-sm font-bold">
                    {user.firstName?.[0]?.toUpperCase() || "U"}
                    {user.lastName?.[0]?.toUpperCase() || ""}
                  </span>
                </div>
              </div>
              <ul
                tabIndex={0}
                className="menu menu-sm dropdown-content mt-3 z-50 p-3 shadow-2xl bg-base-100/95 backdrop-blur-md rounded-2xl w-64 border border-base-300/50 theme-transition"
              >
                <li className="menu-title px-3 py-2">
                  <div className="flex items-center gap-3">
                    <div className="avatar placeholder">
                      <div className="bg-primary/20 text-primary rounded-full w-8 h-8 relative select-none">
                        <span className="absolute inset-0 inline-flex items-center justify-center text-xs font-bold">
                          {user.firstName?.[0]?.toUpperCase() || "U"}
                          {user.lastName?.[0]?.toUpperCase() || ""}
                        </span>
                      </div>
                    </div>
                    <div className="flex flex-col items-start">
                      <span className="font-semibold text-base-content text-left">
                        {user.firstName} {user.lastName}
                      </span>
                      <span className="text-xs text-base-content/60 font-normal">
                        {user.email}
                      </span>
                    </div>
                  </div>
                </li>
                <li>
                  <button
                    onClick={handleLogout}
                    className="flex items-center gap-3 p-3 rounded-xl hover:bg-error/10 hover:text-error transition-all duration-200 text-error/80"
                  >
                    <svg
                      className="w-5 h-5"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"
                      />
                    </svg>
                    <span className="font-medium">Sign out</span>
                  </button>
                </li>
              </ul>
            </div>
          )}
        </div>
      </div>
    </nav>
  );
};

export default Navbar;
