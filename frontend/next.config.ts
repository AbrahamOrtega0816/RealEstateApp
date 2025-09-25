import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  // Deshabilitar todos los source maps
  productionBrowserSourceMaps: false,

  // Configuración experimental para deshabilitar source maps
  experimental: {
    serverSourceMaps: false,
  },

  // Configuración de webpack para deshabilitar source maps completamente
  webpack: (config, { dev, isServer }) => {
    // Deshabilitar source maps en todos los entornos
    config.devtool = false;

    // Asegurar que no se generen source maps
    if (config.optimization) {
      config.optimization.minimize = dev ? false : config.optimization.minimize;
    }

    return config;
  },
};

export default nextConfig;
