"use client";

import React from "react";
import { ProtectedLayout } from "@/components";
import PropertiesPage from "@/modules/properties/page";

/**
 * Página de propiedades protegida
 * Solo accesible para usuarios autenticados
 * Incluye navbar y sidebar integrados
 */
export default function Properties() {
  return (
    <ProtectedLayout>
      <PropertiesPage />
    </ProtectedLayout>
  );
}
