import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  // Deshabilitar source maps para evitar problemas con Bun
  productionBrowserSourceMaps: false,

  // Configuración de webpack para deshabilitar source maps
  webpack: (config, { dev, isServer }) => {
    if (dev) {
      // Deshabilitar source maps en desarrollo para evitar el error
      config.devtool = false;
    }
    return config;
  },
};

export default nextConfig;
