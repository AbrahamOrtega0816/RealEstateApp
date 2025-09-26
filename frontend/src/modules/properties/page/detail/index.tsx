"use client";

import React from "react";
import { useParams } from "next/navigation";
import { Icon } from "@iconify/react";
import { useGetPropertyById } from "../../services/propertyService";
import {
  PropertyDetailHeader,
  PropertyDetailImages,
  PropertyDetailInfo,
} from "../../components/detail";

/**
 * Página de detalles de propiedades
 * Integra el servicio useGetPropertyById para obtener los datos de la propiedad
 */
const PropertiesDetailPage = () => {
  const params = useParams();
  const propertyId = params?.id as string;

  // Usar el servicio para obtener los datos de la propiedad
  const {
    data: property,
    isLoading,
    error,
    isError,
  } = useGetPropertyById(propertyId);

  return (
    <div className="p-4 lg:p-6">
      <div className="max-w-4xl mx-auto">
        {/* Header */}
        <PropertyDetailHeader
          propertyName={property?.name}
          isLoading={isLoading}
        />

        {/* Error State */}
        {isError && (
          <div className="bg-base-100 rounded-lg shadow-lg p-6 border border-base-300 text-center">
            <Icon
              icon="mdi:alert-circle"
              className="w-16 h-16 mx-auto mb-4 text-error"
            />
            <h2 className="text-xl font-semibold mb-2 text-error">
              Error Loading Property
            </h2>
            <p className="text-gray-600 dark:text-base-content/70 mb-4">
              {error?.message ||
                "There was an error loading the property details. Please try again later."}
            </p>
            <button
              onClick={() => window.location.reload()}
              className="btn btn-primary gap-2"
            >
              <Icon icon="mdi:refresh" className="w-4 h-4" />
              Try Again
            </button>
          </div>
        )}

        {/* Property Content */}
        {!isError && (
          <div className="space-y-6">
            {/* Images Section */}
            <PropertyDetailImages
              images={property?.images || []}
              propertyName={property?.name || "Property"}
              isLoading={isLoading}
            />

            {/* Property Information */}
            <PropertyDetailInfo property={property} isLoading={isLoading} />
          </div>
        )}

        {/* Property ID Debug Info (only show when not loading and no error) */}
        {!isLoading && !isError && (
          <div className="mt-6 p-4 bg-base-200 rounded-lg">
            <p className="text-sm text-gray-500 dark:text-base-content/60 text-center">
              Property ID: <span className="font-mono">{propertyId}</span>
            </p>
          </div>
        )}
      </div>
    </div>
  );
};

export default PropertiesDetailPage;
