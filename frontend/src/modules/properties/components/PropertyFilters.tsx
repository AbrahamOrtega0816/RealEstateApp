"use client";

import React from "react";
import { Icon } from "@iconify/react";
import { PropertyFilterDto } from "../types/property";
import { useGetOwners } from "@/modules/owners/services/ownerService";
import { usePropertyFilters } from "../hooks/usePropertyFilters";

interface PropertyFiltersProps {
  onFilter?: (filter: PropertyFilterDto) => void;
  className?: string;
}

/**
 * Componente independiente para los filtros de propiedades
 * Maneja la lógica de búsqueda y filtrado
 */
export const PropertyFilters: React.FC<PropertyFiltersProps> = ({
  onFilter,
  className = "",
}) => {
  // Use the custom hook for filter logic
  const {
    filters,
    updateSearchTerm,
    updateMinPrice,
    updateMaxPrice,
    updateSelectedOwner,
    updateSelectedYear,
    updateIsActive,
    clearFilters,
    hasActiveFilters,
  } = usePropertyFilters({ onFilter });

  // Get owners for the select dropdown
  const { data: ownersData, isLoading: isLoadingOwners } = useGetOwners({
    pageNumber: 1,
    pageSize: 100, // Get all owners for the dropdown
  });

  return (
    <div className={`w-full ${className}`}>
      {/* Barra de filtros siempre visible - Una sola fila */}
      <div className="bg-base-100 border border-base-300 rounded-lg p-4 shadow-sm">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 xl:grid-cols-8 gap-4 items-end">
          {/* Search Input */}
          <div className="form-control xl:col-span-2">
            <label className="label">
              <span className="label-text text-sm font-medium text-base-content">
                Search
              </span>
            </label>
            <div className="relative">
              <input
                type="text"
                placeholder="Search properties..."
                value={filters.searchTerm}
                onChange={(e) => updateSearchTerm(e.target.value)}
                className="input input-bordered w-full pl-10 focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all duration-200"
              />
              <Icon
                icon="mdi:magnify"
                className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-500 pointer-events-none z-10"
                style={{
                  color: "currentColor",
                  opacity: "0.7",
                }}
              />
            </div>
          </div>

          {/* Owner Selection */}
          <div className="form-control">
            <label className="label">
              <span className="label-text text-sm font-medium text-base-content">
                Owner
              </span>
            </label>
            <select
              className="select select-bordered w-full text-base-content focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all duration-200"
              value={filters.selectedOwner}
              onChange={(e) => updateSelectedOwner(e.target.value)}
            >
              <option value="">All Owners</option>
              {isLoadingOwners ? (
                <option disabled>Loading...</option>
              ) : (
                ownersData?.items?.map((owner) => (
                  <option key={owner.id} value={owner.id}>
                    {owner.name}
                  </option>
                ))
              )}
            </select>
          </div>

          {/* Min Price */}
          <div className="form-control">
            <label className="label">
              <span className="label-text text-sm font-medium text-base-content">
                Min Price
              </span>
            </label>
            <input
              type="number"
              placeholder="0"
              value={filters.minPrice}
              onChange={(e) => updateMinPrice(e.target.value)}
              className="input input-bordered w-full focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all duration-200"
            />
          </div>

          {/* Max Price */}
          <div className="form-control">
            <label className="label">
              <span className="label-text text-sm font-medium text-base-content">
                Max Price
              </span>
            </label>
            <input
              type="number"
              placeholder="999999999"
              value={filters.maxPrice}
              onChange={(e) => updateMaxPrice(e.target.value)}
              className="input input-bordered w-full focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all duration-200"
            />
          </div>

          {/* Year Filter */}
          <div className="form-control">
            <label className="label">
              <span className="label-text text-sm font-medium text-base-content">
                Year
              </span>
            </label>
            <select
              className="select select-bordered w-full focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all duration-200"
              value={filters.selectedYear}
              onChange={(e) => updateSelectedYear(e.target.value)}
            >
              <option value="">All Years</option>
              {Array.from(
                { length: new Date().getFullYear() - 1949 },
                (_, i) => {
                  const year = new Date().getFullYear() - i;
                  return (
                    <option key={year} value={year.toString()}>
                      {year}
                    </option>
                  );
                }
              )}
            </select>
          </div>

          {/* Active Checkbox */}
          {/* <div className="form-control">
            <label className="label">
              <span className="label-text text-sm font-medium text-base-content">
                Active Only
              </span>
            </label>
            <div className="flex items-center justify-start h-10">
              <input
                type="checkbox"
                className="checkbox checkbox-primary"
                checked={filters.isActive}
                onChange={(e) => updateIsActive(e.target.checked)}
              />
            </div>
          </div> */}

          {/* Clear Button */}
          <div className="flex items-end">
            <button
              onClick={clearFilters}
              disabled={!hasActiveFilters()}
              className="btn btn-outline btn-error btn-sm gap-2 hover:btn-error hover:text-error-content transition-all duration-200 px-4 disabled:opacity-50 disabled:cursor-not-allowed h-10"
            >
              <Icon icon="mdi:refresh" className="w-4 h-4" />
              Clear
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};
