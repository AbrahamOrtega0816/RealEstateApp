import React from "react";
import { ThemeSelector } from "@/components";

interface LoginLayoutProps {
  children: React.ReactNode;
}

const LoginLayout: React.FC<LoginLayoutProps> = ({ children }) => {
  return (
    <div className="min-h-screen relative overflow-hidden bg-gradient-to-br from-base-200 via-base-100 to-base-200">
      {/* Decorative geometric shapes */}
      <div className="absolute inset-0 overflow-hidden pointer-events-none">
        {/* Large circle - top left */}
        <div className="absolute -top-32 -left-32 w-64 h-64 rounded-full bg-primary/10"></div>

        {/* Medium circle - top right */}
        <div className="absolute -top-16 -right-16 w-48 h-48 rounded-full bg-secondary/8"></div>

        {/* Small circles scattered */}
        <div className="absolute top-1/4 left-1/4 w-32 h-32 rounded-full bg-accent/6"></div>

        {/* Floating geometric shapes */}
        <div className="absolute top-1/3 right-1/4 w-16 h-16 bg-gradient-to-br from-primary to-secondary opacity-8 rotate-45"></div>

        {/* Bottom decorative elements */}
        <div className="absolute -bottom-24 left-1/3 w-56 h-56 rounded-full bg-primary/5"></div>

        {/* Subtle pattern overlay */}
        <div
          className="absolute inset-0 opacity-30"
          style={{
            backgroundImage: `radial-gradient(circle at 25% 25%, hsl(var(--bc) / 0.05) 1px, transparent 1px), radial-gradient(circle at 75% 75%, hsl(var(--bc) / 0.05) 1px, transparent 1px)`,
            backgroundSize: "50px 50px",
          }}
        ></div>
      </div>

      {/* Main Content */}
      <main className="relative z-20 flex items-center justify-center px-4 py-16 min-h-screen">
        <div className="w-full max-w-md">
          {/* Glass morphism container with proper spacing to avoid theme selector overlap */}
          <div className="backdrop-blur-xl rounded-3xl p-8 shadow-2xl border border-base-300/20 bg-base-100/85 mt-20 sm:mt-12 mb-8">
            {children}
          </div>
        </div>
      </main>

      {/* Footer */}
      <footer className="relative z-20 text-center pb-8 pt-4">
        <p className="text-sm font-medium text-base-content/70">
          &copy; 2025 RealEstate App. All rights reserved.
        </p>
      </footer>
    </div>
  );
};

export default LoginLayout;
