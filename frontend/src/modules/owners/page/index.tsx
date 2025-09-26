"use client";

import React, { useState } from "react";
import { useAuth } from "@/modules/login/hooks/useAuth";
import {
  useGetOwners,
  useDeleteOwner,
} from "@/modules/owners/services/ownerService";
import { OwnerTable, CreateOwnerModal } from "@/modules/owners/components";
import { toast } from "react-hot-toast";

/**
 * Main page for owners module
 * Displays the list of owners and allows management
 */
const OwnerPage: React.FC = () => {
  const { user } = useAuth();
  const [currentPage, setCurrentPage] = useState(1);
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const pageSize = 10;

  // Fetch owners data
  const { data, isLoading, refetch } = useGetOwners({
    pageNumber: currentPage,
    pageSize: pageSize,
  });
  const deleteOwnerMutation = useDeleteOwner();

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
  };

  const handleDeleteOwner = async (ownerId: string) => {
    if (confirm("Are you sure you want to delete this owner?")) {
      try {
        await deleteOwnerMutation.mutateAsync(ownerId);
        toast.success("Owner deleted successfully");
        refetch();
      } catch (error) {
        toast.error("Error deleting owner");
      }
    }
  };

  const handleAddOwner = () => {
    setIsCreateModalOpen(true);
  };

  const handleCreateSuccess = () => {
    refetch();
  };

  return (
    <div className="p-6">
      <OwnerTable
        owners={data?.items || []}
        isLoading={isLoading}
        currentPage={currentPage}
        totalPages={data?.totalPages || 1}
        onPageChange={handlePageChange}
        onDelete={handleDeleteOwner}
        onAdd={handleAddOwner}
      />

      <CreateOwnerModal
        isOpen={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
        onSuccess={handleCreateSuccess}
      />
    </div>
  );
};

export default OwnerPage;
