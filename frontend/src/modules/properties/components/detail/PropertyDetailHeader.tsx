"use client";

import React from "react";
import { useRouter } from "next/navigation";
import { Icon } from "@iconify/react";

interface PropertyDetailHeaderProps {
  propertyName?: string;
  isLoading?: boolean;
}

/**
 * Componente header para la página de detalles de propiedad
 * Incluye navegación de regreso y título
 */
export const PropertyDetailHeader: React.FC<PropertyDetailHeaderProps> = ({
  propertyName,
  isLoading = false,
}) => {
  const router = useRouter();

  const handleBackToProperties = () => {
    router.push("/properties");
  };

  return (
    <div className="flex items-center justify-between mb-6">
      <div className="flex items-center gap-4">
        <button
          onClick={handleBackToProperties}
          className="btn btn-ghost btn-circle hover:bg-base-200"
          title="Back to Properties"
        >
          <Icon icon="mdi:arrow-left" className="w-5 h-5" />
        </button>
        <div>
          <h1 className="text-3xl font-bold text-gray-900 dark:text-base-content">
            Property Details
          </h1>
          {!isLoading && propertyName && (
            <p className="text-gray-600 dark:text-base-content/70 mt-1">
              {propertyName}
            </p>
          )}
          {isLoading && <div className="skeleton h-4 w-48 mt-1"></div>}
        </div>
      </div>
    </div>
  );
};
