# Testing Setup - Frontend

## Librerías Instaladas

Se han instalado las siguientes librerías para testing:

- **Jest**: Framework de testing principal
- **@testing-library/react**: Utilidades para testear componentes React
- **@testing-library/jest-dom**: Matchers adicionales para Jest
- **@testing-library/user-event**: Simulación de eventos de usuario
- **jest-environment-jsdom**: Entorno DOM para Jest

## Configuración

### Archivos de Configuración

- `jest.config.js`: Configuración principal de Jest
- `jest.setup.js`: Setup global para los tests
- Scripts en `package.json`:
  - `npm test`: Ejecutar todos los tests
  - `npm run test:watch`: Ejecutar tests en modo watch
  - `npm run test:coverage`: Ejecutar tests con reporte de cobertura

### Estructura de Tests

Los tests están organizados en la carpeta `__tests__/` con la siguiente estructura:

```
__tests__/
├── app/                    # Tests de páginas principales
│   ├── page.test.tsx      # Test de la página principal
│   ├── login.test.tsx     # Test de la página de login
│   ├── owners.test.tsx    # Test de la página de propietarios
│   └── properties.test.tsx # Test de la página de propiedades
├── components/            # Tests de componentes
│   └── ProtectedRoute.test.tsx
├── modules/              # Tests de módulos específicos
│   └── login/
│       └── LoginPage.test.tsx
├── stores/               # Tests de stores/estado
│   └── user.store.test.ts
└── simple.test.tsx       # Test de ejemplo simple
```

## Tests Implementados

### 1. Test de la Página Principal (`page.test.tsx`)

- Verifica redirección a login cuando no está autenticado
- Verifica redirección a properties cuando está autenticado
- Verifica manejo de tokens expirados
- Verifica elementos de UI (loading spinner)

### 2. Test del Store de Usuario (`user.store.test.ts`) ✅

- Tests del estado inicial
- Tests de acciones de autenticación (setAuth, updateUser, updateTokens, clearAuth)
- Tests de expiración de tokens
- Tests de selectores
- Tests de hidratación

### 3. Test de ProtectedRoute (`ProtectedRoute.test.tsx`)

- Verifica protección de rutas
- Verifica redirección cuando no está autenticado
- Verifica renderizado de contenido protegido
- Verifica estados de carga

### 4. Tests de Páginas (`owners.test.tsx`, `properties.test.tsx`)

- Verifica renderizado dentro de ProtectedLayout
- Verifica elementos específicos de cada página
- Verifica que son rutas protegidas

### 5. Test de LoginPage (`LoginPage.test.tsx`)

- Verifica renderizado de componentes
- Verifica manejo de login exitoso
- Verifica manejo de errores
- Verifica estados de carga

## Cómo Ejecutar los Tests

### Ejecutar todos los tests

```bash
npm test
```

### Ejecutar tests en modo watch (desarrollo)

```bash
npm run test:watch
```

### Ejecutar tests con cobertura

```bash
npm run test:coverage
```

### Ejecutar un test específico

```bash
npm test simple.test.tsx
npm test user.store.test.ts
```

## Estado Actual

### ✅ Funcionando

- Configuración básica de Jest y React Testing Library
- Test del store de usuario (user.store.test.ts)
- Test simple de ejemplo (simple.test.tsx)

### ⚠️ Con Issues (Path Aliases)

Algunos tests tienen problemas con la resolución de path aliases (`@/...`). Esto es un problema conocido con la configuración de Jest y Next.js. Los tests están escritos correctamente pero necesitan ajustes en la configuración.

### Tests Disponibles:

- `__tests__/stores/user.store.test.ts` - ✅ Funciona perfectamente
- `__tests__/simple.test.tsx` - ✅ Test de ejemplo funcional
- Otros tests - ⚠️ Requieren ajuste de path aliases

## Próximos Pasos

1. **Resolver Path Aliases**: Configurar correctamente los aliases `@/` en Jest
2. **Tests de Integración**: Agregar tests que prueben flujos completos
3. **Tests de API**: Agregar tests para servicios y llamadas API
4. **Tests E2E**: Considerar agregar Cypress o Playwright para tests end-to-end

## Ejemplos de Uso

### Test Básico de Componente

```typescript
import { render, screen } from "@testing-library/react";
import MyComponent from "./MyComponent";

test("renders component correctly", () => {
  render(<MyComponent title="Test" />);
  expect(screen.getByText("Test")).toBeInTheDocument();
});
```

### Test con User Events

```typescript
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import LoginForm from "./LoginForm";

test("handles form submission", async () => {
  const user = userEvent.setup();
  const onSubmit = jest.fn();

  render(<LoginForm onSubmit={onSubmit} />);

  await user.type(screen.getByRole("textbox"), "test@example.com");
  await user.click(screen.getByRole("button", { name: /submit/i }));

  expect(onSubmit).toHaveBeenCalled();
});
```

## Comandos Útiles

```bash
# Instalar dependencias de testing (ya hecho)
bun add --dev jest @testing-library/react @testing-library/jest-dom @testing-library/user-event jest-environment-jsdom

# Ejecutar tests específicos por patrón
npm test -- --testNamePattern="should render"

# Ejecutar tests con más detalle
npm test -- --verbose

# Ejecutar tests sin fallar por snapshots
npm test -- --passWithNoTests
```
