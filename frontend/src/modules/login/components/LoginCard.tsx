import React from "react";

interface LoginCardProps {
  children: React.ReactNode;
  title?: string;
  subtitle?: string;
}

const LoginCard: React.FC<LoginCardProps> = ({
  children,
  title = "Welcome back",
  subtitle = "Enter your credentials to access your account",
}) => {
  return (
    <div className="w-full">
      {/* Header with enhanced typography */}
      <div className="text-center mb-10">
        <div className="mb-4">
          <h2 className="text-4xl font-bold tracking-tight mb-3 text-base-content">
            {title}
          </h2>
          <div className="w-16 h-1 mx-auto rounded-full bg-gradient-to-r from-primary to-secondary"></div>
        </div>
        <p className="text-lg font-medium leading-relaxed text-base-content/70">
          {subtitle}
        </p>
      </div>

      {/* Content with enhanced styling */}
      <div className="space-y-6">{children}</div>
    </div>
  );
};

export default LoginCard;
