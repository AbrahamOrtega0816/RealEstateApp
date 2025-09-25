// Tipos para autenticación basados en los DTOs del backend

export interface LoginDto {
  email: string;
  password: string;
  rememberMe?: boolean;
}

export interface UserDto {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  isActive: boolean;
  createdAt: string;
  lastLoginAt?: string;
}

export interface AuthResponseDto {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: UserDto;
}

export interface RegisterDto {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
}

export interface RefreshTokenDto {
  refreshToken: string;
}
