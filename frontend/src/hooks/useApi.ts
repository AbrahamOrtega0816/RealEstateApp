import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";

// Configuración base de la API
const API_BASE_URL =
  process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000/api";

// Función helper para realizar peticiones HTTP
const fetchApi = async (endpoint: string, options?: RequestInit) => {
  const url = `${API_BASE_URL}${endpoint}`;

  const response = await fetch(url, {
    headers: {
      "Content-Type": "application/json",
      ...options?.headers,
    },
    ...options,
  });

  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    throw new Error(
      errorData.message || `Error ${response.status}: ${response.statusText}`
    );
  }

  return response.json();
};

// Hook para realizar peticiones GET con React Query
export const useApiQuery = <TData = unknown>(
  key: string | string[],
  endpoint: string,
  options?: {
    enabled?: boolean;
    staleTime?: number;
    gcTime?: number;
    retry?: number;
  }
) => {
  return useQuery<TData>({
    queryKey: Array.isArray(key) ? key : [key],
    queryFn: () => fetchApi(endpoint),
    ...options,
  });
};

// Hook para realizar mutaciones (POST, PUT, DELETE, PATCH)
export const useApiMutation = <TData = unknown, TVariables = unknown>(
  endpoint: string,
  method: "POST" | "PUT" | "DELETE" | "PATCH" = "POST",
  options?: {
    onSuccess?: (data: TData, variables: TVariables) => void;
    onError?: (error: Error, variables: TVariables) => void;
    invalidateQueries?: string | string[];
  }
) => {
  const queryClient = useQueryClient();

  return useMutation<TData, Error, TVariables>({
    mutationFn: (variables: TVariables) =>
      fetchApi(endpoint, {
        method,
        body: JSON.stringify(variables),
      }),
    onSuccess: (data, variables) => {
      // Invalidar queries relacionadas si se especifica
      if (options?.invalidateQueries) {
        const queryKey = Array.isArray(options.invalidateQueries)
          ? options.invalidateQueries
          : [options.invalidateQueries];
        queryClient.invalidateQueries({ queryKey });
      }

      options?.onSuccess?.(data, variables);
    },
    onError: options?.onError,
  });
};

// Hook para invalidar queries manualmente
export const useInvalidateQuery = () => {
  const queryClient = useQueryClient();

  return (key: string | string[]) => {
    const queryKey = Array.isArray(key) ? key : [key];
    queryClient.invalidateQueries({ queryKey });
  };
};

// Hook para obtener datos del cache sin hacer una nueva petición
export const useQueryData = <TData = unknown>(key: string | string[]) => {
  const queryClient = useQueryClient();
  const queryKey = Array.isArray(key) ? key : [key];

  return queryClient.getQueryData<TData>(queryKey);
};
