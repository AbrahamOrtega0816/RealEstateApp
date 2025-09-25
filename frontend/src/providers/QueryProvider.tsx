"use client";

import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { useState } from "react";

export default function QueryProvider({
  children,
}: {
  children: React.ReactNode;
}) {
  // Crear una nueva instancia del QueryClient para cada renderizado del componente
  // Esto evita problemas de hidratación en SSR
  const [queryClient] = useState(
    () =>
      new QueryClient({
        defaultOptions: {
          queries: {
            // Tiempo que los datos permanecen "frescos" antes de ser refetcheados
            staleTime: 5 * 60 * 1000, // 5 minutos
            // Tiempo que los datos permanecen en caché después de que no se usen
            gcTime: 10 * 60 * 1000, // 10 minutos (anteriormente cacheTime)
            // Reintentar automáticamente las consultas fallidas
            retry: 3,
            // Intervalo de reintento exponencial
            retryDelay: (attemptIndex) =>
              Math.min(1000 * 2 ** attemptIndex, 30000),
            // Refetch automático cuando la ventana obtiene el foco
            refetchOnWindowFocus: false,
            // Refetch automático cuando se reconecta la red
            refetchOnReconnect: true,
          },
          mutations: {
            // Reintentar mutaciones fallidas
            retry: 1,
            // Mostrar errores por defecto
            throwOnError: false,
          },
        },
      })
  );

  return (
    <QueryClientProvider client={queryClient}>
      {children}
      {/* Herramientas de desarrollo - solo en desarrollo */}
      {process.env.NODE_ENV === "development" && (
        <ReactQueryDevtools
          initialIsOpen={false}
          buttonPosition="bottom-right"
        />
      )}
    </QueryClientProvider>
  );
}
