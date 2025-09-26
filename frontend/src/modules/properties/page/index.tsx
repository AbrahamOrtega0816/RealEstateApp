"use client";

import React from "react";
import { useAuth } from "@/modules/login/hooks/useAuth";

/**
 * Página principal del módulo de propiedades
 * Muestra la lista de propiedades y permite gestionarlas
 */
const PropertiesPage: React.FC = () => {
  const { user } = useAuth();

  return <div>xxxxx</div>;
};

export default PropertiesPage;
