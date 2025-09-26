import React from "react";

/**
 * Componente que renderiza el encabezado de la tabla de propietarios
 */
export const OwnerHeaderTable: React.FC = () => {
  return (
    <thead className="bg-base-200">
      <tr>
        <th
          scope="col"
          className="px-6 py-3 text-left text-xs font-medium text-base-content/70 uppercase tracking-wider"
        >
          Photo
        </th>
        <th
          scope="col"
          className="px-6 py-3 text-left text-xs font-medium text-base-content/70 uppercase tracking-wider"
        >
          Name
        </th>
        <th
          scope="col"
          className="px-6 py-3 text-left text-xs font-medium text-base-content/70 uppercase tracking-wider"
        >
          Address
        </th>
        <th
          scope="col"
          className="px-6 py-3 text-left text-xs font-medium text-base-content/70 uppercase tracking-wider"
        >
          Birthday
        </th>
        <th
          scope="col"
          className="px-6 py-3 text-left text-xs font-medium text-base-content/70 uppercase tracking-wider"
        >
          Status
        </th>
        <th
          scope="col"
          className="px-6 py-3 text-left text-xs font-medium text-base-content/70 uppercase tracking-wider"
        >
          Actions
        </th>
      </tr>
    </thead>
  );
};
