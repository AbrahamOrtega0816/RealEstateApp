"use client";

import React from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import Input from "@/components/ui/Input";
import Button from "@/components/ui/Button";

// Validation schema with Zod
const loginSchema = z.object({
  email: z
    .string()
    .min(1, "Email is required")
    .email("Please enter a valid email address"),
  password: z
    .string()
    .min(1, "Password is required")
    .min(6, "Password must be at least 6 characters long"),
  rememberMe: z.boolean().optional(),
});

type LoginFormData = z.infer<typeof loginSchema>;

interface LoginFormProps {
  onSubmit: (data: LoginFormData) => void;
  isLoading?: boolean;
}

const LoginForm: React.FC<LoginFormProps> = ({
  onSubmit,
  isLoading = false,
}) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
      <Input
        {...register("email")}
        type="email"
        label="Email address"
        placeholder="your@email.com"
        error={errors.email?.message}
        autoComplete="email"
      />

      <Input
        {...register("password")}
        type="password"
        label="Password"
        placeholder="••••••••"
        error={errors.password?.message}
        autoComplete="current-password"
      />

      <div className="flex items-center justify-between">
        <div className="form-control">
          <label className="label cursor-pointer">
            <input
              {...register("rememberMe")}
              id="remember-me"
              type="checkbox"
              className="checkbox checkbox-primary"
            />
            <span className="label-text ml-2">Remember me</span>
          </label>
        </div>
      </div>

      <div>
        <Button
          type="submit"
          variant="primary"
          size="lg"
          className="btn btn-primary btn-m w-full rounded-2xl"
          isLoading={isLoading}
        >
          Sign in
        </Button>
      </div>
    </form>
  );
};

export default LoginForm;
