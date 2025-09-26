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
    <div
      className={`navbar bg-base-100 shadow-sm border-b border-base-300 px-4 ${
        className || ""
      }`}
    >
      {/* Lado izquierdo - Toggle sidebar y logo */}
      <div className="navbar-start">
        <div className="flex items-center gap-4">
          {showSidebarToggle && (
            <button
              onClick={onToggleSidebar}
              className="btn btn-ghost btn-sm lg:hidden"
              aria-label="Toggle sidebar"
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
                  d="M4 6h16M4 12h16M4 18h16"
                />
              </svg>
            </button>
          )}
          <Link href="/" className="btn btn-ghost text-xl font-bold">
            🏠 RealEstate
          </Link>
        </div>
      </div>

      {/* Centro - Breadcrumbs o título de página (opcional) */}
      <div className="navbar-center hidden lg:flex">
        {/* Aquí se pueden agregar breadcrumbs si se necesitan */}
      </div>

      {/* Lado derecho - Usuario y tema */}
      <div className="navbar-end">
        <div className="flex items-center gap-2">
          {/* Selector de tema */}
          <ThemeSelector />

          {/* Dropdown del usuario */}
          {user && (
            <div className="dropdown dropdown-end">
              <div
                tabIndex={0}
                role="button"
                className="btn btn-ghost btn-circle avatar placeholder"
              >
                <div className="bg-neutral text-neutral-content rounded-full w-10">
                  <span className="text-sm font-medium">
                    {user.firstName?.[0]?.toUpperCase() || "U"}
                    {user.lastName?.[0]?.toUpperCase() || ""}
                  </span>
                </div>
              </div>
              <ul
                tabIndex={0}
                className="menu menu-sm dropdown-content mt-3 z-[1] p-2 shadow bg-base-100 rounded-box w-52 border border-base-300"
              >
                <li className="menu-title">
                  <span>
                    {user.firstName} {user.lastName}
                  </span>
                </li>
                <li>
                  <a className="text-base-content/70 text-xs">{user.email}</a>
                </li>
                <div className="divider my-1"></div>
                <li>
                  <Link href="/profile" className="flex items-center gap-2">
                    <svg
                      className="w-4 h-4"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"
                      />
                    </svg>
                    Perfil
                  </Link>
                </li>
                <li>
                  <Link href="/settings" className="flex items-center gap-2">
                    <svg
                      className="w-4 h-4"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"
                      />
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"
                      />
                    </svg>
                    Configuración
                  </Link>
                </li>
                <div className="divider my-1"></div>
                <li>
                  <button
                    onClick={handleLogout}
                    className="flex items-center gap-2 text-error"
                  >
                    <svg
                      className="w-4 h-4"
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
                    Cerrar Sesión
                  </button>
                </li>
              </ul>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default Navbar;
