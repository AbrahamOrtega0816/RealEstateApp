"use client";

import React from "react";
import { Icon } from "@iconify/react";
import { PropertyDto } from "../types/property";
import { PropertyCard } from "./PropertyCard";

interface PropertyGridProps {
  properties: PropertyDto[];
  isLoading?: boolean;
  onEdit?: (property: PropertyDto) => void;
  onDelete?: (propertyId: string) => void;
  onChangePrice?: (propertyId: string, currentPrice: number) => void;
  onViewDetails?: (propertyId: string) => void;
}

/**
 * Componente que renderiza una grilla de propiedades
 */
export const PropertyGrid: React.FC<PropertyGridProps> = ({
  properties,
  isLoading = false,
  onEdit,
  onDelete,
  onChangePrice,
  onViewDetails,
}) => {
  if (isLoading) {
    return (
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        {[...Array(8)].map((_, index) => (
          <div
            key={index}
            className="bg-base-100 rounded-lg shadow-lg overflow-hidden border border-base-300"
          >
            {/* Image Skeleton */}
            <div className="h-48 bg-base-300 animate-pulse"></div>

            {/* Content Skeleton */}
            <div className="p-4">
              <div className="flex justify-between items-start mb-2">
                <div className="h-6 bg-base-300 rounded w-2/3 animate-pulse"></div>
                <div className="h-6 bg-base-300 rounded w-20 animate-pulse"></div>
              </div>

              <div className="flex items-center gap-1 mb-3">
                <div className="h-4 w-4 bg-base-300 rounded animate-pulse"></div>
                <div className="h-4 bg-base-300 rounded w-full animate-pulse"></div>
              </div>

              <div className="grid grid-cols-2 gap-3 mb-3">
                <div className="flex items-center gap-1">
                  <div className="h-4 w-4 bg-base-300 rounded animate-pulse"></div>
                  <div className="h-4 bg-base-300 rounded w-12 animate-pulse"></div>
                </div>
                <div className="flex items-center gap-1">
                  <div className="h-4 w-4 bg-base-300 rounded animate-pulse"></div>
                  <div className="h-4 bg-base-300 rounded w-16 animate-pulse"></div>
                </div>
              </div>

              <div className="pt-3 border-t border-base-300">
                <div className="h-3 bg-base-300 rounded w-24 animate-pulse"></div>
              </div>
            </div>
          </div>
        ))}
      </div>
    );
  }

  if (properties.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center py-16">
        <Icon
          icon="mdi:home-search-outline"
          className="w-24 h-24 mb-6 text-base-content/30"
        />
        <h3 className="text-2xl font-semibold text-base-content mb-2">
          No Properties Found
        </h3>
        <p className="text-base-content/60 text-center max-w-md">
          There are no properties to display. Start by adding a new property to
          your portfolio.
        </p>
      </div>
    );
  }

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
      {properties.map((property) => (
        <PropertyCard
          key={property.id}
          property={property}
          onEdit={onEdit}
          onDelete={onDelete}
          onChangePrice={onChangePrice}
          onViewDetails={onViewDetails}
        />
      ))}
    </div>
  );
};
