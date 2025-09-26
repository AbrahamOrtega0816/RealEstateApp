import { useQuery, useMutation, UseQueryOptions } from "@tanstack/react-query";
import { api } from "@/lib/axios";
import {
  PropertyDto,
  CreatePropertyDto,
  UpdatePropertyDto,
  PropertyFilterDto,
  PagedPropertyResult,
} from "../types/property";

/**
 * Hook para obtener propiedades con paginación
 */
export const useGetProperties = (
  filter: PropertyFilterDto = {},
  options?: Omit<UseQueryOptions<PagedPropertyResult>, "queryKey" | "queryFn">
) => {
  return useQuery<PagedPropertyResult>({
    queryKey: ["properties", filter],
    queryFn: async () => await api.get("/properties", { params: filter }),
    ...options,
  });
};

/**
 * Hook para obtener una propiedad por ID
 */
export const useGetPropertyById = (
  id: string,
  options?: Omit<UseQueryOptions<PropertyDto>, "queryKey" | "queryFn">
) => {
  return useQuery<PropertyDto>({
    queryKey: ["property", id],
    queryFn: async () => await api.get(`/properties/${id}`),
    enabled: !!id,
    ...options,
  });
};

/**
 * Hook para crear propiedades
 */
export const useCreateProperty = () => {
  return useMutation({
    mutationFn: async (data: FormData) =>
      await api.post("/properties", data, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      }),
  });
};

/**
 * Hook para actualizar propiedades
 */
export const useUpdateProperty = () => {
  return useMutation({
    mutationFn: async ({ id, data }: { id: string; data: UpdatePropertyDto }) =>
      await api.put(`/properties/${id}`, data),
  });
};

/**
 * Hook para eliminar propiedades
 */
export const useDeleteProperty = () => {
  return useMutation({
    mutationFn: async (id: string) => await api.delete(`/properties/${id}`),
  });
};

/**
 * Hook para cambiar precio de propiedad
 */
export const useChangePropertyPrice = () => {
  return useMutation({
    mutationFn: async ({ id, newPrice }: { id: string; newPrice: number }) =>
      await api.patch(`/properties/${id}/price`, newPrice),
  });
};
