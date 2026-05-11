<div align="center">

# AA2 Taller: Automatización de pruebas con Selenium

**Carlos Alberto García Salas**  
**Universidad Tecnológica del Oriente**  
**Pruebas Y Calidad De Software**

</div>

---

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
- `Efac.Tests.Selenium`: 10 pruebas superadas.
- Total: 21 pruebas superadas.

### Etapa 3 - Automatizacion UI Con Selenium

Estado: implementada parcialmente.

Incluye:

- Page Object `ClientesPage` con esperas explicitas.
- Fabrica `WebDriverFactory` preparada para Chrome visible o headless.
- Prueba de carga de pagina principal.
- Prueba de busqueda por NIT.
- Prueba de alternancia de campos entre persona natural y juridica.
- Prueba de calculo de DV desde la UI.

Resultado actual de la suite Selenium activada contra `http://localhost:5100/`:

- `Efac.Tests.Selenium`: 10 pruebas superadas.

## Implementacion De Las Pruebas Automatizadas

La automatizacion se implemento separando las pruebas del codigo productivo para mantener trazabilidad y facilitar la entrega de evidencias.

| Componente | Proposito |
|---|---|
| `Efac.Tests.Api` | Contiene pruebas automatizadas de API con xUnit, `WebApplicationFactory` y FluentAssertions. |
| `Efac.Tests.Selenium` | Contiene pruebas E2E de interfaz con Selenium WebDriver. |
| `Efac.Tests.Selenium/Pages/ClientesPage.cs` | Page Object con selectores y acciones reutilizables sobre la pantalla de clientes. |
| `Efac.Tests.Selenium/Infrastructure/WebDriverFactory.cs` | Fabrica de navegador Chrome en modo visible o headless segun variable de entorno. |
| `run-qa-tests.bat` | Punto de entrada para ejecutar todo el ciclo QA desde Windows. |
| `run-qa-tests.ps1` | Script auxiliar que controla logs, WebAPI, ejecucion de pruebas y evidencias. |
| `test-assets/evidence/reports` | Carpeta donde se guardan logs y reportes TRX por ejecucion. |

### Paso A Paso De Implementacion

1. Se creo el proyecto `Efac.Tests.Api` para validar endpoints, codigos HTTP, contratos JSON y reglas de negocio desde la capa API.
2. Se agrego `WebApplicationFactory<Program>` para ejecutar la WebAPI en memoria durante las pruebas API, sin depender de navegador.
3. Se crearon datos de prueba para persona natural, persona juridica, NIT duplicado, menor de edad y actualizacion/eliminacion.
4. Se creo el proyecto `Efac.Tests.Selenium` para validar flujos visibles de usuario sobre la interfaz Razor.
5. Se agrego un Page Object `ClientesPage` para centralizar selectores como `input-search`, `btn-new-client`, `input-nit` e `input-dv`.
6. Se configuro `WebDriverFactory` para levantar Chrome en modo visible para evidencias o en modo headless para ejecuciones silenciosas.
7. Se agregaron variables de entorno para activar Selenium solo cuando se requiera:

```powershell
$env:EFAC_RUN_SELENIUM="true"
$env:EFAC_BASE_URL="http://localhost:5100/"
$env:EFAC_SELENIUM_HEADLESS="false"
```

8. Se preparo `run-qa-tests.bat` como script de entrega. Este llama a `run-qa-tests.ps1`, ejecuta todas las fases y deja evidencia en pantalla y archivo.

## Que Se Prueba

### Pruebas API Automatizadas

Las pruebas API se encuentran en `Efac.Tests.Api/ClientesApiTests.cs`.

| Caso | Endpoint | Validacion principal |
|---|---|---|
| Listar clientes | `GET /api/clientes` | Retorna `200 OK` y datos semilla. |
| Cliente inexistente | `GET /api/clientes/{id}` | Retorna `404 Not Found`. |
| Calcular DV | `GET /api/clientes/calcular-dv/{nit}` | Normaliza NIT y calcula digito de verificacion. |
| NIT invalido | `GET /api/clientes/calcular-dv/ABC` | Retorna `400 Bad Request`. |
| Crear natural valido | `POST /api/clientes` | Retorna `201 Created`. |
| NIT duplicado | `POST /api/clientes` | Retorna `409 Conflict`. |
| Menor de edad | `POST /api/clientes` | Retorna `400 Bad Request`. |
| Natural sin fecha | `POST /api/clientes` | Retorna `400 Bad Request`. |
| Juridica sin razon social | `POST /api/clientes` | Retorna `400 Bad Request`. |
| Actualizar cliente | `PUT /api/clientes/{id}` | Retorna `200 OK` y datos actualizados. |
| Eliminar cliente | `DELETE /api/clientes/{id}` | Retorna `204 No Content` y luego `404 Not Found`. |

