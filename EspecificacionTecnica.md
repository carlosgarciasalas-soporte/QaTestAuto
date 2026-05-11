# Especificación Técnica: Sistema CRUD de Clientes (DIAN Compliance)

## 1. Arquitectura y Tecnologías
- **Framework:** ASP.NET Core (.NET 9).
- **Patrón:** Clean Architecture con Web API integrada.
- **Frontend:** Razor Pages + Bootstrap 5 + AJAX (para evitar recargas).

## 2. Requerimientos Funcionales (CRUD Completo)
El agente debe implementar las 4 operaciones básicas sobre la entidad `Cliente`:

1.  **LECTURA (Read):** Al cargar la página principal, se debe mostrar una **Tabla de Datos** con todos los clientes registrados (Identificación, DV, Nombre/Razón Social, Ciudad).
2.  **CREACIÓN (Create):** Un botón "Nuevo Cliente" que despliegue un formulario (o modal) con las validaciones DIAN (NIT, DV automático, Mayoría de edad).
3.  **EDICIÓN (Update):** Un botón "Editar" en cada fila de la tabla que cargue los datos del cliente en el formulario para su modificación.
4.  **ELIMINACIÓN (Delete):** Un botón "Eliminar" con una alerta de confirmación previa.
5.  **CONSULTA (Search/Filter):** Un campo de búsqueda para filtrar la tabla por NIT o Nombre.

## 3. Lógica de Negocio (Capa de Aplicación)
- **Cálculo de DV:** El sistema debe calcular el Dígito de Verificación (Módulo 11) en tiempo real o antes de guardar.
- **Validación de Edad:** Bloquear el registro de personas naturales menores de 18 años.
- **Diferenciación de Campos:** Habilitar 'Razón Social' solo para Jurídicas y 'Nombres/Apellidos' solo para Naturales.

## 4. Diseño de la Interfaz (UI/UX para QA)
- **Vista Principal:** Dashboard con una tabla de Bootstrap.
- **Acciones:** Cada fila debe tener iconos/botones claros: 👁️ (Ver), ✏️ (Editar), 🗑️ (Eliminar).
- **Formulario:** Utilizar etiquetas `asp-for` y `id` específicos (ej: `id="input-nit"`) para facilitar el uso de **Selenium** en el futuro.
- **Validaciones:** Los errores de la API (como el de mayoría de edad) deben mostrarse en un `Summary` de validación al inicio del formulario.

## 5. Endpoints de la API (Para Pruebas Manuales)
El backend debe exponer los siguientes endpoints testeables en Swagger:
- `GET /api/clientes`: Retorna la lista completa.
- `POST /api/clientes`: Crea un nuevo registro (Valida negocio).
- `PUT /api/clientes/{id}`: Actualiza un registro existente.
- `DELETE /api/clientes/{id}`: Elimina un registro.

## 6. Instrucciones para el Agente IA
1. Implementar la persistencia (usar `Entity Framework Core` con `InMemory` o `SQLite` para que el CRUD sea funcional de inmediato).
2. Asegurar que los DTOs separen la información de entrada de la de salida.
3. Configurar Swagger para que el usuario pueda probar los endpoints de forma manual antes de usar la interfaz web.