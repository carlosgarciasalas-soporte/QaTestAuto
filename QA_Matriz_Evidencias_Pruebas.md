# Matriz de Trazabilidad y Evidencias de Pruebas QA

Proyecto: Sistema CRUD de Clientes EFAC  
Tecnologia: ASP.NET Core Web API .NET 9, Razor Pages, Bootstrap, AJAX  
Alcance: Validaciones de aplicacion, reglas DIAN, reglas de negocio y pruebas manuales por Swagger/interfaz.

---

## A. Matriz de Trazabilidad de Pruebas

Formato ajustado para hoja vertical en Word. Recomendacion al pegar: usar fuente Arial 9 o Calibri 9 y opcion "Autoajustar a la ventana".

| ID | Requisito / Caso de prueba | Validacion | Esperado |
|---|---|---|---|
| REQ-DIAN-01 | Calcular DV DIAN. TC-DIAN-001: `GET /api/clientes/calcular-dv/800197268`. | Negocio DIAN/C# | HTTP 200. Retorna NIT normalizado y DV calculado. |
| REQ-DIAN-02 | Normalizar NIT. TC-DIAN-002: consultar DV con `800.197.268-4`. | Negocio DIAN/C# | HTTP 200. Retorna solo digitos en `nit`. |
| REQ-DIAN-03 | Rechazar NIT sin digitos. TC-DIAN-003: consultar DV con `ABC`. | Negocio DIAN/C# | HTTP 400. Error por NIT invalido. |
| REQ-CLI-01 | Crear cliente valido. TC-CLI-001: `POST /api/clientes` con persona natural mayor de edad. | Negocio C# / Persistencia | HTTP 201. Retorna `id`, `nit`, `dv` y datos del cliente. |
| REQ-CLI-02 | Evitar NIT duplicado. TC-CLI-002: crear dos clientes con el mismo NIT. | Negocio C# | Segundo POST retorna HTTP 409 por NIT existente. |
| REQ-CLI-03 | Listar clientes. TC-CLI-003: `GET /api/clientes`. | Aplicacion API | HTTP 200. Retorna arreglo JSON. |
| REQ-CLI-04 | Consultar por ID. TC-CLI-004: `GET /api/clientes/{id}` existente. | Aplicacion API | HTTP 200. Retorna el cliente solicitado. |
| REQ-CLI-05 | Cliente inexistente. TC-CLI-005: consultar GUID vacio. | Aplicacion API | HTTP 404. Error `El cliente no existe`. |
| REQ-CLI-06 | Actualizar cliente. TC-CLI-006: `PUT /api/clientes/{id}`. | Negocio C# / Persistencia | HTTP 200. Retorna datos actualizados. |
| REQ-CLI-07 | Eliminar cliente. TC-CLI-007: `DELETE /api/clientes/{id}`. | Aplicacion API | HTTP 204. Ya no aparece en el listado. |
| REQ-EDAD-01 | Bloquear menor de edad. TC-EDAD-001: nacimiento `2010-01-01`. | Negocio Edad/C# | HTTP 400. Error `El cliente debe ser mayor de edad`. |
| REQ-EDAD-02 | Fecha obligatoria para natural. TC-EDAD-002: `fechaNacimiento: null`. | Negocio Edad/C# | HTTP 400. Error por fecha obligatoria. |
| REQ-TIPO-01 | Nombres/apellidos obligatorios para natural. TC-TIPO-001: enviar vacios. | Negocio C# | HTTP 400. Error de nombres o apellidos. |
| REQ-TIPO-02 | Natural no persiste razon social. TC-TIPO-002: enviar `razonSocial`. | Negocio C# | HTTP 201. Respuesta con `razonSocial: null`. |
| REQ-TIPO-03 | Juridica requiere razon social. TC-TIPO-003: enviar nula/vacia. | Negocio C# | HTTP 400. Error por razon social obligatoria. |
| REQ-TIPO-04 | Juridica no persiste datos de natural. TC-TIPO-004: enviar nombres y fecha. | Negocio C# | HTTP 201. Nombres, apellidos y fecha quedan `null`. |
| REQ-UI-01 | Cargar tabla inicial. TC-UI-001: abrir `http://localhost:5100/`. | Aplicacion UI/AJAX | Muestra tabla con identificacion, DV, nombre, ciudad y acciones. |
| REQ-UI-02 | Buscar por NIT/nombre. TC-UI-002: escribir en `input-search`. | Aplicacion UI/AJAX | Filtra filas sin recargar pagina. |
| REQ-UI-03 | Abrir formulario. TC-UI-003: clic en `Nuevo Cliente`. | Aplicacion UI | Abre modal con campos identificables para QA. |
| REQ-UI-04 | Ocultar razon social en Natural. TC-UI-004: seleccionar `Natural`. | Aplicacion UI | Oculta/deshabilita `input-razon-social`. |
| REQ-UI-05 | Mostrar razon social en Juridica. TC-UI-005: seleccionar `Juridica`. | Aplicacion UI | Muestra razon social y oculta campos de natural. |
| REQ-UI-06 | Calcular DV en formulario. TC-UI-006: escribir NIT. | UI/AJAX + DIAN/C# | `input-dv` se llena con DV de la API. |
| REQ-UI-07 | Mostrar error de API en UI. TC-UI-007: guardar menor de edad. | UI/AJAX + Edad/C# | Muestra error de mayoria de edad en resumen. |
| REQ-SWAG-01 | Swagger disponible. TC-SWAG-001: abrir `/swagger`. | Aplicacion API | Swagger UI carga y expone endpoints. |

