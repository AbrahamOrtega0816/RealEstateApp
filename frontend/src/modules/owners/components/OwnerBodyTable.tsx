import React from "react";
import { Icon } from "@iconify/react";
import { OwnerDto } from "@/modules/owners/types/owner";
import { OwnerRowTable } from "./OwnerRowTable";

interface OwnerBodyTableProps {
  owners: OwnerDto[];
  isLoading?: boolean;
  onDelete?: (ownerId: string) => void;
}

/**
 * Componente que renderiza el cuerpo de la tabla de propietarios
 */
export const OwnerBodyTable: React.FC<OwnerBodyTableProps> = ({
  owners,
  isLoading,
  onDelete,
}) => {
  if (isLoading) {
    return (
      <tbody className="bg-base-100 divide-y divide-base-300">
        {[...Array(5)].map((_, index) => (
          <tr key={index}>
            <td colSpan={6} className="px-6 py-4">
              <div className="animate-pulse flex items-center space-x-4">
                <div className="rounded-full bg-base-300 h-10 w-10"></div>
                <div className="flex-1 space-y-2 py-1">
                  <div className="h-4 bg-base-300 rounded w-3/4"></div>
                  <div className="h-4 bg-base-300 rounded w-1/2"></div>
                </div>
              </div>
            </td>
          </tr>
        ))}
      </tbody>
    );
  }

  if (owners.length === 0) {
    return (
      <tbody className="bg-base-100">
        <tr>
          <td
            colSpan={6}
            className="px-6 py-12 text-center text-base-content/60"
          >
            <div className="flex flex-col items-center">
              <Icon
                icon="mdi:inbox-outline"
                className="w-12 h-12 mb-4 text-base-content/40"
              />
              <p className="text-lg font-medium text-base-content">
                No owners registered
              </p>
              <p className="text-sm mt-1 text-base-content/60">
                Start by adding a new owner
              </p>
            </div>
          </td>
        </tr>
      </tbody>
    );
  }

  return (
    <tbody className="bg-base-100 divide-y divide-base-300">
      {owners.map((owner) => (
        <OwnerRowTable key={owner.id} owner={owner} onDelete={onDelete} />
      ))}
    </tbody>
  );
};
