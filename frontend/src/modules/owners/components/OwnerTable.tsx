import React from "react";
import { Icon } from "@iconify/react";
import { OwnerHeaderTable } from "./OwnerHeaderTable";
import { OwnerBodyTable } from "./OwnerBodyTable";
import { Pagination } from "@/components/ui/Pagination";
import { OwnerDto } from "@/modules/owners/types/owner";

interface OwnerTableProps {
  owners: OwnerDto[];
  isLoading?: boolean;
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
  onDelete?: (ownerId: string) => void;
  onAdd?: () => void;
}

/**
 * Componente contenedor de la tabla de propietarios
 * Integra el header, body y paginación
 */
export const OwnerTable: React.FC<OwnerTableProps> = ({
  owners,
  isLoading,
  currentPage,
  totalPages,
  onPageChange,
  onDelete,
  onAdd,
}) => {
  return (
    <div className="flex flex-col">
      {/* Header con título y botón de agregar */}
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-2xl font-bold text-base-content">Owners List</h2>
        <button className="btn btn-dash btn-primary" onClick={onAdd}>
          {" "}
          <Icon icon="mdi:plus" className="w-5 h-5" />
          Add Owner
        </button>
      </div>

      {/* Tabla */}
      <div className="shadow-lg overflow-hidden border border-base-300 rounded-lg bg-base-100">
        <table className="min-w-full divide-y divide-base-300">
          <OwnerHeaderTable />
          <OwnerBodyTable
            owners={owners}
            isLoading={isLoading}
            onDelete={onDelete}
          />
        </table>
      </div>

      {/* Pagination */}
      <Pagination
        currentPage={currentPage}
        totalPages={totalPages}
        onPageChange={onPageChange}
      />
    </div>
  );
};