Campos validados por las pruebas API:

- `tipoPersona`: diferencia persona natural y juridica.
- `nit`: normalizacion, duplicidad y persistencia.
- `dv`: calculo DIAN modulo 11.
- `nombres` y `apellidos`: requeridos para persona natural.
- `razonSocial`: requerida para persona juridica y no persistida para natural.
- `fechaNacimiento`: requerida para natural y usada para validar mayoria de edad.
- `email`, `telefono`, `direccion`, `ciudadCodigoMunicipio`: persistencia en creacion y actualizacion.
- `responsabilidadFiscal`: persistencia del tipo de responsabilidad.

### Pruebas Selenium Automatizadas

Las pruebas Selenium se encuentran en `Efac.Tests.Selenium/Tests/ClientesUiSmokeTests.cs`.

| Caso | Flujo validado | Controles principales |
|---|---|---|
| Carga inicial | Abre `http://localhost:5100/` y valida controles principales. | `input-search`, `btn-new-client` |
| Busqueda por NIT | Escribe un NIT y verifica filtrado de tabla. | `input-search`, tabla de clientes |
| Alternancia Natural/Juridica | Abre modal y cambia tipo de persona. | `btn-new-client`, `input-tipo-persona`, `input-nombres`, `input-apellidos`, `input-fecha-nacimiento`, `input-razon-social` |
| Calculo de DV desde UI | Escribe NIT y espera el DV calculado por API. | `input-nit`, `input-dv` |
| Crear persona natural | Llena formulario, guarda y valida aparicion en tabla. | `input-nit`, `input-nombres`, `input-apellidos`, `input-fecha-nacimiento`, `input-email`, `btn-save-client` |
| Crear persona juridica | Llena razon social, guarda y valida aparicion en tabla. | `input-tipo-persona`, `input-razon-social`, `input-email`, `btn-save-client` |
| NIT duplicado | Crea un cliente y vuelve a intentar guardar el mismo NIT. | `input-nit`, `form-validation-summary` |
| Menor de edad | Intenta guardar natural menor de edad. | `input-fecha-nacimiento`, `form-validation-summary` |
| Editar cliente | Abre accion Editar y modifica datos. | Boton `Editar`, `input-email`, `btn-save-client` |
| Eliminar cliente | Abre accion Eliminar, confirma y valida ausencia en tabla. | Boton `Eliminar`, confirmacion del navegador, `input-search` |

Flujo Selenium cubierto:

1. Abrir la pagina principal.
2. Confirmar que la pantalla de clientes carga.
3. Buscar cliente por NIT.
4. Abrir el formulario de nuevo cliente.
5. Validar campos habilitados para persona natural.
6. Cambiar a persona juridica.
7. Validar que se deshabilitan campos naturales y se habilita razon social.
8. Escribir un NIT.
9. Esperar el calculo automatico del DV.
10. Confirmar que `input-dv` contiene el valor esperado.
11. Crear cliente natural y juridico desde UI.
12. Validar errores por NIT duplicado y menor de edad.
13. Editar un cliente creado desde UI.
14. Eliminar un cliente creado desde UI.

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

Para ver el navegador ejecutando los flujos, usar:

```powershell
$env:EFAC_SELENIUM_HEADLESS="false"
```

Para ejecutarlo oculto, usar:

```powershell
$env:EFAC_SELENIUM_HEADLESS="true"
```

Ejecutar ciclo QA automatico completo con evidencias:

```powershell
.\run-qa-tests.bat
```

El script realiza:

- Restauracion de paquetes.
- Compilacion de la solucion.
- Ejecucion de pruebas API.
- Levantamiento automatico de la WebAPI en `http://localhost:5100/`.
- Ejecucion de pruebas Selenium.
- Ejecucion de la suite completa.
- Cierre automatico de la WebAPI.
- Generacion de log y reportes TRX por ejecucion.
- Ejecuta Selenium en Chrome visible para permitir capturas del navegador llenando formularios.
- Mantiene la ventana abierta al finalizar para permitir capturas de pantalla.

Cada ejecucion crea una carpeta independiente:

```text
test-assets/evidence/reports/yyyyMMdd_HHmmss_qa-run
```

Dentro de esa carpeta quedan evidencias como:

```text
qa-execution.log
api-tests.trx
selenium-tests.trx
webapi.log
```

### Flujo Recomendado Para Capturas De Entrega

1. Abrir PowerShell o CMD en la raiz del proyecto.
2. Ejecutar:

```powershell
.\run-qa-tests.bat
```

