"use client";

import React from "react";
import { ProtectedRoute } from "@/components";
import PropertiesPage from "@/modules/properties/page";

/**
 * Página de propiedades protegida
 * Solo accesible para usuarios autenticados
 */
export default function Properties() {
  return (
    <ProtectedRoute>
      <PropertiesPage />
    </ProtectedRoute>
  );
}
