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
$env:EFAC_SELENIUM_HEADLESS="false"
dotnet test Efac.Tests.Selenium --no-build
```

Ejecucion automatizada para entrega y evidencias:

```powershell
.\run-qa-tests.bat
```

El script crea una carpeta por ciclo en `test-assets/evidence/reports`, muestra el avance en pantalla y guarda el archivo `qa-execution.log` junto con reportes TRX.

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
- Fabrica `WebDriverFactory` preparada para Chrome visible o headless.
- Prueba de carga de pagina principal.
- Prueba de busqueda por NIT.
- Prueba de alternancia de campos Natural/Juridica.
- Prueba de calculo de DV desde la UI usando la API.
- Prueba de creacion de persona natural desde UI.
- Prueba de creacion de persona juridica desde UI.
- Prueba de rechazo por NIT duplicado desde UI.
- Prueba de rechazo por menor de edad desde UI.
- Prueba de edicion de cliente desde UI.
- Prueba de eliminacion de cliente desde UI.

Resultado actual:

- `Efac.Tests.Selenium`: 10 pruebas superadas.

## Criterio de avance

Cada etapa debe cerrar con:

- Compilacion sin errores.
- Pruebas automatizadas ejecutadas.
- Evidencia de resultado.
- Actualizacion de este plan.

## Etapa 4 - Evidencias automatizadas

Estado: implementada.

Incluye:

- Script `run-qa-tests.bat` como punto de entrada para Windows.
- Script auxiliar `run-qa-tests.ps1` para controlar logs, procesos y errores.
- Carpeta de evidencia independiente por ejecucion.
- Log de consola persistido en `qa-execution.log`.
- Reportes TRX para pruebas API, Selenium y suite completa.
- Arranque y cierre automatico de la WebAPI local.
- Pausa final en `run-qa-tests.bat` para que el usuario tome capturas antes de cerrar la ventana.
- Documentacion de flujo automatico, campos probados y evidencias esperadas.
- Selenium visible por defecto en el script de entrega mediante `EFAC_SELENIUM_HEADLESS=false`.

## Paso a paso para ejecutar el ciclo automatico

1. Abrir CMD o PowerShell en la raiz del proyecto.
2. Ejecutar:

```powershell
.\run-qa-tests.bat
```

3. Esperar que el script muestre el encabezado `EFAC - EJECUCION QA AUTOMATICA`.
4. Confirmar en pantalla las etapas:
   - `Restaurando paquetes NuGet`
   - `Compilando solucion`
   - `Ejecutando pruebas API`
   - `Levantando WebAPI`
   - `Ejecutando pruebas Selenium`
   - `Ejecutando suite completa`
5. Tomar capturas de pantalla de los resultados relevantes.
6. Revisar la ruta indicada en `Reportes TRX`.
7. Presionar una tecla para cerrar la ventana cuando ya se hayan tomado las evidencias.

## Evidencias automaticas esperadas

Cada ejecucion crea una carpeta con formato:

```text
test-assets/evidence/reports/yyyyMMdd_HHmmss_qa-run
```

Contenido esperado:

- `qa-execution.log`: log completo visible tambien en consola.
- `api-tests.trx`: reporte de pruebas API.
- `selenium-tests.trx`: reporte de pruebas Selenium.
- Reportes `.trx` adicionales de la suite completa.
- `webapi.log`: salida de la WebAPI durante el ciclo automatico.

## Cobertura automatizada documentada

API:

- Listado de clientes.
- Consulta de cliente inexistente.
- Calculo y normalizacion de DV.
- Rechazo de NIT invalido.
- Creacion de persona natural valida.
- Rechazo de NIT duplicado.
- Rechazo de menor de edad.
- Rechazo de natural sin fecha de nacimiento.
- Rechazo de juridica sin razon social.
- Actualizacion de cliente.
- Eliminacion de cliente.

UI Selenium:

- Carga de pantalla principal.
- Busqueda por NIT.
- Apertura de modal de cliente.
- Alternancia de campos entre Natural y Juridica.
- Calculo de DV desde el formulario.
- Creacion de persona natural.
- Creacion de persona juridica.
- Validacion de NIT duplicado.
- Validacion de menor de edad.
- Edicion de cliente.
- Eliminacion de cliente.

Campos principales validados:

- `tipoPersona`
- `nit`
- `dv`
- `nombres`
- `apellidos`
- `fechaNacimiento`
- `razonSocial`
- `email`
- `telefono`
- `direccion`
- `ciudadCodigoMunicipio`
- `responsabilidadFiscal`