3. Tomar captura del inicio de la ejecucion donde se vea el encabezado `EFAC - EJECUCION QA AUTOMATICA`.
4. Tomar captura de la seccion de compilacion donde se vea `Compilacion correcta`.
5. Tomar captura del resultado API donde se vea `Superado: 11`.
6. Tomar captura del resultado Selenium donde se vea `Superado: 10`.
7. Tomar captura del resumen final con `Restore: OK`, `Build: OK`, `Pruebas API: OK`, `Pruebas Selenium: OK` y `Suite completa: OK`.
8. Abrir la carpeta indicada en `Reportes TRX` y tomar captura de los archivos generados.
9. Cerrar la ventana manualmente presionando una tecla cuando ya se hayan tomado las capturas.

## Evidencias

Las evidencias de automatizacion se organizan en:

```text
test-assets/evidence/screenshots
test-assets/evidence/reports
```

Estas carpetas estan preparadas para capturas de pantalla, reportes y registros de ejecucion. Los archivos generados no deben versionarse salvo que sean evidencias finales requeridas por el equipo.

### Evidencias Incluidas En El Repositorio

El repositorio visible en GitHub deja evidencia de tres tipos: codigo, capturas sugeridas y resultados automatizados.

#### 1. Evidencia De Codigo

| Evidencia | Ubicacion | Que demuestra |
|---|---|---|
| Pruebas API | `Efac.Tests.Api/ClientesApiTests.cs` | Validacion automatizada de endpoints, codigos HTTP y reglas de negocio. |
| Pruebas UI Selenium | `Efac.Tests.Selenium/Tests/ClientesUiSmokeTests.cs` | Flujos de navegador: carga, busqueda, creacion, duplicado, menor de edad, edicion y eliminacion. |
| Page Object | `Efac.Tests.Selenium/Pages/ClientesPage.cs` | Reutilizacion de selectores, acciones de formulario, busqueda, edicion y eliminacion. |
| Configuracion Selenium | `Efac.Tests.Selenium/Infrastructure/WebDriverFactory.cs` | Ejecucion de Chrome visible o headless. |
| Configuracion de ejecucion | `Efac.Tests.Selenium/Infrastructure/SeleniumTestSettings.cs` | Variables `EFAC_RUN_SELENIUM`, `EFAC_BASE_URL` y `EFAC_SELENIUM_HEADLESS`. |
| Script de entrega | `run-qa-tests.bat` | Punto de entrada para ejecutar el ciclo QA desde Windows. |
| Script de automatizacion | `run-qa-tests.ps1` | Restore, build, WebAPI, Selenium, suite completa, logs y reportes. |
| Matriz QA | `QA_Matriz_Evidencias_Pruebas.md` | Trazabilidad entre requisitos, casos, campos, resultados esperados y evidencias. |

#### 2. Evidencia De Capturas

Las capturas no se versionan por defecto para evitar ruido en el repositorio, pero la documentacion indica exactamente cuales tomar durante la ejecucion:

| Captura | Momento recomendado |
|---|---|
| Inicio del script | Cuando aparezca `EFAC - EJECUCION QA AUTOMATICA`. |
| Build correcto | Cuando aparezca `Compilacion correcta`. |
| Pruebas API | Cuando aparezca `Superado: 11`. |
| Selenium visible | Mientras Chrome llena formularios, busca, edita y elimina clientes. |
| Pruebas Selenium | Cuando aparezca `Superado: 10`. |
| Resumen final | Cuando aparezcan todas las etapas en `OK`. |
| Carpeta de evidencias | Al abrir la ruta indicada en `Reportes TRX`. |

Ruta sugerida para guardar capturas finales:

```text
test-assets/evidence/screenshots
```

#### 3. Evidencia De Resultados

Cada ejecucion del script genera resultados en:

```text
test-assets/evidence/reports/yyyyMMdd_HHmmss_qa-run
```

Archivos esperados:

| Archivo | Que evidencia |
|---|---|
| `qa-execution.log` | Registro completo de comandos ejecutados y resultado por etapa. |
| `api-tests.trx` | Resultado formal de las 11 pruebas API. |
| `selenium-tests.trx` | Resultado formal de las 10 pruebas Selenium. |
| `*.trx` adicionales | Resultados de la suite completa sobre todos los proyectos de prueba. |
| `webapi.log` | Salida de la WebAPI durante la ejecucion automatizada. |

Resumen esperado de una ejecucion satisfactoria:

```text
Restore: OK
Build: OK
Pruebas API: OK
Pruebas Selenium: OK
Suite completa: OK
```

## Roadmap Tecnico

### Etapa 5 - Flujos UI CRUD Con Selenium Visible

Casos recomendados:

Estado: implementada.

Incluye:

- Creacion de cliente natural valido.
- Creacion de cliente juridico valido.
- Mensaje de error por menor de edad.
- Mensaje de error por NIT duplicado.
- Busqueda por NIT.
- Edicion de cliente.
- Eliminacion de cliente.
- Chrome visible configurable con `EFAC_SELENIUM_HEADLESS=false`.

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
