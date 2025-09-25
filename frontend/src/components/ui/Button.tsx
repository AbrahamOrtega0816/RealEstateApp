import React from "react";

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: "primary" | "secondary" | "outline" | "ghost" | "link";
  size?: "xs" | "sm" | "md" | "lg";
  isLoading?: boolean;
  children: React.ReactNode;
}

const Button: React.FC<ButtonProps> = ({
  variant = "primary",
  size = "md",
  isLoading = false,
  children,
  className = "btn",
  disabled,
  ...props
}) => {
  const variantClass = variant ? `btn-${variant}` : "";
  const sizeClass = size !== "md" ? `btn-${size}` : "";

  return (
    <button
      className={`${className} ${variantClass} ${sizeClass}`.trim()}
      disabled={disabled || isLoading}
      {...props}
    >
      {isLoading && <span className="loading loading-spinner"></span>}
      {children}
    </button>
  );
};

export default Button;
