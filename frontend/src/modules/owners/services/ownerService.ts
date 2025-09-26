import { useQuery, useMutation } from "@tanstack/react-query";
import { api } from "@/lib/axios";
import {
  OwnerDto,
  CreateOwnerDto,
  PagedOwnerResult,
} from "@/modules/owners/types/owner";

/**
 * Hook para obtener la lista de propietarios con paginación
 */
export const useGetOwners = (params: {
  pageNumber: number;
  pageSize: number;
}) => {
  return useQuery<PagedOwnerResult>({
    queryKey: ["owners", params],
    queryFn: async () =>
      await api.get("/owners", {
        params: {
          pageNumber: params.pageNumber,
          pageSize: params.pageSize,
        },
      }),
  });
};

/**
 * Hook para obtener un propietario por ID
 */
export const useGetOwnerById = (id: string) => {
  return useQuery<OwnerDto>({
    queryKey: ["owner", id],
    queryFn: async () => await api.get(`/owners/${id}`),
    enabled: !!id, // Solo ejecutar si hay un ID
    staleTime: 30000,
    gcTime: 300000,
  });
};

/**
 * Hook para crear un nuevo propietario
 */
export const useCreateOwner = () => {
  return useMutation<OwnerDto, Error, FormData>({
    mutationFn: async (formData: FormData) =>
      await api.post("/owners", formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      }),
    mutationKey: ["useCreateOwner"],
  });
};

/**
 * Hook para actualizar un propietario
 */
export const useUpdateOwner = (id: string) => {
  return useMutation<OwnerDto, Error, CreateOwnerDto>({
    mutationFn: async (ownerData: CreateOwnerDto) =>
      await api.put(`/owners/${id}`, ownerData),
    onSuccess: (data) => {
      console.log("Propietario actualizado exitosamente:", data);
    },
    onError: (error) => {
      console.error("Error al actualizar propietario:", error.message);
    },
  });
};

/**
 * Hook para eliminar un propietario
 */
export const useDeleteOwner = () => {
  return useMutation<void, Error, string>({
    mutationFn: async (id: string) => await api.delete(`/owners/${id}`),
  });
};
