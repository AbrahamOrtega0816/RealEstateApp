"use client";

import React from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { useAuth } from "@/modules/login/hooks/useAuth";

interface MenuItem {
  id: string;
  label: string;
  href: string;
  icon: React.ReactNode;
  badge?: string | number;
  children?: MenuItem[];
}

interface SidebarProps {
  isOpen?: boolean;
  onClose?: () => void;
  className?: string;
}

/**
 * Componente de barra lateral de navegación
 * Incluye menú de navegación principal con iconos y badges opcionales
 */
const Sidebar: React.FC<SidebarProps> = ({
  isOpen = true,
  onClose,
  className = "",
}) => {
  const pathname = usePathname();
  const { user } = useAuth();

  // Configuración de elementos del menú
  const menuItems: MenuItem[] = [
    {
      id: "properties",
      label: "Properties",
      href: "/properties",
      icon: (
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
            d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"
          />
        </svg>
      ),
    },
    {
      id: "owners",
      label: "Owners",
      href: "/owners",
      icon: (
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
            d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.25 2.25 0 11-4.5 0 2.25 2.25 0 014.5 0z"
          />
        </svg>
      ),
    },
  ];

  const isActive = (href: string) => {
    if (href === "/") {
      return pathname === "/";
    }
    return pathname.startsWith(href);
  };

  const handleItemClick = () => {
    // Cerrar sidebar en móvil al hacer click en un item
    if (onClose) {
      onClose();
    }
  };

  return (
    <>
      {/* Overlay para móvil */}
      {isOpen && (
        <div className="fixed inset-0 z-40 lg:hidden" onClick={onClose} />
      )}

      {/* Sidebar */}
      <aside
        className={`
          fixed top-0 left-0 z-50 h-screen w-64 min-h-screen max-h-screen
          bg-base-200/90 backdrop-blur-sm
          border-r border-base-300/60 shadow-2xl flex flex-col
          transform transition-transform duration-300 ease-in-out
          lg:relative lg:translate-x-0 lg:z-auto lg:h-screen lg:min-h-screen lg:max-h-screen
          ${isOpen ? "translate-x-0" : "-translate-x-full"}
          ${className}
        `}
      >
        {/* Header del sidebar */}
        <div className="flex items-center justify-end px-3 py-2 border-b border-base-300/60 bg-base-200/60 flex-shrink-0 lg:hidden">
          {/* Botón cerrar para móvil */}
          <button
            onClick={onClose}
            className="p-2 rounded-lg hover:bg-base-300/70 text-base-content/80 hover:text-base-content transition-all duration-200"
            aria-label="Close sidebar"
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
                d="M6 18L18 6M6 6l12 12"
              />
            </svg>
          </button>
        </div>

        {/* Navegación */}
        <nav className="flex-1 overflow-y-auto p-3 pt-2 lg:p-4 lg:pt-4">
          <div className="space-y-2">
            {menuItems.map((item) => (
              <Link
                key={item.id}
                href={item.href}
                onClick={handleItemClick}
                className={`
                  flex items-center gap-4 rounded-xl px-3 py-2.5 transition-all duration-200 group relative
                  ${
                    isActive(item.href)
                      ? "bg-gradient-to-r from-primary/15 to-primary/5 text-primary shadow-sm"
                      : "text-base-content/80 hover:bg-base-300/60 hover:text-base-content hover:translate-x-1"
                  }
                `}
              >
                <div
                  className={`transition-all duration-200 ${
                    isActive(item.href)
                      ? "text-primary"
                      : "group-hover:text-primary"
                  }`}
                >
                  {item.icon}
                </div>
                <span className="font-medium">{item.label}</span>
                {item.badge && (
                  <span className="ml-auto px-2 py-1 text-xs font-bold bg-primary text-primary-content rounded-full">
                    {item.badge}
                  </span>
                )}
              </Link>
            ))}
          </div>
        </nav>

        {/* Footer del sidebar */}
        <div className="p-4 border-t border-base-300/60 bg-base-200/40 flex-shrink-0 mt-auto">
          <div className="text-center">
            <p className="text-xs text-base-content/60 font-medium">
              RealEstate App v1.0
            </p>
            <div className="mt-2 flex justify-center">
              <div className="w-8 h-1 bg-gradient-to-r from-primary to-secondary rounded-full"></div>
            </div>
          </div>
        </div>
      </aside>
    </>
  );
};

export default Sidebar;
