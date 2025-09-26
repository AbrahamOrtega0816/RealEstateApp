"use client";

import React from "react";
import { ProtectedLayout } from "@/components";
import OwnerPage from "@/modules/owners/page";

/**
 * Página de propiedades protegida
 * Solo accesible para usuarios autenticados
 * Incluye navbar y sidebar integrados
 */
export default function Owners() {
  return (
    <ProtectedLayout>
      <OwnerPage />
    </ProtectedLayout>
  );
}
