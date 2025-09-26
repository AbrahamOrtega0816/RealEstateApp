"use client";

import React from "react";
import { ProtectedLayout } from "@/components";
import PropertiesDetailPage from "@/modules/properties/page/detail";

/**
 * Página de propiedades protegida
 * Solo accesible para usuarios autenticados
 * Incluye navbar y sidebar integrados
 */
export default function PropertieDetail() {
  return (
    <ProtectedLayout>
      <PropertiesDetailPage />
    </ProtectedLayout>
  );
}
