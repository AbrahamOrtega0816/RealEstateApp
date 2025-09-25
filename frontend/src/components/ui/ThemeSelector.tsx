"use client";

import React from "react";
import { useTheme } from "../../providers/ThemeProvider";

interface ThemeSelectorProps {
  className?: string;
}

// Compact three-icon toggle: Light (sun) / Dark (moon) / Luxury (diamond)
export function ThemeSelector({ className = "" }: ThemeSelectorProps) {
  const { theme, setTheme } = useTheme();

  const baseBtn =
    "btn btn-ghost btn-xs btn-square rounded-full w-8 h-8 mx-0.5 text-base-content/70 transition-all duration-300 ease-out hover:scale-105 hover:bg-base-200/80 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary/50";
  const active =
    "!bg-primary !text-primary-content shadow-lg scale-110 ring-2 ring-primary/30";

  return (
    <div
      className={`join rounded-full border border-base-300/40 bg-base-100/90 backdrop-blur-md shadow-2xl px-3 py-2 gap-1 transition-all duration-300 hover:shadow-3xl hover:border-base-300/60 ${className}`}
      aria-label="Theme selector"
    >
      <button
        className={`${baseBtn} join-item ${theme === "light" ? active : ""}`}
        onClick={() => setTheme("light")}
        aria-label="Set theme to Light"
        title="Light"
        aria-pressed={theme === "light"}
      >
        <svg
          className={`w-3.5 h-3.5 transition-transform ${
            theme === "light" ? "scale-110" : ""
          }`}
          viewBox="0 0 20 20"
          fill="currentColor"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            fillRule="evenodd"
            d="M10 2a1 1 0 011 1v1a1 1 0 11-2 0V3a1 1 0 011-1zm4 8a4 4 0 11-8 0 4 4 0 018 0zm-.464 4.95l.707.707a1 1 0 001.414-1.414l-.707-.707a1 1 0 00-1.414 1.414zm2.12-10.607a1 1 0 010 1.414l-.706.707a1 1 0 11-1.414-1.414l.707-.707a1 1 0 011.414 0zM17 11a1 1 0 100-2h-1a1 1 0 100 2h1zm-7 4a1 1 0 011 1v1a1 1 0 11-2 0v-1a1 1 0 011-1zM5.05 6.464A1 1 0 106.465 5.05l-.708-.707a1 1 0 00-1.414 1.414l.707.707zm1.414 8.486l-.707.707a1 1 0 01-1.414-1.414l.707-.707a1 1 0 011.414 1.414z"
            clipRule="evenodd"
          />
        </svg>
      </button>
      <button
        className={`${baseBtn} join-item ${theme === "dark" ? active : ""}`}
        onClick={() => setTheme("dark")}
        aria-label="Set theme to Dark"
        title="Dark"
        aria-pressed={theme === "dark"}
      >
        <svg
          className={`w-3.5 h-3.5 transition-transform ${
            theme === "dark" ? "scale-110" : ""
          }`}
          viewBox="0 0 20 20"
          fill="currentColor"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path d="M17.293 13.293A8 8 0 016.707 2.707a8.001 8.001 0 1010.586 10.586z" />
        </svg>
      </button>
      <button
        className={`${baseBtn} join-item ${theme === "luxury" ? active : ""}`}
        onClick={() => setTheme("luxury")}
        aria-label="Set theme to Luxury"
        title="Luxury"
        aria-pressed={theme === "luxury"}
      >
        {/* Diamond icon */}
        <svg
          className={`w-3.5 h-3.5 transition-transform ${
            theme === "luxury" ? "scale-110" : ""
          }`}
          viewBox="0 0 24 24"
          fill="currentColor"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path d="M3.1 9.2L8.6 3h6.8l5.5 6.2-9 11.3-8.8-11.3zM9.3 5l-3.3 3.7 3 1.1L12 5.7 14.9 10l3-1.1L14.7 5H9.3z" />
        </svg>
      </button>
    </div>
  );
}

// Backwards compatible export name
export function ThemeToggle({ className = "" }: { className?: string }) {
  return <ThemeSelector className={className} />;
}
