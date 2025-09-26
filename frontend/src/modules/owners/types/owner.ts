/**
 * Tipos TypeScript para el módulo de owners
 */

/**
 * DTO para transferir información de propietario
 */
export interface OwnerDto {
  id: string;
  name: string;
  address: string;
  photo: string;
  birthday: string;
  createdAt: string;
  updatedAt: string;
  isActive: boolean;
}

/**
 * DTO para crear nuevos propietarios
 */
export interface CreateOwnerDto {
  name: string;
  address: string;
  photo: string;
  birthday: string;
}

/**
 * Resultado paginado para owners
 */
export interface PagedOwnerResult {
  items: OwnerDto[];
  totalItems: number;
  currentPage: number;
  totalPages: number;
  pageSize: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}
