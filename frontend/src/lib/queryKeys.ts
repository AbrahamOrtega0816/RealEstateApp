// Claves de consulta centralizadas para React Query
// Esto ayuda a mantener consistencia y evita errores tipográficos

export const queryKeys = {
  // Autenticación
  auth: {
    user: ["auth", "user"] as const,
    profile: ["auth", "profile"] as const,
  },

  // Propiedades
  properties: {
    all: ["properties"] as const,
    list: (filters?: Record<string, any>) =>
      ["properties", "list", filters] as const,
    detail: (id: string) => ["properties", "detail", id] as const,
    byOwner: (ownerId: string) => ["properties", "byOwner", ownerId] as const,
  },

  // Propietarios
  owners: {
    all: ["owners"] as const,
    list: (filters?: Record<string, any>) =>
      ["owners", "list", filters] as const,
    detail: (id: string) => ["owners", "detail", id] as const,
  },

  // Trazas de propiedades
  propertyTraces: {
    all: ["propertyTraces"] as const,
    list: (filters?: Record<string, any>) =>
      ["propertyTraces", "list", filters] as const,
    detail: (id: string) => ["propertyTraces", "detail", id] as const,
    byProperty: (propertyId: string) =>
      ["propertyTraces", "byProperty", propertyId] as const,
  },
} as const;

// Tipos para las claves de consulta
export type QueryKeys = typeof queryKeys;

// Helper para crear claves de consulta dinámicas
export const createQueryKey = (
  base: string[],
  params?: Record<string, any>
) => {
  if (!params) return base;
  return [...base, params] as const;
};
