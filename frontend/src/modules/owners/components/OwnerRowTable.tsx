import React from "react";
import { Icon } from "@iconify/react";
import { OwnerDto } from "@/modules/owners/types/owner";
import Button from "@/components/ui/Button";

interface OwnerRowTableProps {
  owner: OwnerDto;
  onDelete?: (ownerId: string) => void;
}

/**
 * Componente que renderiza una fila de la tabla de propietarios
 */
export const OwnerRowTable: React.FC<OwnerRowTableProps> = ({
  owner,
  onDelete,
}) => {
  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString("en-US", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
    });
  };

  return (
    <tr className="hover:bg-base-200 transition-colors">
      <td className="px-6 py-4 whitespace-nowrap">
        <div className="flex items-center">
          <div className="h-10 w-10 flex-shrink-0">
            {owner.photo ? (
              <img
                className="h-10 w-10 rounded-full object-cover"
                src={owner.photo}
                alt={owner.name}
                onError={(e) => {
                  e.currentTarget.src = `https://ui-avatars.com/api/?name=${encodeURIComponent(
                    owner.name
                  )}&background=random`;
                }}
              />
            ) : (
              <div className="h-10 w-10 rounded-full bg-base-300 flex items-center justify-center">
                <span className="text-base-content font-medium">
                  {owner.name.charAt(0).toUpperCase()}
                </span>
              </div>
            )}
          </div>
        </div>
      </td>
      <td className="px-6 py-4 whitespace-nowrap">
        <div className="text-sm font-medium text-base-content">
          {owner.name}
        </div>
      </td>
      <td className="px-6 py-4 whitespace-nowrap">
        <div className="text-sm text-base-content/70">{owner.address}</div>
      </td>
      <td className="px-6 py-4 whitespace-nowrap">
        <div className="text-sm text-base-content/70">
          {formatDate(owner.birthday)}
        </div>
      </td>
      <td className="px-6 py-4 whitespace-nowrap">
        <div className="flex items-center gap-2">
          <Icon
            icon={owner.isActive ? "mdi:check-circle" : "mdi:close-circle"}
            className={`w-4 h-4 ${
              owner.isActive ? "text-success" : "text-error"
            }`}
          />
          <span
            className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
              owner.isActive
                ? "bg-success/10 text-success"
                : "bg-error/10 text-error"
            }`}
          >
            {owner.isActive ? "Active" : "Inactive"}
          </span>
        </div>
      </td>
      <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
        <Button
          variant="ghost"
          size="sm"
          onClick={() => onDelete?.(owner.id)}
          className="text-error hover:text-error/80 flex items-center gap-2"
        >
          <Icon icon="mdi:delete-outline" className="w-4 h-4" />
          Delete
        </Button>
      </td>
    </tr>
  );
};