---

## B. Guia de Evidencias de Pruebas Manuales

### 1. Prueba de Endpoint en Swagger: Cliente Menor de Edad

Objetivo: validar que el backend bloquea el registro de personas naturales menores de 18 anos.

Precondiciones:

- Ejecutar el proyecto Web API en HTTP.
- Abrir Swagger en: `http://localhost:5100/swagger`.
- Confirmar que existe el endpoint `POST /api/clientes`.

Pasos:

1. Abrir `http://localhost:5100/swagger`.
2. Expandir el endpoint `POST /api/clientes`.
3. Hacer clic en `Try it out`.
4. Pegar el siguiente JSON en el request body:

```json
{
  "tipoPersona": 1,
  "nit": "1020304051",
  "nombres": "Carlos",
  "apellidos": "Menor QA",
  "razonSocial": null,
  "fechaNacimiento": "2010-01-01",
  "email": "menor.qa@example.com",
  "telefono": "3001234567",
  "direccion": "Calle 10 # 20-30",
  "ciudadCodigoMunicipio": "11001",
  "responsabilidadFiscal": 2
}
```

5. Hacer clic en `Execute`.
6. Capturar evidencia de pantalla del request y response.

Resultado esperado:

- Codigo HTTP: `400 Bad Request`.
- Respuesta esperada:

```json
{
  "error": "El cliente debe ser mayor de edad"
}
```

Criterio de aprobacion:

- La API no debe crear el cliente.
- El mensaje debe indicar claramente la regla de mayoria de edad.
- Al ejecutar `GET /api/clientes`, el NIT enviado no debe aparecer en la lista.

---

### 2. Prueba de Interfaz: Ocultar Razon Social para Persona Natural

Objetivo: validar que la interfaz diferencia correctamente los campos por tipo de persona.

Precondiciones:

- Ejecutar el proyecto Web API en HTTP.
- Abrir la interfaz en: `http://localhost:5100/`.

Pasos:

1. Abrir `http://localhost:5100/`.
2. Hacer clic en el boton `Nuevo Cliente`.
3. En el campo `Tipo de persona`, seleccionar `Natural`.
4. Verificar visualmente que los campos de persona natural quedan visibles:
   - `Nombres`
   - `Apellidos`
   - `Fecha de nacimiento`
