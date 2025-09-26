"use client";

import React, { useState } from "react";
import { Icon } from "@iconify/react";

interface PropertyDetailImagesProps {
  images: string[];
  propertyName: string;
  isLoading?: boolean;
}

/**
 * Componente para mostrar la galería de imágenes de una propiedad
 * Incluye navegación entre imágenes y vista modal
 */
export const PropertyDetailImages: React.FC<PropertyDetailImagesProps> = ({
  images,
  propertyName,
  isLoading = false,
}) => {
  const [currentImageIndex, setCurrentImageIndex] = useState(0);
  const [isModalOpen, setIsModalOpen] = useState(false);

  const defaultImage = `https://ui-avatars.com/api/?name=${encodeURIComponent(
    propertyName
  )}&background=e5e7eb&color=6b7280&size=800&format=svg`;

  const handlePreviousImage = () => {
    setCurrentImageIndex((prev) => (prev === 0 ? images.length - 1 : prev - 1));
  };

  const handleNextImage = () => {
    setCurrentImageIndex((prev) => (prev === images.length - 1 ? 0 : prev + 1));
  };

  const handleImageClick = () => {
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
  };

  if (isLoading) {
    return (
      <div className="bg-base-100 rounded-lg shadow-lg overflow-hidden border border-base-300">
        <div className="skeleton h-96 w-full"></div>
        <div className="p-4">
          <div className="skeleton h-4 w-1/3 mb-2"></div>
          <div className="flex gap-2">
            <div className="skeleton h-16 w-16"></div>
            <div className="skeleton h-16 w-16"></div>
            <div className="skeleton h-16 w-16"></div>
          </div>
        </div>
      </div>
    );
  }

  const displayImages = images.length > 0 ? images : [defaultImage];
  const currentImage = displayImages[currentImageIndex];

  return (
    <>
      <div className="bg-base-100 rounded-lg shadow-lg overflow-hidden border border-base-300">
        {/* Main Image */}
        <div className="relative h-96 bg-base-200">
          <img
            src={currentImage}
            alt={`${propertyName} - Image ${currentImageIndex + 1}`}
            className="w-full h-full object-cover cursor-pointer"
            onClick={handleImageClick}
            onError={(e) => {
              e.currentTarget.src = defaultImage;
            }}
          />

          {/* Image Navigation - only show if there are multiple images */}
          {displayImages.length > 1 && (
            <>
              <button
                onClick={handlePreviousImage}
                className="absolute left-4 top-1/2 transform -translate-y-1/2 bg-black/50 text-white rounded-full p-2 hover:bg-black/70 transition-colors"
              >
                <Icon icon="mdi:chevron-left" className="w-6 h-6" />
              </button>
              <button
                onClick={handleNextImage}
                className="absolute right-4 top-1/2 transform -translate-y-1/2 bg-black/50 text-white rounded-full p-2 hover:bg-black/70 transition-colors"
              >
                <Icon icon="mdi:chevron-right" className="w-6 h-6" />
              </button>

              {/* Image Counter */}
              <div className="absolute top-4 right-4 bg-black/50 text-white px-3 py-1 rounded-full text-sm">
                {currentImageIndex + 1} / {displayImages.length}
              </div>
            </>
          )}

          {/* Expand Icon */}
          <button
            onClick={handleImageClick}
            className="absolute bottom-4 right-4 bg-black/50 text-white rounded-full p-2 hover:bg-black/70 transition-colors"
            title="View fullscreen"
          >
            <Icon icon="mdi:fullscreen" className="w-5 h-5" />
          </button>
        </div>

        {/* Image Thumbnails */}
        {displayImages.length > 1 && (
          <div className="p-4">
            <h3 className="text-lg font-semibold mb-3 flex items-center gap-2 text-gray-900 dark:text-base-content">
              <Icon icon="mdi:image-multiple" className="w-5 h-5" />
              Images ({displayImages.length})
            </h3>
            <div className="flex gap-2 overflow-x-auto pb-2">
              {displayImages.map((image, index) => (
                <button
                  key={index}
                  onClick={() => setCurrentImageIndex(index)}
                  className={`flex-shrink-0 w-16 h-16 rounded-lg overflow-hidden border-2 transition-all ${
                    index === currentImageIndex
                      ? "border-primary"
                      : "border-base-300 hover:border-primary/50"
                  }`}
                >
                  <img
                    src={image}
                    alt={`${propertyName} - Thumbnail ${index + 1}`}
                    className="w-full h-full object-cover"
                    onError={(e) => {
                      e.currentTarget.src = defaultImage;
                    }}
                  />
                </button>
              ))}
            </div>
          </div>
        )}
      </div>

      {/* Modal for fullscreen view */}
      {isModalOpen && (
        <div className="modal modal-open">
          <div className="modal-box max-w-6xl w-full h-full max-h-screen p-0">
            <div className="relative h-full">
              <img
                src={currentImage}
                alt={`${propertyName} - Fullscreen`}
                className="w-full h-full object-contain bg-black"
                onError={(e) => {
                  e.currentTarget.src = defaultImage;
                }}
              />

              {/* Close Button */}
              <button
                onClick={handleCloseModal}
                className="absolute top-4 right-4 bg-black/50 text-white rounded-full p-2 hover:bg-black/70 transition-colors z-10"
              >
                <Icon icon="mdi:close" className="w-6 h-6" />
              </button>

              {/* Navigation in modal */}
              {displayImages.length > 1 && (
                <>
                  <button
                    onClick={handlePreviousImage}
                    className="absolute left-4 top-1/2 transform -translate-y-1/2 bg-black/50 text-white rounded-full p-3 hover:bg-black/70 transition-colors z-10"
                  >
                    <Icon icon="mdi:chevron-left" className="w-8 h-8" />
                  </button>
                  <button
                    onClick={handleNextImage}
                    className="absolute right-4 top-1/2 transform -translate-y-1/2 bg-black/50 text-white rounded-full p-3 hover:bg-black/70 transition-colors z-10"
                  >
                    <Icon icon="mdi:chevron-right" className="w-8 h-8" />
                  </button>

                  {/* Image Counter in modal */}
                  <div className="absolute bottom-4 left-1/2 transform -translate-x-1/2 bg-black/50 text-white px-4 py-2 rounded-full">
                    {currentImageIndex + 1} / {displayImages.length}
                  </div>
                </>
              )}
            </div>
          </div>
          <form method="dialog" className="modal-backdrop">
            <button onClick={handleCloseModal}>close</button>
          </form>
        </div>
      )}
    </>
  );
};
