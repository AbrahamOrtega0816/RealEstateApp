import { useApiQuery, useApiMutation, fetchApi } from "@/hooks/useApi";
import {
  OwnerDto,
  CreateOwnerDto,
  PagedOwnerResult,
} from "@/modules/owners/types/owner";
import { useQuery } from "@tanstack/react-query";

/**
 * Hook para obtener la lista de propietarios con paginación
 */
export const useGetOwners = (params: {
  pageNumber: number;
  pageSize: number;
}) => {
  return useQuery({
    queryKey: ["owners", params],
    queryFn: () =>
      fetchApi(
        `/owners?pageNumber=${params.pageNumber}&pageSize=${params.pageSize}`
      ),
  });
};

/**
 * Hook para obtener un propietario por ID
 */
export const useGetOwnerById = (id: string) => {
  return useApiQuery<OwnerDto>(["owner", id], `/owners/${id}`, {
    enabled: !!id, // Solo ejecutar si hay un ID
    staleTime: 30000,
    gcTime: 300000,
  });
};

/**
 * Hook para crear un nuevo propietario
 */
export const useCreateOwner = () => {
  return useApiMutation<OwnerDto, CreateOwnerDto>("/owners", "POST", {
    onSuccess: (data) => {
      console.log("Propietario creado exitosamente:", data);
    },
    onError: (error) => {
      console.error("Error al crear propietario:", error.message);
    },
  });
};

/**
 * Hook para actualizar un propietario
 */
export const useUpdateOwner = (id: string) => {
  return useApiMutation<OwnerDto, CreateOwnerDto>(`/owners/${id}`, "PUT", {
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
export const useDeleteOwner = (id: string) => {
  return useApiMutation<void, string>(`/owners/${id}`, "DELETE", {
    onSuccess: () => {
      console.log("Propietario eliminado exitosamente");
    },
    onError: (error) => {
      console.error("Error al eliminar propietario:", error.message);
    },
  });
};
