"use client";

import React from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

import Input from "@/components/ui/Input";
import Button from "@/components/ui/Button";
import { LoginFormData, LoginFormProps, loginSchema } from "../const/form";

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
