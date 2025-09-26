"use client";

import React from "react";
import { Icon } from "@iconify/react";
import { PropertyDto, PropertyFilterDto } from "../types/property";
import { PropertyGrid } from "./PropertyGrid";
import { PropertyFilters } from "./PropertyFilters";
import { Pagination } from "@/components/ui/Pagination";

interface PropertyContainerProps {
  properties: PropertyDto[];
  isLoading?: boolean;
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
  onAdd?: () => void;
  onEdit?: (property: PropertyDto) => void;
  onDelete?: (propertyId: string) => void;
  onChangePrice?: (propertyId: string, currentPrice: number) => void;
  onFilter?: (filter: PropertyFilterDto) => void;
  onViewDetails?: (propertyId: string) => void;
}

/**
 * Componente contenedor principal para las propiedades
 * Integra la grilla, filtros y paginación
 */
export const PropertyContainer: React.FC<PropertyContainerProps> = ({
  properties,
  isLoading = false,
  currentPage,
  totalPages,
  onPageChange,
  onAdd,
  onEdit,
  onDelete,
  onChangePrice,
  onFilter,
  onViewDetails,
}) => {
  // Debug pagination values
  return (
    <div className="flex flex-col space-y-4">
      {/* Header - solo título y botones */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <h2 className="text-2xl font-bold text-base-content flex-shrink-0">
          Properties Portfolio
        </h2>

        {/* Add Property Button */}
        {onAdd && (
          <button
            className="btn btn-primary transition-all duration-200 flex-shrink-0"
            onClick={onAdd}
          >
            <Icon icon="mdi:plus" className="w-4 h-4" />
            Add Property
          </button>
        )}
      </div>

      {/* Barra de filtros siempre visible */}
      <div className="w-full">
        <PropertyFilters onFilter={onFilter} />
      </div>

      {/* Properties Grid */}
      <PropertyGrid
        properties={properties}
        isLoading={isLoading}
        onEdit={onEdit}
        onDelete={onDelete}
        onChangePrice={onChangePrice}
        onViewDetails={onViewDetails}
      />

      {/* Pagination */}

      <div className="mt-8">
        <Pagination
          currentPage={currentPage}
          totalPages={totalPages}
          onPageChange={onPageChange}
        />
      </div>
    </div>
  );
};
