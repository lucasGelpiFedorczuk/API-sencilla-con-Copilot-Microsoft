# UserManagementAPI

Resumen de cambios realizados

Este repositorio contiene una API mínima para la gestión de usuarios. Durante esta sesión se añadieron los componentes necesarios para exponer endpoints CRUD y la configuración asociada.

Cambios principales

- Dependencias:
  - Se añadió `Microsoft.EntityFrameworkCore.InMemory` para persistencia en memoria durante desarrollo.
  - Se usa `Microsoft.AspNetCore.OpenApi` (integrado) para OpenAPI/Swagger en entorno de desarrollo.

- Archivos añadidos/actualizados (resumen):
  - `Program.cs` — registra servicios: controllers, OpenAPI, CORS, DbContext InMemory, DI para repositorio/servicio.
  - `UserManagementAPI.csproj` — referencias a EF Core InMemory y OpenAPI (ajustes de paquetes).
  - `Models/User.cs` — entidad `User`.
  - `Data/UserDbContext.cs` — `DbContext` con `DbSet<User>`.
  - `Repositories/IUserRepository.cs` — interfaz del repositorio.
  - `Repositories/UserRepository.cs` — implementación usando EF Core.
  - `Services/IUserService.cs` — interfaz del servicio de negocio.
  - `Services/UserService.cs` — implementación del servicio (CRUD lógico).
  - `Controllers/UsersController.cs` — endpoints CRUD: GET, GET/{id}, POST, PUT/{id}, DELETE/{id}.
  - `Models/Dtos/UserCreateDto.cs`, `UserUpdateDto.cs`, `UserReadDto.cs` — DTOs con validación (DataAnnotations).

Contrato de la API (endpoints)

- GET /api/users
  - Respuesta: 200 OK con lista de `UserReadDto` (puede estar vacía)
- GET /api/users/{id}
  - Respuesta: 200 OK con `UserReadDto` | 404 Not Found
- POST /api/users
  - Cuerpo: `UserCreateDto` (FirstName, LastName, Email)
  - Respuesta: 201 Created con `UserReadDto` (Location header apunta a GET por id)
- PUT /api/users/{id}
  - Cuerpo: `UserUpdateDto` (Id, FirstName, LastName, Email)
  - Respuesta: 204 No Content | 400 Bad Request (id mismatch / validación) | 404 Not Found
- DELETE /api/users/{id}
  - Respuesta: 204 No Content | 404 Not Found

Nota: `UserReadDto` incluye `CreatedAt`.

Cómo ejecutar (Windows / cmd.exe)

Abre una terminal en la carpeta del proyecto y ejecuta:

```cmd
cd "c:\Users\Lucas\Desktop\Copilot\UserManagementAPI"
dotnet restore "c:\Users\Lucas\Desktop\Copilot\UserManagementAPI\UserManagementAPI.csproj"
dotnet build "c:\Users\Lucas\Desktop\Copilot\UserManagementAPI\UserManagementAPI.csproj" -c Debug
dotnet run --project "c:\Users\Lucas\Desktop\Copilot\UserManagementAPI\UserManagementAPI.csproj"
```

Cuando la aplicación esté corriendo verás una URL (por ejemplo `http://localhost:5249`). En entorno de desarrollo, la OpenAPI integrada queda disponible.

Pruebas rápidas (PowerShell)

Lista de usuarios (espera array vacío al inicio):

```powershell
Invoke-RestMethod -Uri http://localhost:5249/api/users -Method Get
```

Crear usuario:

```powershell
$json = @{ FirstName = 'Lucas'; LastName = 'Perez'; Email = 'lucas@example.com' } | ConvertTo-Json
Invoke-RestMethod -Uri http://localhost:5249/api/users -Method Post -Body $json -ContentType 'application/json'
```

Obtener usuario por id (reemplaza {id}):

```powershell
Invoke-RestMethod -Uri http://localhost:5249/api/users/{id} -Method Get
```

Actualizar usuario:

```powershell
$json = @{ Id = '<GUID>'; FirstName = 'Nuevo'; LastName = 'Apellido'; Email = 'nuevo@example.com' } | ConvertTo-Json
Invoke-RestMethod -Uri http://localhost:5249/api/users/<GUID> -Method Put -Body $json -ContentType 'application/json'
```

Eliminar usuario:

```powershell
Invoke-RestMethod -Uri http://localhost:5249/api/users/<GUID> -Method Delete
```

Notas importantes

- Persistencia: actualmente se usa InMemoryDatabase. Al reiniciar la aplicación, los datos se perderán. Para producción, reemplazar por SQL Server/Postgres y usar migraciones de EF Core.
- Validación: se usan DataAnnotations en DTOs. Considera FluentValidation para reglas más complejas.
- Seguridad: no hay autenticación ni autorización. Añadir JWT o similar antes de exponer en producción.

Sugerencia de flujo Git para subir los cambios (cmd.exe)

```cmd
cd "c:\Users\Lucas\Desktop\Copilot\UserManagementAPI"
rem Crear una nueva rama para estos cambios
git checkout -b feature/users-crud

rem Añadir los archivos y hacer commit
git add .
git commit -m "feat: users CRUD endpoints, DTOs, InMemory EF Core and basic DI/config"

rem Subir la rama al remoto (origin)
git push -u origin feature/users-crud
```

Sugerencia de mensaje de commit:

"feat(users): add CRUD endpoints, DTOs, EF InMemory persistence and DI configuration"

Próximos pasos recomendados (opcional)

- Añadir autenticación/autorization (JWT + roles)
- Reemplazar InMemory por SQL Server/Postgres y añadir migraciones
- Añadir pruebas unitarias e integración (xUnit + test server)
- Agregar logging estructurado y manejo de errores global

Validaciones añadidas

- Validación de ID vacía: los endpoints que reciben `id` ahora devuelven `400 Bad Request` si se pasa `Guid.Empty`. Esto evita llamadas inválidas al servicio/repositorio.
- Prevención de email duplicado: en creación y actualización se comprueba si el email ya está en uso. Si existe un usuario con el mismo email, la API devuelve `409 Conflict` con un cuerpo JSON: `{ "message": "Email already in use" }`.

Notas sobre la validación de email:
- La verificación se realiza en la capa de servicio usando `IUserRepository.EmailExistsAsync(email, excludeUserId)`.
- Para producción recomendamos además:
  - normalizar el email (trim + ToLowerInvariant) antes de persistir y comparar,
  - añadir un índice único en la columna `Email` a nivel de base de datos para garantizar unicidad.

---

Si quieres, puedo:

- crear un archivo `README.md` (ya lo hice) y/o agregar un `CHANGELOG.md` más detallado,
- generar y añadir tests unitarios/integ. con instrucciones para correrlos,
- preparar las migraciones y cambiar la configuración para SQL Server.

Dime cuál sigue y lo implemento.
