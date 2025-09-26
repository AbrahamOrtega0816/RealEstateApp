"use client";

import React, { useState } from "react";
import { useForm } from "react-hook-form";
import { toast } from "react-hot-toast";
import Button from "@/components/ui/Button";
import Input from "@/components/ui/Input";
import { useCreateProperty } from "@/modules/properties/services/propertyService";
import { useGetOwners } from "@/modules/owners/services/ownerService";
import { CreatePropertyDto } from "@/modules/properties/types/property";

interface CreatePropertyModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess?: () => void;
}

interface CreatePropertyFormData {
  idOwner: string;
  name: string;
  address: string;
  price: number;
  codeInternal: string;
  year: number;
  images?: FileList;
}

const CreatePropertyModal: React.FC<CreatePropertyModalProps> = ({
  isOpen,
  onClose,
  onSuccess,
}) => {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [imagesPreviews, setImagesPreviews] = useState<string[]>([]);
  const createPropertyMutation = useCreateProperty();

  // Get owners for the select dropdown
  const { data: ownersData, isLoading: isLoadingOwners } = useGetOwners({
    pageNumber: 1,
    pageSize: 100, // Get all owners for the dropdown
  });

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
    watch,
  } = useForm<CreatePropertyFormData>();

  // Watch images field to show preview
  const imagesFiles = watch("images");

  React.useEffect(() => {
    if (imagesFiles && imagesFiles.length > 0) {
      const previews: string[] = [];
      Array.from(imagesFiles).forEach((file) => {
        const reader = new FileReader();
        reader.onload = (e) => {
          previews.push(e.target?.result as string);
          if (previews.length === imagesFiles.length) {
            setImagesPreviews([...previews]);
          }
        };
        reader.readAsDataURL(file);
      });
    } else {
      setImagesPreviews([]);
    }
  }, [imagesFiles]);

  const onSubmit = async (data: CreatePropertyFormData) => {
    try {
      setIsSubmitting(true);

      // Create FormData to handle file upload
      const formData = new FormData();
      formData.append("idOwner", data.idOwner);
      formData.append("name", data.name);
      formData.append("address", data.address);
      formData.append("price", data.price.toString());
      formData.append("codeInternal", data.codeInternal);
      formData.append("year", data.year.toString());

      if (data.images && data.images.length > 0) {
        Array.from(data.images).forEach((file) => {
          formData.append("images", file);
        });
      }

      await createPropertyMutation.mutateAsync(formData);

      toast.success("Property created successfully!");
      reset();
      setImagesPreviews([]);
      onSuccess?.();
      onClose();
    } catch (error) {
      toast.error("Error creating property");
      console.error("Error creating property:", error);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleClose = () => {
    reset();
    setImagesPreviews([]);
    onClose();
  };

  const currentYear = new Date().getFullYear();

  return (
    <dialog className={`modal ${isOpen ? "modal-open" : ""}`}>
      <div className="modal-box max-w-2xl">
        <div className="flex justify-between items-center mb-4">
          <h3 className="font-bold text-lg text-base-content">
            Create New Property
          </h3>
          <button
            onClick={handleClose}
            className="btn btn-sm btn-circle btn-ghost absolute right-2 top-2 text-base-content"
            type="button"
          >
            ✕
          </button>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          {/* Owner Selection */}
          <div className="form-control w-full">
            <label className="label">
              <span className="label-text font-medium">Owner *</span>
            </label>
            <select
              className="select select-bordered w-full"
              {...register("idOwner", {
                required: "Owner is required",
              })}
            >
              <option value="">Select an owner</option>
              {isLoadingOwners ? (
                <option disabled>Loading owners...</option>
              ) : (
                ownersData?.items?.map((owner) => (
                  <option key={owner.id} value={owner.id}>
                    {owner.name}
                  </option>
                ))
              )}
            </select>
            {errors.idOwner && (
              <label className="label">
                <span className="label-text-alt text-error">
                  {errors.idOwner.message}
                </span>
              </label>
            )}
          </div>

          {/* Property Name */}
          <Input
            label="Property Name"
            placeholder="Enter property name"
            {...register("name", {
              required: "Property name is required",
              minLength: {
                value: 2,
                message: "Property name must be at least 2 characters",
              },
            })}
            error={errors.name?.message}
          />

          {/* Address */}
          <Input
            label="Address"
            placeholder="Enter property address"
            {...register("address", {
              required: "Address is required",
              minLength: {
                value: 5,
                message: "Address must be at least 5 characters",
              },
            })}
            error={errors.address?.message}
          />

          {/* Price and Code Internal - Row */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <Input
              label="Price"
              type="number"
              placeholder="Enter price"
              step="0.01"
              min="0"
              {...register("price", {
                required: "Price is required",
                min: {
                  value: 0,
                  message: "Price must be greater than 0",
                },
                valueAsNumber: true,
              })}
              error={errors.price?.message}
            />

            <Input
              label="Internal Code"
              placeholder="Enter internal code"
              {...register("codeInternal", {
                required: "Internal code is required",
                minLength: {
                  value: 2,
                  message: "Internal code must be at least 2 characters",
                },
              })}
              error={errors.codeInternal?.message}
            />
          </div>

          {/* Year */}
          <div className="form-control">
            <label className="label">
              <span className="label-text text-sm font-medium text-base-content">
                Year Built
              </span>
            </label>
            <select
              className="select select-bordered w-full focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all duration-200"
              {...register("year", {
                required: "Year is required",
                valueAsNumber: true,
              })}
            >
              <option value="">Select year</option>
              {Array.from({ length: currentYear - 1799 }, (_, i) => {
                const year = currentYear - i;
                return (
                  <option key={year} value={year}>
                    {year}
                  </option>
                );
              })}
            </select>
            {errors.year && (
              <label className="label">
                <span className="label-text-alt text-error">
                  {errors.year.message}
                </span>
              </label>
            )}
          </div>

          {/* Images */}
          <div className="form-control w-full">
            <label className="label">
              <span className="label-text font-medium">Property Images</span>
            </label>
            <input
              type="file"
              accept="image/*"
              multiple
              className="file-input file-input-bordered w-full"
              {...register("images")}
            />
            {errors.images && (
              <label className="label">
                <span className="label-text-alt text-error">
                  {errors.images.message}
                </span>
              </label>
            )}
          </div>

          {/* Images Preview */}
          {imagesPreviews.length > 0 && (
            <div className="mt-4">
              <label className="label">
                <span className="label-text font-medium">Images Preview</span>
              </label>
              <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                {imagesPreviews.map((preview, index) => (
                  <div key={index} className="relative">
                    <img
                      src={preview}
                      alt={`Property image ${index + 1}`}
                      className="w-full h-24 object-cover rounded-lg border"
                    />
                  </div>
                ))}
              </div>
            </div>
          )}

          <div className="modal-action">
            <Button type="button" variant="outline" onClick={handleClose}>
              Cancel
            </Button>
            <Button type="submit" isLoading={isSubmitting} variant="primary">
              Create Property
            </Button>
          </div>
        </form>
      </div>

      {/* Modal backdrop - closes modal when clicked outside */}
      <form method="dialog" className="modal-backdrop">
        <button type="button" onClick={handleClose}>
          close
        </button>
      </form>
    </dialog>
  );
};

export default CreatePropertyModal;
