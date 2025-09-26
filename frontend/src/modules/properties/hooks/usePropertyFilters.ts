import { useState, useCallback, useEffect } from "react";
import { PropertyFilterDto } from "../types/property";

export interface PropertyFiltersState {
  searchTerm: string;
  minPrice: string;
  maxPrice: string;
  selectedOwner: string;
  selectedYear: string;
  isActive: boolean;
}

export interface UsePropertyFiltersProps {
  onFilter?: (filter: PropertyFilterDto) => void;
  debounceMs?: number;
}

export const usePropertyFilters = ({
  onFilter,
  debounceMs = 1000,
}: UsePropertyFiltersProps) => {
  // Filter states
  const [filters, setFilters] = useState<PropertyFiltersState>({
    searchTerm: "",
    minPrice: "",
    maxPrice: "",
    selectedOwner: "",
    selectedYear: "",
    isActive: true, // Default to active properties
  });

  // Debounced filter function
  const debouncedFilter = useCallback(
    (filterState: PropertyFiltersState) => {
      if (onFilter) {
        const filter: PropertyFilterDto = {
          pageNumber: 1,
          pageSize: 12,
          isActive: filterState.isActive,
        };

        if (filterState.searchTerm.trim()) {
          filter.name = filterState.searchTerm.trim();
        }

        if (filterState.minPrice && !isNaN(Number(filterState.minPrice))) {
          filter.minPrice = Number(filterState.minPrice);
        }

        if (filterState.maxPrice && !isNaN(Number(filterState.maxPrice))) {
          filter.maxPrice = Number(filterState.maxPrice);
        }

        if (filterState.selectedOwner) {
          filter.idOwner = filterState.selectedOwner;
        }

        if (
          filterState.selectedYear &&
          !isNaN(Number(filterState.selectedYear))
        ) {
          filter.year = Number(filterState.selectedYear);
        }

        // isActive is already set above in the filter object

        onFilter(filter);
      }
    },
    [onFilter]
  );

  // Debounce effect
  useEffect(() => {
    const timer = setTimeout(() => {
      debouncedFilter(filters);
    }, debounceMs);

    return () => clearTimeout(timer);
  }, [filters, debouncedFilter, debounceMs]);

  // Filter update functions
  const updateSearchTerm = useCallback((value: string) => {
    setFilters((prev) => ({ ...prev, searchTerm: value }));
  }, []);

  const updateMinPrice = useCallback((value: string) => {
    setFilters((prev) => ({ ...prev, minPrice: value }));
  }, []);

  const updateMaxPrice = useCallback((value: string) => {
    setFilters((prev) => ({ ...prev, maxPrice: value }));
  }, []);

  const updateSelectedOwner = useCallback((value: string) => {
    setFilters((prev) => ({ ...prev, selectedOwner: value }));
  }, []);

  const updateSelectedYear = useCallback((value: string) => {
    setFilters((prev) => ({ ...prev, selectedYear: value }));
  }, []);

  const updateIsActive = useCallback((value: boolean) => {
    setFilters((prev) => ({ ...prev, isActive: value }));
  }, []);

  // Clear all filters
  const clearFilters = useCallback(() => {
    setFilters({
      searchTerm: "",
      minPrice: "",
      maxPrice: "",
      selectedOwner: "",
      selectedYear: "",
      isActive: true, // Reset to default active
    });
  }, []);

  // Get current filter values
  const getFilterValues = useCallback(() => filters, [filters]);

  // Check if any filters are active (excluding isActive since it's default)
  const hasActiveFilters = useCallback(() => {
    return !!(
      filters.searchTerm ||
      filters.minPrice ||
      filters.maxPrice ||
      filters.selectedOwner ||
      filters.selectedYear
    );
  }, [filters]);

  return {
    // Current filter state
    filters,

    // Update functions
    updateSearchTerm,
    updateMinPrice,
    updateMaxPrice,
    updateSelectedOwner,
    updateSelectedYear,
    updateIsActive,

    // Utility functions
    clearFilters,
    getFilterValues,
    hasActiveFilters,
  };
};
