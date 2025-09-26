"use client";

import React, { useState } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/modules/login/hooks/useAuth";
import {
  useGetProperties,
  useDeleteProperty,
} from "../services/propertyService";
import { PropertyContainer, CreatePropertyModal } from "../components";
import { PropertyDto, PropertyFilterDto } from "../types/property";
import { toast } from "react-hot-toast";

/**
 * Página principal del módulo de propiedades
 * Muestra la lista de propiedades y permite gestionarlas
 */
const PropertiesPage: React.FC = () => {
  const { user } = useAuth();
  const router = useRouter();
  const [currentPage, setCurrentPage] = useState(1);
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [currentFilter, setCurrentFilter] = useState<PropertyFilterDto>({
    pageNumber: 1,
    pageSize: 6,
  });

  // Fetch properties data
  const { data, isLoading, refetch } = useGetProperties(currentFilter);
  const deletePropertyMutation = useDeleteProperty();

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
    setCurrentFilter((prev) => ({
      ...prev,
      pageNumber: page,
    }));
  };

  const handleFilter = (filter: PropertyFilterDto) => {
    setCurrentFilter(filter);
    setCurrentPage(filter.pageNumber || 1);
  };

  const handleDeleteProperty = async (propertyId: string) => {
    if (
      confirm(
        "Are you sure you want to delete this property? This action cannot be undone."
      )
    ) {
      try {
        await deletePropertyMutation.mutateAsync(propertyId);
        toast.success("Property deleted successfully");
        refetch();
      } catch (error) {
        console.error("Error deleting property:", error);
        toast.error("Error deleting property. Please try again.");
      }
    }
  };

  const handleEditProperty = (property: PropertyDto) => {
    // TODO: Implement edit functionality
    toast("Edit functionality will be implemented soon", { icon: "ℹ️" });
    console.log("Edit property:", property);
  };

  const handleAddProperty = () => {
    setIsCreateModalOpen(true);
  };

  const handleCreateSuccess = () => {
    refetch();
  };

  const handleChangePrice = (propertyId: string, currentPrice: number) => {
    // TODO: Implement price change functionality
    toast("Change price functionality will be implemented soon", {
      icon: "ℹ️",
    });
    console.log(
      "Change price for property:",
      propertyId,
      "current price:",
      currentPrice
    );
  };

  const handleViewDetails = (propertyId: string) => {
    router.push(`/properties/${propertyId}`);
  };

  return (
    <div className="p-6">
      <PropertyContainer
        properties={data?.items || []}
        isLoading={isLoading}
        currentPage={
          Number.isFinite(data?.pageNumber) && data?.pageNumber
            ? data.pageNumber
            : currentPage
        }
        totalPages={
          Number.isFinite(data?.totalPages) && data?.totalPages
            ? data.totalPages
            : 1
        }
        onPageChange={handlePageChange}
        onFilter={handleFilter}
        onAdd={handleAddProperty}
        onEdit={handleEditProperty}
        onDelete={handleDeleteProperty}
        onChangePrice={handleChangePrice}
        onViewDetails={handleViewDetails}
      />

      <CreatePropertyModal
        isOpen={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
        onSuccess={handleCreateSuccess}
      />
    </div>
  );
};

export default PropertiesPage;
