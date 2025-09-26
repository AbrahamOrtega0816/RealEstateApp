"use client";

import React, { useState } from "react";
import { useForm } from "react-hook-form";
import { toast } from "react-hot-toast";
import Button from "@/components/ui/Button";
import Input from "@/components/ui/Input";
import { useCreateOwner } from "@/modules/owners/services/ownerService";
import { CreateOwnerDto } from "@/modules/owners/types/owner";

interface CreateOwnerModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess?: () => void;
}

interface CreateOwnerFormData {
  name: string;
  address: string;
  birthday: string;
  photo?: FileList;
}

const CreateOwnerModal: React.FC<CreateOwnerModalProps> = ({
  isOpen,
  onClose,
  onSuccess,
}) => {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [photoPreview, setPhotoPreview] = useState<string | null>(null);
  const createOwnerMutation = useCreateOwner();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
    watch,
  } = useForm<CreateOwnerFormData>();

  // Watch photo field to show preview
  const photoFile = watch("photo");

  React.useEffect(() => {
    if (photoFile && photoFile[0]) {
      const file = photoFile[0];
      const reader = new FileReader();
      reader.onload = (e) => {
        setPhotoPreview(e.target?.result as string);
      };
      reader.readAsDataURL(file);
    } else {
      setPhotoPreview(null);
    }
  }, [photoFile]);

  const onSubmit = async (data: CreateOwnerFormData) => {
    try {
      setIsSubmitting(true);

      // Create FormData to handle file upload
      const formData = new FormData();
      formData.append("name", data.name);
      formData.append("address", data.address);
      formData.append("birthday", data.birthday);

      if (data.photo && data.photo[0]) {
        formData.append("photo", data.photo[0]);
      }

      await createOwnerMutation.mutateAsync(formData);

      toast.success("Owner created successfully!");
      reset();
      setPhotoPreview(null);
      onSuccess?.();
      onClose();
    } catch (error) {
      toast.error("Error creating owner");
      console.error("Error creating owner:", error);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleClose = () => {
    reset();
    setPhotoPreview(null);
    onClose();
  };

  return (
    <dialog className={`modal ${isOpen ? "modal-open" : ""}`}>
      <div className="modal-box">
        <div className="flex justify-between items-center mb-4">
          <h3 className="font-bold text-lg text-base-content">
            Create New Owner
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
          <Input
            label="Name"
            placeholder="Enter owner name"
            {...register("name", {
              required: "Name is required",
              minLength: {
                value: 2,
                message: "Name must be at least 2 characters",
              },
            })}
            error={errors.name?.message}
          />

          <Input
            label="Address"
            placeholder="Enter address"
            {...register("address", {
              required: "Address is required",
              minLength: {
                value: 5,
                message: "Address must be at least 5 characters",
              },
            })}
            error={errors.address?.message}
          />

          <Input
            label="Birthday"
            type="date"
            {...register("birthday", {
              required: "Birthday is required",
            })}
            error={errors.birthday?.message}
          />

          <div className="form-control w-full">
            <label className="label">
              <span className="label-text font-medium">Photo</span>
            </label>
            <input
              type="file"
              accept="image/*"
              className="file-input file-input-bordered w-full"
              {...register("photo")}
            />
            {errors.photo && (
              <label className="label">
                <span className="label-text-alt text-error">
                  {errors.photo.message}
                </span>
              </label>
            )}
          </div>

          {photoPreview && (
            <div className="mt-4">
              <label className="label">
                <span className="label-text font-medium">Photo Preview</span>
              </label>
              <div className="flex justify-center">
                <img
                  src={photoPreview}
                  alt="Photo preview"
                  className="w-32 h-32 object-cover rounded-lg border"
                />
              </div>
            </div>
          )}

          <div className="modal-action">
            <Button type="button" variant="outline" onClick={handleClose}>
              Cancel
            </Button>
            <Button type="submit" isLoading={isSubmitting} variant="primary">
              Create Owner
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

export default CreateOwnerModal;
