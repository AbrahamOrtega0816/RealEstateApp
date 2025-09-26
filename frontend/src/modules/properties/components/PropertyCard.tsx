"use client";

import React, { useState } from "react";
import { Icon } from "@iconify/react";
import { PropertyDto } from "../types/property";

interface PropertyCardProps {
  property: PropertyDto;
  onEdit?: (property: PropertyDto) => void;
  onDelete?: (propertyId: string) => void;
  onChangePrice?: (propertyId: string, currentPrice: number) => void;
  onViewDetails?: (propertyId: string) => void;
}

/**
 * Componente que renderiza una tarjeta de propiedad
 */
export const PropertyCard: React.FC<PropertyCardProps> = ({
  property,
  onEdit,
  onDelete,
  onChangePrice,
  onViewDetails,
}) => {
  const [currentImageIndex, setCurrentImageIndex] = useState(0);

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
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
    });
  };

  const handlePreviousImage = () => {
    setCurrentImageIndex((prev) =>
      prev === 0 ? property.images.length - 1 : prev - 1
    );
  };

  const handleNextImage = () => {
    setCurrentImageIndex((prev) =>
      prev === property.images.length - 1 ? 0 : prev + 1
    );
  };

  const defaultImage = `https://ui-avatars.com/api/?name=${encodeURIComponent(
    property.name
  )}&background=e5e7eb&color=6b7280&size=400&format=svg`;

  return (
    <div className="bg-base-100 rounded-lg shadow-lg overflow-hidden border border-base-300 hover:shadow-xl transition-shadow duration-300">
      {/* Image Section */}
      <div className="relative h-48 bg-base-200">
        {property.images.length > 0 ? (
          <>
            <img
              src={property.images[currentImageIndex] || defaultImage}
              alt={property.name}
              className="w-full h-full object-cover"
              onError={(e) => {
                e.currentTarget.src = defaultImage;
              }}
            />

            {/* Image Navigation */}
            {property.images.length > 1 && (
              <>
                <button
                  onClick={handlePreviousImage}
                  className="absolute left-2 top-1/2 transform -translate-y-1/2 bg-black/50 text-white rounded-full p-1 hover:bg-black/70 transition-colors"
                >
                  <Icon icon="mdi:chevron-left" className="w-5 h-5" />
                </button>
                <button
                  onClick={handleNextImage}
                  className="absolute right-2 top-1/2 transform -translate-y-1/2 bg-black/50 text-white rounded-full p-1 hover:bg-black/70 transition-colors"
                >
                  <Icon icon="mdi:chevron-right" className="w-5 h-5" />
                </button>

                {/* Image Indicators */}
                <div className="absolute bottom-2 left-1/2 transform -translate-x-1/2 flex space-x-1">
                  {property.images.map((_, index) => (
                    <button
                      key={index}
                      onClick={() => setCurrentImageIndex(index)}
                      className={`w-2 h-2 rounded-full transition-colors ${
                        index === currentImageIndex ? "bg-white" : "bg-white/50"
                      }`}
                    />
                  ))}
                </div>
              </>
            )}
          </>
        ) : (
          <div className="w-full h-full flex items-center justify-center">
            <Icon
              icon="mdi:home-outline"
              className="w-16 h-16 text-base-content/30"
            />
          </div>
        )}

        {/* Status Badge */}
        <div className="absolute top-2 left-2">
          <span
            className={`px-2 py-1 text-xs font-medium rounded-full ${
              property.isActive
                ? "bg-success text-success-content"
                : "bg-error text-error-content"
            }`}
          >
            {property.isActive ? "Active" : "Inactive"}
          </span>
        </div>

        {/* Action Menu */}
        <div className="absolute top-2 right-2">
          <div className="dropdown dropdown-end">
            <div
              tabIndex={0}
              role="button"
              className="btn btn-ghost btn-sm btn-circle bg-black/50 text-white hover:bg-black/70"
            >
              <Icon icon="mdi:dots-vertical" className="w-4 h-4" />
            </div>
            <ul
              tabIndex={0}
              className="dropdown-content menu bg-base-100 rounded-box z-[1] w-52 p-2 shadow-lg border border-base-300"
            >
              {onViewDetails && (
                <li>
                  <button
                    onClick={() => onViewDetails(property.id)}
                    className="flex items-center gap-2 text-sm text-base-content hover:bg-base-200 hover:text-base-content"
                  >
                    <Icon icon="mdi:eye" className="w-4 h-4" />
                    View Details
                  </button>
                </li>
              )}
              {onDelete && (
                <li>
                  <button
                    onClick={() => onDelete(property.id)}
                    className="flex items-center gap-2 text-sm text-error hover:text-error"
                  >
                    <Icon icon="mdi:delete" className="w-4 h-4" />
                    Delete Property
                  </button>
                </li>
              )}
            </ul>
          </div>
        </div>
      </div>

      {/* Content Section */}
      <div className="p-4">
        {/* Title and Price */}
        <div className="flex justify-between items-start mb-2">
          <h3 className="text-lg font-semibold text-base-content truncate mr-2">
            {property.name}
          </h3>
          <span className="text-xl font-bold text-primary whitespace-nowrap">
            {formatPrice(property.price)}
          </span>
        </div>

        {/* Address */}
        <div className="flex items-center gap-1 mb-3">
          <Icon
            icon="mdi:map-marker"
            className="w-4 h-4 text-base-content/60 flex-shrink-0"
          />
          <p className="text-sm text-base-content/70 truncate">
            {property.address}
          </p>
        </div>

        {/* Details Grid */}
        <div className="grid grid-cols-2 gap-3 mb-3">
          <div className="flex items-center gap-1">
            <Icon
              icon="mdi:calendar"
              className="w-4 h-4 text-base-content/60"
            />
            <span className="text-sm text-base-content/70">
              {property.year}
            </span>
          </div>
          <div className="flex items-center gap-1">
            <Icon icon="mdi:barcode" className="w-4 h-4 text-base-content/60" />
            <span className="text-sm text-base-content/70 truncate">
              {property.codeInternal}
            </span>
          </div>
        </div>

        {/* Footer */}
        <div className="flex justify-between items-center pt-3 border-t border-base-300">
          <div className="text-xs text-base-content/50">
            Created: {formatDate(property.createdAt)}
          </div>
          {property.updatedAt !== property.createdAt && (
            <div className="text-xs text-base-content/50">
              Updated: {formatDate(property.updatedAt)}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};
