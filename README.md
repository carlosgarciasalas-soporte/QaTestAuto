# EFAC QA Testing y Automatizacion

## Descripcion General

EFAC es un sistema CRUD de clientes desarrollado en ASP.NET Core .NET 9 con enfoque academico y tecnico en QA Testing. El proyecto fue construido inicialmente para soportar pruebas manuales sobre API e interfaz web, y actualmente se encuentra preparado para evolucionar por etapas hacia pruebas automatizadas de backend y UI.

El objetivo funcional principal es administrar clientes bajo reglas de negocio asociadas a cumplimiento DIAN, incluyendo calculo de digito de verificacion, validacion de mayoria de edad, control de duplicidad de NIT y diferenciacion entre persona natural y persona juridica.

## Stack Tecnico

- Framework: ASP.NET Core .NET 9.
- Arquitectura: separacion por capas con enfoque tipo Clean Architecture.
- Backend: ASP.NET Core Web API.
- Frontend: Razor Pages, Bootstrap 5 y JavaScript/AJAX.
- Persistencia: Entity Framework Core InMemory.
- Documentacion y pruebas manuales de API: Swagger.
- Automatizacion API: xUnit, WebApplicationFactory y FluentAssertions.
- Automatizacion UI: Selenium WebDriver, Selenium Support y xUnit.

## Arquitectura Del Proyecto

La solucion `Efac.sln` esta organizada en proyectos independientes:

```text
Efac.Domain
Efac.Application
Efac.Infrastructure
Efac.WebAPI
Efac.Tests.Api
Efac.Tests.Selenium
test-assets
```

### `Efac.Domain`

Contiene la logica central del negocio:

- Entidad `Cliente`.
- Enums `TipoPersona` y `ResponsabilidadFiscal`.
- Excepciones de dominio.
- Servicio `DianModulo11Calculator` para normalizacion de NIT y calculo de DV.

### `Efac.Application`

Contiene casos de uso y contratos de aplicacion:

- DTOs de entrada y salida.
- Servicio `ClienteService`.
- Validaciones de aplicacion.
- Mapeos entre entidad y DTO.
- Contrato `IClienteRepository`.

### `Efac.Infrastructure`

Contiene la implementacion de persistencia:

- `EfacDbContext`.
- `ClienteRepository`.
- Configuracion de Entity Framework Core InMemory.
- Inyeccion de dependencias de infraestructura.

### `Efac.WebAPI`

Contiene la capa de exposicion:

- Endpoints REST.
- Swagger.
- Razor Pages.
- Interfaz web para pruebas manuales.
- IDs estables en controles de UI para facilitar automatizacion con Selenium.

### `Efac.Tests.Api`

Proyecto independiente para pruebas automatizadas de API. Permite validar reglas HTTP, contratos JSON y reglas de negocio sin necesidad de navegador.

### `Efac.Tests.Selenium`

Proyecto independiente para pruebas E2E de interfaz. Simula interacciones reales del usuario usando Selenium WebDriver.

## Reglas De Negocio Principales

- El NIT debe normalizarse antes de persistir o calcular DV.
- El digito de verificacion se calcula con algoritmo DIAN modulo 11.
- No se permite registrar dos clientes con el mismo NIT.
- La duplicidad de NIT responde `409 Conflict`.
- Una persona natural requiere nombres, apellidos y fecha de nacimiento.
- Una persona natural menor de edad debe ser rechazada.
- Una persona juridica requiere razon social.
- Para persona juridica no deben persistirse nombres, apellidos ni fecha de nacimiento.
- Para persona natural no debe persistirse razon social.

## Endpoints Disponibles

```http
GET    /api/clientes
GET    /api/clientes/{id}
GET    /api/clientes/calcular-dv/{nit}
POST   /api/clientes
PUT    /api/clientes/{id}
DELETE /api/clientes/{id}
```

Swagger se encuentra disponible en:

```text
http://localhost:5100/swagger
```

La interfaz web se encuentra disponible en:

```text
http://localhost:5100/
```

## Enfoque QA Manual

La primera fase del proyecto fue orientada a pruebas manuales. Swagger se utiliza como herramienta principal para validar los endpoints, payloads, codigos HTTP y respuestas JSON.

La interfaz web permite validar flujos funcionales desde la perspectiva del usuario:

- Carga inicial del listado.
- Creacion de cliente.
- Edicion de cliente.
- Eliminacion de cliente.
- Busqueda por NIT o nombre.
- Calculo automatico de DV.
- Visualizacion de errores de negocio.
- Alternancia de campos entre persona natural y juridica.

La trazabilidad de casos manuales esta documentada en:

```text
QA_Matriz_Evidencias_Pruebas.md
```

## Adaptacion De Recomendaciones Del Equipo De Desarrollo

El equipo propuso una referencia general para automatizacion basada en backend, frontend y scripts. Para este proyecto se decidio adaptar esas recomendaciones al stack real del sistema, manteniendo .NET como tecnologia principal.

Decision tecnica:

- Se mantiene la estructura .NET del proyecto actual.
- No se incorpora Postman en la fase automatizada porque Swagger ya cubrio la validacion manual de API.
- Las pruebas automatizadas de API se implementan con xUnit y `WebApplicationFactory`.
- Las pruebas E2E se implementan con Selenium WebDriver en un proyecto .NET separado.
- La automatizacion se agrega sin mezclar codigo productivo con codigo de pruebas.

