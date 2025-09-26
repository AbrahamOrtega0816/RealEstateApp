"use client";

import React from "react";
import { useAuth } from "@/modules/login/hooks/useAuth";

/**
 * Página principal del módulo de propiedades
 * Muestra la lista de propiedades y permite gestionarlas
 */
const OwnerPage: React.FC = () => {
  const { user } = useAuth();

  return <div>OwnerPage</div>;
};

export default OwnerPage;
