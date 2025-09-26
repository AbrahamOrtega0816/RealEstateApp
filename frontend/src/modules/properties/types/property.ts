/**
 * Tipos TypeScript para el módulo de propiedades
 */

/**
 * DTO para transferir información de propiedades
 */
export interface PropertyDto {
  id: string;
  idOwner: string;
  name: string;
  address: string;
  price: number;
  images: string[];
  codeInternal: string;
  year: number;
  createdAt: string;
  updatedAt: string;
  isActive: boolean;
}

/**
 * DTO para crear nuevas propiedades
 */
export interface CreatePropertyDto {
  idOwner: string;
  name: string;
  address: string;
  price: number;
  codeInternal: string;
  year: number;
  images?: FileList;
}

/**
 * DTO para actualizar propiedades
 */
export interface UpdatePropertyDto {
  name?: string;
  address?: string;
  price?: number;
  codeInternal?: string;
  year?: number;
  images?: FileList;
}

/**
 * DTO para filtrar propiedades
 */
export interface PropertyFilterDto {
  pageNumber?: number;
  pageSize?: number;
  name?: string;
  address?: string;
  minPrice?: number;
  maxPrice?: number;
  year?: number;
  idOwner?: string;
  isActive?: boolean;
  sortBy?: string;
  sortDirection?: string;
}

/**
 * Resultado paginado para properties
 * Matches backend PagedResultDto<PropertyDto> structure
 */
export interface PagedPropertyResult {
  items: PropertyDto[];
  totalCount: number;
  pageNumber: number;
  totalPages: number;
  pageSize: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}