5. Verificar que el campo `Razon social` no se muestra o queda deshabilitado.
6. Cambiar el campo `Tipo de persona` a `Juridica`.
7. Verificar que `Razon social` aparece y que los campos de persona natural se ocultan.
8. Capturar evidencia de pantalla en ambos estados: Natural y Juridica.

Resultado esperado:

- Persona Natural:
  - `input-nombres`: visible/habilitado.
  - `input-apellidos`: visible/habilitado.
  - `input-fecha-nacimiento`: visible/habilitado.
  - `input-razon-social`: oculto/deshabilitado.
- Persona Juridica:
  - `input-razon-social`: visible/habilitado.
  - Campos de persona natural: ocultos/deshabilitados.

Criterio de aprobacion:

- La interfaz evita capturar campos que no corresponden al tipo de persona seleccionado.
- La UI no requiere recarga de pagina para alternar los campos.

---

### 3. Simulaciones de Respuesta: Payloads JSON Exitosos y Fallidos

#### 3.1 Payload exitoso: Persona Natural mayor de edad

Endpoint: `POST /api/clientes`

Request:

```json
{
  "tipoPersona": 1,
  "nit": "123456789",
  "nombres": "Laura",
  "apellidos": "Gomez",
  "razonSocial": null,
  "fechaNacimiento": "1992-01-15",
  "email": "laura.gomez@example.com",
  "telefono": "3000000000",
  "direccion": "Calle 1 # 2-3",
  "ciudadCodigoMunicipio": "11001",
  "responsabilidadFiscal": 2
}
```

Respuesta esperada:

```json
{
  "id": "guid-generado-por-el-sistema",
  "tipoPersona": 1,
  "nit": "123456789",
  "dv": 6,
  "nombres": "Laura",
  "apellidos": "Gomez",
  "razonSocial": null,
  "nombreCompleto": "Laura Gomez",
  "fechaNacimiento": "1992-01-15",
  "email": "laura.gomez@example.com",
  "telefono": "3000000000",
  "direccion": "Calle 1 # 2-3",
  "ciudadCodigoMunicipio": "11001",
  "responsabilidadFiscal": 2
}
```

Codigo esperado: `201 Created`.

---

#### 3.2 Payload exitoso: Persona Juridica

Endpoint: `POST /api/clientes`

Request:

```json
{
  "tipoPersona": 2,
  "nit": "900373914",
  "nombres": "No debe persistir",
  "apellidos": "No debe persistir",
  "razonSocial": "Empresa QA S.A.S.",
  "fechaNacimiento": "1990-01-01",
  "email": "contacto@empresaqa.com",
  "telefono": "6015550000",
  "direccion": "Avenida 100 # 10-20",
  "ciudadCodigoMunicipio": "11001",
  "responsabilidadFiscal": 1
}
```

Respuesta esperada:

```json
{
  "id": "guid-generado-por-el-sistema",
  "tipoPersona": 2,
  "nit": "900373914",
  "dv": 2,
  "nombres": null,
  "apellidos": null,
  "razonSocial": "Empresa QA S.A.S.",
  "nombreCompleto": "Empresa QA S.A.S.",
  "fechaNacimiento": null,
  "email": "contacto@empresaqa.com",
  "telefono": "6015550000",
  "direccion": "Avenida 100 # 10-20",
  "ciudadCodigoMunicipio": "11001",
  "responsabilidadFiscal": 1
}
```

Codigo esperado: `201 Created`.

---

#### 3.3 Payload fallido: Persona Natural menor de edad

Endpoint: `POST /api/clientes`

Request:

```json
{
  "tipoPersona": 1,
  "nit": "1020304051",
  "nombres": "Carlos",
  "apellidos": "Menor QA",
  "razonSocial": null,
  "fechaNacimiento": "2010-01-01",
  "email": "menor.qa@example.com",
  "telefono": "3001234567",
  "direccion": "Calle 10 # 20-30",
  "ciudadCodigoMunicipio": "11001",
  "responsabilidadFiscal": 2
}
```

