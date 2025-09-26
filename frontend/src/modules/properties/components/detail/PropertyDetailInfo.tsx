"use client";

import React from "react";
import { Icon } from "@iconify/react";
import { PropertyDto } from "../../types/property";

interface PropertyDetailInfoProps {
  property?: PropertyDto;
  isLoading?: boolean;
}

/**
 * Componente para mostrar la información detallada de una propiedad
 */
export const PropertyDetailInfo: React.FC<PropertyDetailInfoProps> = ({
  property,
  isLoading = false,
}) => {
  const formatPrice = (price: number) => {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
      minimumFractionDigits: 0,
      maximumFractionDigits: 0,
    }).format(price);
  };

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString("en-US", {
      weekday: "long",
      year: "numeric",
      month: "long",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  };

  if (isLoading) {
    return (
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Basic Information Skeleton */}
        <div className="bg-base-100 rounded-lg shadow-lg p-6 border border-base-300">
          <div className="skeleton h-6 w-1/3 mb-4"></div>
          <div className="space-y-3">
            <div className="skeleton h-4 w-full"></div>
            <div className="skeleton h-4 w-3/4"></div>
            <div className="skeleton h-4 w-1/2"></div>
          </div>
        </div>

        {/* Details Skeleton */}
        <div className="bg-base-100 rounded-lg shadow-lg p-6 border border-base-300">
          <div className="skeleton h-6 w-1/3 mb-4"></div>
          <div className="space-y-3">
            <div className="skeleton h-4 w-full"></div>
            <div className="skeleton h-4 w-3/4"></div>
            <div className="skeleton h-4 w-1/2"></div>
          </div>
        </div>
      </div>
    );
  }

  if (!property) {
    return (
      <div className="bg-base-100 rounded-lg shadow-lg p-6 border border-base-300 text-center">
        <Icon
          icon="mdi:alert-circle"
          className="w-16 h-16 mx-auto mb-4 text-warning"
        />
        <h3 className="text-lg font-semibold mb-2 text-gray-900 dark:text-base-content">
          Property Not Found
        </h3>
        <p className="text-gray-600 dark:text-base-content/70">
          The requested property could not be found or does not exist.
        </p>
      </div>
    );
  }

  return (
    <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
      {/* Basic Information */}
      <div className="bg-base-100 rounded-lg shadow-lg p-6 border border-base-300">
        <h3 className="text-xl font-semibold mb-4 flex items-center gap-2 text-gray-900 dark:text-base-content">
          <Icon icon="mdi:information" className="w-6 h-6 text-primary" />
          Basic Information
        </h3>

        <div className="space-y-4">
          {/* Property Name */}
          <div className="flex items-start gap-3">
            <Icon
              icon="mdi:home"
              className="w-5 h-5 text-base-content/60 mt-0.5"
            />
            <div>
              <label className="text-sm font-medium text-gray-600 dark:text-base-content/60">
                Property Name
              </label>
              <p className="text-base font-medium text-gray-900 dark:text-base-content">
                {property.name}
              </p>
            </div>
          </div>

          {/* Address */}
          <div className="flex items-start gap-3">
            <Icon
              icon="mdi:map-marker"
              className="w-5 h-5 text-base-content/60 mt-0.5"
            />
            <div>
              <label className="text-sm font-medium text-gray-600 dark:text-base-content/60">
                Address
              </label>
              <p className="text-base font-medium text-gray-900 dark:text-base-content">
                {property.address}
              </p>
            </div>
          </div>

          {/* Price */}
          <div className="flex items-start gap-3">
            <Icon
              icon="mdi:currency-usd"
              className="w-5 h-5 text-base-content/60 mt-0.5"
            />
            <div>
              <label className="text-sm font-medium text-gray-600 dark:text-base-content/60">
                Price
              </label>
              <p className="text-2xl font-bold text-primary">
                {formatPrice(property.price)}
              </p>
            </div>
          </div>

          {/* Status */}
          <div className="flex items-start gap-3">
            <Icon
              icon="mdi:check-circle"
              className="w-5 h-5 text-base-content/60 mt-0.5"
            />
            <div>
              <label className="text-sm font-medium text-gray-600 dark:text-base-content/60">
                Status
              </label>
              <div className="flex items-center gap-2 mt-1">
                <span
                  className={`px-3 py-1 text-sm font-medium rounded-full ${
                    property.isActive
                      ? "bg-success text-success-content"
                      : "bg-error text-error-content"
                  }`}
                >
                  {property.isActive ? "Active" : "Inactive"}
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Property Details */}
      <div className="bg-base-100 rounded-lg shadow-lg p-6 border border-base-300">
        <h3 className="text-xl font-semibold mb-4 flex items-center gap-2 text-gray-900 dark:text-base-content">
          <Icon icon="mdi:file-document" className="w-6 h-6 text-primary" />
          Property Details
        </h3>

        <div className="space-y-4">
          {/* Internal Code */}
          <div className="flex items-start gap-3">
            <Icon
              icon="mdi:barcode"
              className="w-5 h-5 text-base-content/60 mt-0.5"
            />
            <div>
              <label className="text-sm font-medium text-gray-600 dark:text-base-content/60">
                Internal Code
              </label>
              <p className="text-base font-medium font-mono bg-gray-100 dark:bg-base-200 px-2 py-1 rounded text-gray-900 dark:text-base-content">
                {property.codeInternal}
              </p>
            </div>
          </div>

          {/* Year Built */}
          <div className="flex items-start gap-3">
            <Icon
              icon="mdi:calendar"
              className="w-5 h-5 text-base-content/60 mt-0.5"
            />
            <div>
              <label className="text-sm font-medium text-gray-600 dark:text-base-content/60">
                Year Built
              </label>
              <p className="text-base font-medium text-gray-900 dark:text-base-content">
                {property.year}
              </p>
            </div>
          </div>

          {/* Owner ID */}
          <div className="flex items-start gap-3">
            <Icon
              icon="mdi:account"
              className="w-5 h-5 text-base-content/60 mt-0.5"
            />
            <div>
              <label className="text-sm font-medium text-gray-600 dark:text-base-content/60">
                Owner ID
              </label>
              <p className="text-base font-medium font-mono bg-gray-100 dark:bg-base-200 px-2 py-1 rounded text-gray-900 dark:text-base-content">
                {property.idOwner}
              </p>
            </div>
          </div>

          {/* Images Count */}
          <div className="flex items-start gap-3">
            <Icon
              icon="mdi:image-multiple"
              className="w-5 h-5 text-base-content/60 mt-0.5"
            />
            <div>
              <label className="text-sm font-medium text-gray-600 dark:text-base-content/60">
                Images
              </label>
              <p className="text-base font-medium text-gray-900 dark:text-base-content">
                {property.images.length} image
                {property.images.length !== 1 ? "s" : ""}
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* Timestamps */}
      <div className="lg:col-span-2">
        <div className="bg-base-100 rounded-lg shadow-lg p-6 border border-base-300">
          <h3 className="text-xl font-semibold mb-4 flex items-center gap-2 text-gray-900 dark:text-base-content">
            <Icon icon="mdi:clock" className="w-6 h-6 text-primary" />
            Timeline Information
          </h3>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {/* Created Date */}
            <div className="flex items-start gap-3">
              <Icon
                icon="mdi:plus-circle"
                className="w-5 h-5 text-success mt-0.5"
              />
              <div>
                <label className="text-sm font-medium text-gray-600 dark:text-base-content/60">
                  Created
                </label>
                <p className="text-base font-medium text-gray-900 dark:text-base-content">
                  {formatDate(property.createdAt)}
                </p>
              </div>
            </div>

            {/* Updated Date */}
            <div className="flex items-start gap-3">
              <Icon
                icon="mdi:pencil-circle"
                className="w-5 h-5 text-warning mt-0.5"
              />
              <div>
                <label className="text-sm font-medium text-gray-600 dark:text-base-content/60">
                  Last Updated
                </label>
                <p className="text-base font-medium text-gray-900 dark:text-base-content">
                  {formatDate(property.updatedAt)}
                </p>
                {property.updatedAt !== property.createdAt && (
                  <p className="text-sm text-gray-500 dark:text-base-content/60 mt-1">
                    (Modified after creation)
                  </p>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
