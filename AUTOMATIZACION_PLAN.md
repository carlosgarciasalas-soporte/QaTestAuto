# Plan de Automatizacion QA - EFAC

## Alcance

El proyecto mantiene su arquitectura principal en .NET 9:

- `Efac.Domain`
- `Efac.Application`
- `Efac.Infrastructure`
- `Efac.WebAPI`

La automatizacion se implementa en proyectos separados para no mezclar codigo productivo con codigo de pruebas:

- `Efac.Tests.Api`: pruebas automatizadas de API y reglas HTTP.
- `Efac.Tests.Selenium`: pruebas E2E de interfaz con Selenium WebDriver.

Swagger queda como soporte manual/documental. Postman queda fuera del alcance de esta fase.

## Etapa 1 - Base tecnica

Estado: implementada.

Incluye:

- Proyectos de prueba agregados a `Efac.sln`.
- Paquetes base para xUnit, FluentAssertions, WebApplicationFactory y Selenium.
- Pruebas API iniciales:
  - `GET /api/clientes` responde `200 OK` con datos semilla.
  - `POST /api/clientes` con NIT duplicado responde `409 Conflict`.
- Base Selenium inicial:
  - Configuracion de URL base.
  - Fabrica de Chrome WebDriver.
  - Page Object inicial `ClientesPage`.
  - Prueba smoke activable por variable de entorno.
- Carpetas de evidencias:
  - `test-assets/evidence/screenshots`
  - `test-assets/evidence/reports`

## Ejecucion

Restaurar y compilar:

```powershell
dotnet restore Efac.sln
dotnet build Efac.sln --no-restore
```

Ejecutar pruebas automatizadas:

```powershell
dotnet test Efac.sln --no-build
```

Ejecutar Selenium contra la aplicacion local:

```powershell
dotnet run --project Efac.WebAPI --launch-profile http
$env:EFAC_RUN_SELENIUM="true"
$env:EFAC_BASE_URL="http://localhost:5100/"
dotnet test Efac.Tests.Selenium --no-build
```

## Etapa 2 - Pruebas API

Estado: implementada.

Incluye:

- Cliente inexistente retorna `404 Not Found`.
- Calculo DV con NIT formateado retorna `200 OK`, NIT normalizado y DV.
- NIT invalido retorna `400 Bad Request`.
- Creacion valida de persona natural retorna `201 Created`.
- NIT duplicado retorna `409 Conflict`.
- Menor de edad retorna `400 Bad Request`.
- Persona natural sin fecha retorna `400 Bad Request`.
- Persona juridica sin razon social retorna `400 Bad Request`.
- Actualizacion valida retorna `200 OK`.
- Eliminacion retorna `204 No Content` y el cliente queda inaccesible.

## Etapa 3 - Selenium

Estado: implementada parcialmente.

Incluye:

- Page Object `ClientesPage` con esperas explicitas.
- Fabrica `WebDriverFactory` preparada para Chrome headless.
- Prueba de carga de pagina principal.
- Prueba de busqueda por NIT.
- Prueba de alternancia de campos Natural/Juridica.
- Prueba de calculo de DV desde la UI usando la API.

Pendiente para la siguiente etapa:

- Creacion de cliente desde la UI.
- Mensaje de error por menor de edad.
- Edicion de cliente.
- Eliminacion de cliente.

## Criterio de avance

Cada etapa debe cerrar con:

- Compilacion sin errores.
- Pruebas automatizadas ejecutadas.
- Evidencia de resultado.
- Actualizacion de este plan.