Respuesta esperada:

```json
{
  "error": "El cliente debe ser mayor de edad"
}
```

Codigo esperado: `409 Conflict`.

---

#### 3.4 Payload fallido: Persona Juridica sin razon social

Endpoint: `POST /api/clientes`

Request:

```json
{
  "tipoPersona": 2,
  "nit": "900373915",
  "nombres": null,
  "apellidos": null,
  "razonSocial": null,
  "fechaNacimiento": null,
  "email": "sinrazon@example.com",
  "telefono": "6015551111",
  "direccion": "Carrera 20 # 30-40",
  "ciudadCodigoMunicipio": "05001",
  "responsabilidadFiscal": 1
}
```

Respuesta esperada:

```json
{
  "error": "La razon social es obligatoria para persona juridica"
}
```

Codigo esperado: `400 Bad Request`.

---

#### 3.5 Payload fallido: NIT duplicado

Endpoint: `POST /api/clientes`

Paso 1: crear un cliente valido con NIT `123456789`.

Paso 2: intentar crear otro cliente con el mismo NIT:

```json
{
  "tipoPersona": 1,
  "nit": "123456789",
  "nombres": "Duplicado",
  "apellidos": "QA",
  "razonSocial": null,
  "fechaNacimiento": "1991-05-20",
  "email": "duplicado@example.com",
  "telefono": "3009999999",
  "direccion": "Calle 99 # 1-2",
  "ciudadCodigoMunicipio": "11001",
  "responsabilidadFiscal": 2
}
```

Respuesta esperada:

```json
{
  "error": "Ya existe un cliente registrado con el NIT indicado"
}
```

Codigo esperado: `400 Bad Request`.

---

#### 3.6 Payload fallido: Persona Natural sin fecha de nacimiento

Endpoint: `POST /api/clientes`

Request:

```json
{
  "tipoPersona": 1,
  "nit": "1020304052",
  "nombres": "Ana",
  "apellidos": "Sin Fecha",
  "razonSocial": null,
  "fechaNacimiento": null,
  "email": "ana.sinfecha@example.com",
  "telefono": "3002223333",
  "direccion": "Calle 50 # 60-70",
  "ciudadCodigoMunicipio": "76001",
  "responsabilidadFiscal": 2
}
```

Respuesta esperada:

```json
{
  "error": "La fecha de nacimiento es obligatoria para persona natural"
}
```

Codigo esperado: `400 Bad Request`.

---

## Checklist de Evidencias Recomendadas

| Evidencia | Descripcion | Estado |
|---|---|---|
| EV-001 | Captura de Swagger cargado en `http://localhost:5100/swagger`. | Pendiente |
| EV-002 | Captura de `POST /api/clientes` exitoso para persona natural. | Pendiente |
| EV-003 | Captura de `POST /api/clientes` fallido por menor de edad. | Pendiente |
| EV-004 | Captura de `POST /api/clientes` fallido por NIT duplicado. | Pendiente |
| EV-005 | Captura de interfaz con Persona Natural y razon social oculta. | Pendiente |
| EV-006 | Captura de interfaz con Persona Juridica y razon social visible. | Pendiente |
| EV-007 | Captura de busqueda por NIT o nombre en la tabla principal. | Pendiente |
| EV-008 | Captura de eliminacion con confirmacion previa. | Pendiente |

---

## Observaciones QA

- Las validaciones de aplicacion se verifican principalmente en la UI, el formato de llamada HTTP, Swagger y la comunicacion AJAX.
- Las validaciones de negocio se verifican en la capa C# de dominio/aplicacion: calculo DV, mayoria de edad, campos por tipo de persona y duplicidad de NIT.
- Para evitar falsos positivos por NIT duplicado, usar NIT diferentes en cada ejecucion de pruebas o eliminar los registros temporales al finalizar.
- El proyecto esta configurado para depuracion HTTP en `http://localhost:5100`.