## Preparacion Para Selenium

La UI fue construida con identificadores estables para facilitar automatizacion. Selectores relevantes:

```text
input-search
btn-new-client
cliente-modal
form-validation-summary
input-tipo-persona
input-nit
input-dv
input-nombres
input-apellidos
input-fecha-nacimiento
input-razon-social
input-email
input-telefono
input-direccion
input-ciudad
input-responsabilidad
btn-save-client
empty-state
```

Estos IDs permiten crear Page Objects robustos sin depender de textos visibles, posiciones en pantalla o clases CSS cambiantes.

## Estado De Automatizacion

### Etapa 1 - Base Tecnica

Estado: implementada.

Incluye:

- Creacion de `Efac.Tests.Api`.
- Creacion de `Efac.Tests.Selenium`.
- Inclusion de ambos proyectos en `Efac.sln`.
- Configuracion de paquetes base.
- Pruebas API iniciales.
- Base inicial de Page Object para Selenium.
- Carpetas para evidencias.
- Documento `AUTOMATIZACION_PLAN.md`.

Pruebas API iniciales:

- `GET /api/clientes` responde `200 OK`.
- `POST /api/clientes` con NIT duplicado responde `409 Conflict`.

Prueba Selenium inicial:

- Smoke test de carga de controles principales, activable mediante variable de entorno.

### Etapa 2 - Pruebas Automatizadas De API

Estado: implementada.

Incluye pruebas automatizadas para:

- Disponibilidad de `GET /api/clientes`.
- Consulta de cliente inexistente con `404 Not Found`.
- Calculo de DV con NIT formateado.
- Rechazo de NIT invalido con `400 Bad Request`.
- Creacion valida de persona natural con `201 Created`.
- Rechazo de NIT duplicado con `409 Conflict`.
- Rechazo de persona natural menor de edad.
- Rechazo de persona natural sin fecha de nacimiento.
- Rechazo de persona juridica sin razon social.
- Actualizacion valida de cliente.
- Eliminacion valida de cliente y verificacion posterior de `404 Not Found`.

Resultado actual de la suite:

- `Efac.Tests.Api`: 11 pruebas automatizadas superadas.
- `Efac.Tests.Selenium`: 4 pruebas superadas.
- Total: 15 pruebas superadas.

### Etapa 3 - Automatizacion UI Con Selenium

Estado: implementada parcialmente.

Incluye:

- Page Object `ClientesPage` con esperas explicitas.
- Fabrica `WebDriverFactory` preparada para Chrome headless.
- Prueba de carga de pagina principal.
- Prueba de busqueda por NIT.
- Prueba de alternancia de campos entre persona natural y juridica.
- Prueba de calculo de DV desde la UI.

Resultado actual de la suite Selenium activada contra `http://localhost:5100/`:

- `Efac.Tests.Selenium`: 4 pruebas superadas.

## Ejecucion Del Proyecto

Restaurar dependencias:

```powershell
dotnet restore Efac.sln
```

Compilar:

```powershell
dotnet build Efac.sln --no-restore
```

Levantar la WebAPI:

```powershell
dotnet run --project Efac.WebAPI --launch-profile http
```

Ejecutar pruebas automatizadas:

```powershell
dotnet test Efac.sln --no-build
```

Ejecutar Selenium contra la aplicacion local:

```powershell
$env:EFAC_RUN_SELENIUM="true"
$env:EFAC_BASE_URL="http://localhost:5100/"
dotnet test Efac.Tests.Selenium --no-build
```

## Evidencias

Las evidencias de automatizacion se organizan en:

```text
test-assets/evidence/screenshots
test-assets/evidence/reports
```

Estas carpetas estan preparadas para capturas de pantalla, reportes y registros de ejecucion. Los archivos generados no deben versionarse salvo que sean evidencias finales requeridas por el equipo.

## Roadmap Tecnico

### Etapa 4 - Flujos UI CRUD Con Selenium

Casos recomendados:

- Creacion de cliente natural valido.
- Creacion de cliente juridico valido.
- Mensaje de error por menor de edad.
- Busqueda por NIT o nombre.
- Edicion de cliente.
- Eliminacion de cliente.

### Etapa 4 - Evidencias Y Reportes

Objetivos:

- Capturas automaticas ante fallos de Selenium.
- Reportes de ejecucion.
- Comandos `.bat` para ejecucion en Windows.
- Documentacion de resultados por ciclo.

## Criterio De Actualizacion Del README

Este README debe mantenerse como documento tecnico vivo. Cada avance debe reflejar:

- Nueva etapa implementada.
- Pruebas agregadas.
- Cambios relevantes en arquitectura o dependencias.
- Comandos nuevos de ejecucion.
- Resultado de validacion.
- Estado de evidencias.

Antes de cerrar cada fase se debe ejecutar:

```powershell
dotnet build Efac.sln --no-restore
dotnet test Efac.sln --no-build
```

## Repositorio

Repositorio GitHub:

```text
https://github.com/carlosgarciasalas-soporte/QaTestAuto.git
```
