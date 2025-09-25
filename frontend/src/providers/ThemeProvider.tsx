"use client";

import React, { createContext, useContext, useEffect, useState } from "react";

// Available themes (limited to three: light, dark, luxury)
export type Theme = "light" | "dark" | "luxury";

interface ThemeContextType {
  theme: Theme;
  setTheme: (theme: Theme) => void;
  toggleTheme: () => void;
}

const ThemeContext = createContext<ThemeContextType | undefined>(undefined);

interface ThemeProviderProps {
  children: React.ReactNode;
  defaultTheme?: Theme;
}

export function ThemeProvider({
  children,
  defaultTheme = "light",
}: ThemeProviderProps) {
  const [theme, setThemeState] = useState<Theme>(defaultTheme);

  // Change theme and persist preference
  const setTheme = (newTheme: Theme) => {
    setThemeState(newTheme);
    localStorage.setItem("theme", newTheme);
    document.documentElement.setAttribute("data-theme", newTheme);
  };

  // Cycle through light -> dark -> luxury -> light
  const toggleTheme = () => {
    let nextTheme: Theme = "light";
    if (theme === "light") nextTheme = "dark";
    else if (theme === "dark") nextTheme = "luxury";
    else nextTheme = "light";
    setTheme(nextTheme);
  };

  // Initialize theme on mount
  useEffect(() => {
    // Check if there is a saved theme in localStorage
    const savedTheme = localStorage.getItem("theme") as Theme;

    if (savedTheme) {
      setTheme(savedTheme);
    } else {
      // If there is no saved theme, use system preference (light/dark)
      const prefersDark = window.matchMedia(
        "(prefers-color-scheme: dark)"
      ).matches;
      const systemTheme = prefersDark ? "dark" : "light";
      setTheme(systemTheme);
    }

    // Listen for system preference changes
    const mediaQuery = window.matchMedia("(prefers-color-scheme: dark)");
    const handleChange = (e: MediaQueryListEvent) => {
      if (!localStorage.getItem("theme")) {
        const systemTheme = e.matches ? "dark" : "light";
        setTheme(systemTheme);
      }
    };

    mediaQuery.addEventListener("change", handleChange);
    return () => mediaQuery.removeEventListener("change", handleChange);
  }, []);

  const value = {
    theme,
    setTheme,
    toggleTheme,
  };

  return (
    <ThemeContext.Provider value={value}>{children}</ThemeContext.Provider>
  );
}

// Hook to consume theme context
export function useTheme() {
  const context = useContext(ThemeContext);
  if (context === undefined) {
    throw new Error("useTheme must be used within a ThemeProvider");
  }
  return context;
}

// Themes available for pickers
export const availableThemes: Theme[] = ["light", "dark", "luxury"];

// Utility to get display name
export function getThemeDisplayName(theme: Theme): string {
  const themeNames: Record<Theme, string> = {
    light: "Light",
    dark: "Dark",
    luxury: "Luxury",
  };

  return themeNames[theme] || theme;
}
