"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import {
  useIsAuthenticated,
  useAccessToken,
  useUserStore,
} from "@/stores/user.store";

export default function Home() {
  const router = useRouter();
  const isAuthenticated = useIsAuthenticated();
  const accessToken = useAccessToken();
  const isTokenExpired = useUserStore((state) => state.isTokenExpired);

  useEffect(() => {
    // Check if user is authenticated and token is valid
    if (isAuthenticated && accessToken && !isTokenExpired()) {
      // Authenticated user with valid token, redirect to properties
      router.push("/properties");
    } else {
      // User not authenticated or token expired, redirect to login
      router.push("/login");
    }
  }, [isAuthenticated, accessToken, isTokenExpired, router]);

  // Show loading while checking authentication
  return (
    <div className="min-h-screen flex items-center justify-center bg-base-200">
      <div className="text-center">
        <div
          role="status"
          aria-live="polite"
          className="loading loading-spinner loading-lg text-primary"
        ></div>
        <p className="mt-4 text-base-content/70">Checking authentication...</p>
      </div>
    </div>
  );
}
