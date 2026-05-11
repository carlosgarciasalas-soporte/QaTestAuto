from __future__ import annotations

import subprocess
from datetime import datetime
from pathlib import Path

from reportlab.lib import colors
from reportlab.lib.enums import TA_CENTER, TA_LEFT
from reportlab.lib.pagesizes import letter
from reportlab.lib.styles import ParagraphStyle, getSampleStyleSheet
from reportlab.lib.units import cm
from reportlab.platypus import (
    PageBreak,
    Paragraph,
    SimpleDocTemplate,
    Spacer,
    Table,
    TableStyle,
)


ROOT = Path(__file__).resolve().parents[1]
OUT_DIR = ROOT / "docs"
PDF_PATH = OUT_DIR / "Informe_Consolidado_QA_EFAC.pdf"

NAVY = colors.HexColor("#1F4E79")
TEAL = colors.HexColor("#0F766E")
GRAY = colors.HexColor("#64748B")
LIGHT_BLUE = colors.HexColor("#D9EAF7")
LIGHT_TEAL = colors.HexColor("#DDF3F0")
LIGHT_GRAY = colors.HexColor("#F2F4F7")
GRID = colors.HexColor("#D0D7DE")


def git_value(*args: str) -> str:
    try:
        return subprocess.check_output(["git", *args], cwd=ROOT, text=True).strip()
    except Exception:
        return "No disponible"


def p(text: str, style: ParagraphStyle):
    return Paragraph(text.replace("\n", "<br/>"), style)


def table(data, widths=None, header=True):
    rows = []
    for row in data:
        rows.append([p(str(cell), STYLES["TableCell"]) for cell in row])

    tbl = Table(rows, colWidths=widths, hAlign="CENTER", repeatRows=1 if header else 0)
    commands = [
        ("GRID", (0, 0), (-1, -1), 0.4, GRID),
        ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
        ("LEFTPADDING", (0, 0), (-1, -1), 6),
        ("RIGHTPADDING", (0, 0), (-1, -1), 6),
        ("TOPPADDING", (0, 0), (-1, -1), 5),
        ("BOTTOMPADDING", (0, 0), (-1, -1), 5),
    ]
    if header:
        commands.extend(
            [
                ("BACKGROUND", (0, 0), (-1, 0), LIGHT_BLUE),
                ("TEXTCOLOR", (0, 0), (-1, 0), NAVY),
                ("FONTNAME", (0, 0), (-1, 0), "Helvetica-Bold"),
            ]
        )
    for idx in range(1 if header else 0, len(data)):
        if idx % 2 == 0:
            commands.append(("BACKGROUND", (0, idx), (-1, idx), colors.HexColor("#FAFBFC")))
    tbl.setStyle(TableStyle(commands))
    return tbl


def callout(title: str, body: str):
    data = [[p(f"<b>{title}</b><br/>{body}", STYLES["Callout"])]]
    tbl = Table(data, colWidths=[16.5 * cm], hAlign="CENTER")
    tbl.setStyle(
        TableStyle(
            [
                ("BACKGROUND", (0, 0), (-1, -1), LIGHT_TEAL),
                ("BOX", (0, 0), (-1, -1), 0.5, TEAL),
                ("LEFTPADDING", (0, 0), (-1, -1), 10),
                ("RIGHTPADDING", (0, 0), (-1, -1), 10),
                ("TOPPADDING", (0, 0), (-1, -1), 8),
                ("BOTTOMPADDING", (0, 0), (-1, -1), 8),
            ]
        )
    )
    return tbl


def bullets(items: list[str]):
    story = []
    for item in items:
        story.append(p(f"• {item}", STYLES["Body"]))
    return story


def numbered(items: list[str]):
    story = []
    for idx, item in enumerate(items, start=1):
        story.append(p(f"{idx}. {item}", STYLES["Body"]))
    return story


def heading(text: str, level: int = 1):
    return p(text, STYLES[f"QAHeading{level}"])


def spacer(height=0.18 * cm):
    return Spacer(1, height)


def on_page(canvas, doc):
    canvas.saveState()
    canvas.setFont("Helvetica", 8)
    canvas.setFillColor(GRAY)
    canvas.drawString(1.6 * cm, 1.05 * cm, "EFAC - Informe consolidado QA")
    canvas.drawRightString(20.0 * cm, 1.05 * cm, f"Pagina {doc.page}")
    canvas.restoreState()


def build_styles():
    styles = getSampleStyleSheet()
    styles.add(
        ParagraphStyle(
            "CoverTitle",
            parent=styles["Title"],
            alignment=TA_CENTER,
            fontName="Helvetica-Bold",
            fontSize=24,
            leading=29,
            textColor=NAVY,
            spaceAfter=12,
        )
    )
    styles.add(
        ParagraphStyle(
            "CoverSubtitle",
            parent=styles["Normal"],
            alignment=TA_CENTER,
            fontSize=12.5,
            leading=16,
            textColor=TEAL,
            spaceAfter=18,
        )
    )
    styles.add(
        ParagraphStyle(
            "QAHeading1",
            parent=styles["Heading1"],
            fontName="Helvetica-Bold",
            fontSize=16,
            leading=20,
            textColor=NAVY,
            spaceBefore=10,
            spaceAfter=6,
        )
    )
    styles.add(
        ParagraphStyle(
            "QAHeading2",
            parent=styles["Heading2"],
            fontName="Helvetica-Bold",
            fontSize=12.5,
            leading=16,
            textColor=TEAL,
            spaceBefore=8,
            spaceAfter=4,
        )
    )
    styles.add(
        ParagraphStyle(
            "QAHeading3",
            parent=styles["Heading3"],
            fontName="Helvetica-Bold",
            fontSize=10.5,
            leading=13,
            textColor=GRAY,
            spaceBefore=6,
            spaceAfter=3,
        )
    )
    styles.add(
        ParagraphStyle(
            "Body",
            parent=styles["BodyText"],
            fontName="Helvetica",
            fontSize=9.7,
            leading=12.4,
            alignment=TA_LEFT,
            spaceAfter=4,
        )
    )
    styles.add(
        ParagraphStyle(
            "TableCell",
            parent=styles["BodyText"],
            fontName="Helvetica",
            fontSize=8.5,
            leading=10.4,
            alignment=TA_LEFT,
        )
    )
    styles.add(
        ParagraphStyle(
            "Callout",
            parent=styles["BodyText"],
            fontName="Helvetica",
            fontSize=9.2,
            leading=12,
            textColor=colors.HexColor("#1F2933"),
        )
    )
    return styles


STYLES = build_styles()


def build_story():
    commit = git_value("rev-parse", "--short", "HEAD")
    branch = git_value("branch", "--show-current")
    story = []

    story.append(Spacer(1, 1.2 * cm))
    story.append(p("Informe Consolidado QA<br/>Sistema CRUD de Clientes EFAC", STYLES["CoverTitle"]))
    story.append(p("Analisis del proyecto, estrategia de pruebas, automatizacion y evidencias", STYLES["CoverSubtitle"]))
    story.append(
        table(
            [
                ["Dato", "Valor"],
                ["Proyecto", "EFAC QA Testing y Automatizacion"],
                ["Tecnologia", "ASP.NET Core .NET 9, Razor Pages, Web API, xUnit, Selenium"],
                ["Repositorio", "https://github.com/carlosgarciasalas-soporte/QaTestAuto.git"],
                ["Rama / Commit", f"{branch} / {commit}"],
                ["Fecha de consolidacion", datetime.now().strftime("%Y-%m-%d %H:%M")],
            ],
            [4.2 * cm, 12.2 * cm],
        )
    )
    story.append(spacer())
    story.append(
        callout(
            "Proposito del documento",
            "Consolidar en un solo PDF la informacion relevante del README, la matriz de evidencias, "
            "el plan de automatizacion y el estado actual de pruebas automaticas del proyecto EFAC.",
        )
    )
    story.append(PageBreak())

    story.append(heading("1. Resumen ejecutivo"))
    story.append(
        p(
            "EFAC es un sistema CRUD de clientes construido con ASP.NET Core .NET 9. "
            "Administra clientes bajo reglas de negocio asociadas a cumplimiento DIAN: normalizacion de NIT, "
            "calculo de digito de verificacion, control de duplicidad, validacion de mayoria de edad y "
            "diferenciacion entre persona natural y juridica.",
            STYLES["Body"],
        )
    )
    story.append(
        callout(
            "Estado general",
            "El proyecto cuenta con pruebas automatizadas de API y UI, script de ejecucion completo para Windows, "
            "reportes TRX, log persistente y documentacion de evidencias. La ultima validacion consolidada reporta "
            "11 pruebas API superadas y 10 pruebas Selenium superadas.",
        )
    )
    story.append(spacer())
    story.append(
        table(
            [
                ["Area", "Resultado consolidado"],
                ["Backend/API", "Endpoints CRUD y calculo DV disponibles en /api/clientes."],
                ["Frontend", "Interfaz Razor Pages con IDs estables para automatizacion Selenium."],
                ["Persistencia", "Entity Framework Core InMemory para ejecucion academica y pruebas."],
                ["Automatizacion API", "11 pruebas xUnit con WebApplicationFactory y FluentAssertions."],
                ["Automatizacion UI", "10 pruebas Selenium con Chrome visible o headless."],
                ["Evidencias", "Log QA, reportes TRX, guia de capturas y matriz de trazabilidad."],
            ],
            [4.0 * cm, 12.5 * cm],
        )
    )

    story.append(heading("2. Alcance y fuentes consolidadas"))
    story.append(
        p(
            "Este informe integra la informacion operativa y tecnica necesaria para revisar el proyecto, "
            "analizar su cobertura de pruebas y ejecutar el ciclo QA automatizado.",
            STYLES["Body"],
        )
    )
    story.append(
        table(
            [
                ["Fuente", "Contenido integrado"],
                ["README.md", "Descripcion, arquitectura, reglas, ejecucion, automatizacion y evidencias."],
                ["QA_Matriz_Evidencias_Pruebas.md", "Trazabilidad, casos manuales, evidencias, campos y resultados esperados."],
                ["AUTOMATIZACION_PLAN.md", "Etapas de automatizacion, criterios de avance y cobertura automatizada."],
                ["Codigo de pruebas", "Casos API y Selenium implementados en los proyectos de prueba."],
                ["Scripts QA", "Ejecucion automatica con run-qa-tests.bat y run-qa-tests.ps1."],
            ],
            [5.4 * cm, 11.1 * cm],
        )
    )

    story.append(heading("3. Vision tecnica del proyecto"))
    story.append(heading("3.1 Arquitectura", 2))
    story.append(
        table(
            [
                ["Proyecto", "Responsabilidad"],
                ["Efac.Domain", "Entidad Cliente, enums, excepciones y calculo DIAN modulo 11."],
                ["Efac.Application", "DTOs, servicios de caso de uso, validaciones y contratos."],
                ["Efac.Infrastructure", "DbContext, repositorio e inyeccion de dependencias de persistencia."],
                ["Efac.WebAPI", "Controladores REST, Swagger, Razor Pages e interfaz web."],
                ["Efac.Tests.Api", "Pruebas automatizadas de endpoints y reglas de negocio."],
                ["Efac.Tests.Selenium", "Pruebas E2E de interfaz con navegador."],
                ["test-assets", "Estructura para evidencias de ejecucion, capturas y reportes."],
            ],
            [4.5 * cm, 12.0 * cm],
        )
    )
    story.append(heading("3.2 Endpoints disponibles", 2))
    story.append(
        table(
            [
                ["Metodo", "Ruta", "Uso"],
                ["GET", "/api/clientes", "Listar clientes."],
                ["GET", "/api/clientes/{id}", "Consultar cliente por identificador."],
                ["GET", "/api/clientes/calcular-dv/{nit}", "Normalizar NIT y calcular DV."],
                ["POST", "/api/clientes", "Crear cliente natural o juridico."],
                ["PUT", "/api/clientes/{id}", "Actualizar cliente existente."],
                ["DELETE", "/api/clientes/{id}", "Eliminar cliente existente."],
            ],
            [2.3 * cm, 6.3 * cm, 7.9 * cm],
        )
    )
    story.append(heading("3.3 Reglas de negocio principales", 2))
    story.extend(
        bullets(
            [
                "El NIT se normaliza antes de persistir, buscar o calcular el DV.",
                "El digito de verificacion se calcula con algoritmo DIAN modulo 11.",
                "No se permite registrar dos clientes con el mismo NIT.",
                "La persona natural requiere nombres, apellidos y fecha de nacimiento.",
                "La persona natural menor de edad debe ser rechazada.",
                "La persona juridica requiere razon social.",
                "Los campos no aplicables al tipo de persona no deben persistirse.",
            ]
        )
    )

    story.append(heading("4. Estrategia QA"))
    story.append(
        table(
            [
                ["Nivel", "Herramienta", "Objetivo"],
                ["Prueba manual API", "Swagger", "Validar contratos y payloads durante exploracion manual."],
                ["Prueba automatizada API", "xUnit + WebApplicationFactory", "Validar reglas HTTP, negocio y persistencia en memoria."],
                ["Prueba E2E UI", "Selenium WebDriver", "Simular flujos reales de usuario en navegador."],
                ["Evidencias", "Logs, TRX y capturas", "Sustentar ejecucion y resultado de pruebas."],
                ["Orquestacion", "run-qa-tests.bat / .ps1", "Ejecutar restore, build, API, UI, suite completa y reportes."],
            ],
            [3.8 * cm, 4.8 * cm, 7.9 * cm],
        )
    )

    story.append(heading("5. Cobertura automatizada"))
    story.append(heading("5.1 Pruebas API", 2))
    story.append(
        table(
            [
                ["ID", "Caso", "Resultado esperado"],
                ["API-001", "Listar clientes", "200 OK con datos semilla."],
                ["API-002", "Consultar cliente inexistente", "404 Not Found."],
                ["API-003", "Calcular DV con NIT formateado", "200 OK, NIT normalizado y DV calculado."],
                ["API-004", "Rechazar NIT invalido", "400 Bad Request."],
                ["API-005", "Crear persona natural valida", "201 Created."],
                ["API-006", "Rechazar NIT duplicado", "409 Conflict."],
                ["API-007", "Rechazar menor de edad", "400 Bad Request."],
                ["API-008", "Rechazar natural sin fecha", "400 Bad Request."],
                ["API-009", "Rechazar juridica sin razon social", "400 Bad Request."],
                ["API-010", "Actualizar cliente", "200 OK con datos actualizados."],
                ["API-011", "Eliminar cliente", "204 No Content y posterior 404."],
            ],
            [2.2 * cm, 6.7 * cm, 7.6 * cm],
        )
    )

    story.append(heading("5.2 Pruebas Selenium", 2))
    story.append(
        table(
            [
                ["ID", "Caso", "Controles o flujo validado"],
                ["UI-001", "Carga principal", "Titulo, input-search y btn-new-client."],
                ["UI-002", "Busqueda por NIT", "input-search y tabla de clientes."],
                ["UI-003", "Alternancia Natural/Juridica", "Campos naturales y razon social."],
                ["UI-004", "Calculo DV", "input-nit, input-dv y llamada a API."],
                ["UI-005", "Crear natural", "Formulario completo y aparicion en tabla."],
                ["UI-006", "Crear juridica", "Tipo persona, razon social y guardado."],
                ["UI-007", "NIT duplicado", "form-validation-summary con error esperado."],
                ["UI-008", "Menor de edad", "fechaNacimiento y mensaje de error."],
                ["UI-009", "Editar cliente", "Accion Editar y persistencia del cambio."],
                ["UI-010", "Eliminar cliente", "Accion Eliminar, confirmacion y ausencia en tabla."],
            ],
            [2.2 * cm, 5.7 * cm, 8.6 * cm],
        )
    )

    story.append(heading("5.3 Campos validados", 2))
    story.append(
        table(
            [
                ["Campo", "Validacion"],
                ["tipoPersona", "Diferencia Natural y Juridica; activa/oculta campos segun tipo."],
                ["nit", "Normalizacion, busqueda, duplicidad y calculo de DV."],
                ["dv", "Calculo DIAN modulo 11."],
                ["nombres / apellidos", "Obligatorios y visibles para persona natural."],
                ["fechaNacimiento", "Obligatoria para natural; valida mayoria de edad."],
                ["razonSocial", "Obligatoria y visible para persona juridica."],
                ["email / telefono / direccion", "Persistencia en creacion y actualizacion."],
                ["ciudadCodigoMunicipio", "Persistencia del municipio."],
                ["responsabilidadFiscal", "Persistencia del regimen fiscal seleccionado."],
            ],
            [5.2 * cm, 11.3 * cm],
        )
    )

    story.append(heading("6. Flujo de ejecucion y evidencias"))
    story.append(heading("6.1 Ejecucion automatica", 2))
    story.extend(
        numbered(
            [
                "Abrir CMD o PowerShell en la raiz del proyecto.",
                "Ejecutar .\\run-qa-tests.bat.",
                "Verificar el encabezado EFAC - EJECUCION QA AUTOMATICA.",
                "Esperar restore, build, pruebas API, WebAPI local, Selenium y suite completa.",
                "Tomar capturas del navegador y consola durante la ejecucion.",
                "Revisar la carpeta indicada en Reportes TRX.",
                "Presionar una tecla para cerrar la ventana al finalizar.",
            ]
        )
    )
    story.append(heading("6.2 Archivos generados", 2))
    story.append(
        table(
            [
                ["Archivo", "Evidencia"],
                ["qa-execution.log", "Registro completo de comandos, etapas y resultados."],
                ["api-tests.trx", "Resultado formal de las 11 pruebas API."],
                ["selenium-tests.trx", "Resultado formal de las 10 pruebas Selenium."],
                ["*.trx adicionales", "Resultados de la suite completa."],
                ["webapi.log", "Salida de la WebAPI durante el ciclo automatico."],
            ],
            [5.2 * cm, 11.3 * cm],
        )
    )
    story.append(heading("6.3 Capturas recomendadas", 2))
    story.extend(
        bullets(
            [
                "Inicio del script con el encabezado de ejecucion automatica.",
                "Compilacion correcta.",
                "Pruebas API con Superado: 11.",
                "Chrome visible llenando formularios, buscando, editando y eliminando clientes.",
                "Errores visibles por NIT duplicado y menor de edad.",
                "Pruebas Selenium con Superado: 10.",
                "Resumen final con Restore, Build, API, Selenium y Suite completa en OK.",
                "Carpeta generada en test-assets/evidence/reports.",
            ]
        )
    )

    story.append(heading("7. Evidencia visible en GitHub"))
    story.append(
        table(
            [
                ["Tipo", "Archivo", "Evidencia"],
                ["Codigo", "Efac.Tests.Api/ClientesApiTests.cs", "Casos API y reglas de negocio."],
                ["Codigo", "Efac.Tests.Selenium/Tests/ClientesUiSmokeTests.cs", "Flujos UI CRUD y validaciones negativas."],
                ["Codigo", "Efac.Tests.Selenium/Pages/ClientesPage.cs", "Page Object y acciones de navegador."],
                ["Codigo", "run-qa-tests.bat", "Entrada de ejecucion para Windows con pausa final."],
                ["Codigo", "run-qa-tests.ps1", "Orquestacion completa y generacion de evidencias."],
                ["Documentacion", "README.md", "Guia de proyecto, pruebas, ejecucion y evidencias."],
                ["Documentacion", "QA_Matriz_Evidencias_Pruebas.md", "Trazabilidad y checklist de evidencias."],
                ["Documentacion", "AUTOMATIZACION_PLAN.md", "Plan por etapas y cobertura."],
            ],
            [3.0 * cm, 6.0 * cm, 7.5 * cm],
        )
    )

    story.append(heading("8. Analisis del proyecto"))
    story.append(heading("8.1 Fortalezas", 2))
    story.extend(
        bullets(
            [
                "Separacion clara por capas y por proyectos de prueba.",
                "Cobertura API alineada con reglas de negocio criticas.",
                "Selenium visible permite evidencias demostrativas durante la entrega.",
                "El Page Object reduce fragilidad y centraliza selectores.",
                "El script automatico deja trazabilidad por ejecucion mediante logs y TRX.",
                "La documentacion conecta requisitos, casos, campos y evidencia.",
            ]
        )
    )
    story.append(heading("8.2 Riesgos y consideraciones", 2))
    story.extend(
        bullets(
            [
                "La persistencia InMemory es adecuada para entorno academico, pero no reemplaza pruebas contra base de datos real.",
                "Las capturas deben tomarse manualmente durante ejecucion visible si se requieren como evidencia grafica final.",
                "Selenium visible depende de Chrome/ChromeDriver disponible en el equipo.",
                "Para despliegue productivo se recomienda una plataforma compatible con ASP.NET Core, no Vercel directamente.",
            ]
        )
    )
    story.append(heading("9. Conclusiones"))
    story.append(
        p(
            "El proyecto EFAC presenta una base tecnica consistente para pruebas y calidad de software. "
            "La automatizacion cubre escenarios funcionales clave de API y UI, incluyendo casos positivos, "
            "validaciones negativas, CRUD visual con Selenium y generacion de evidencias. La estructura documental "
            "y los scripts permiten reproducir el ciclo QA de forma clara, auditable y apta para entrega academica.",
            STYLES["Body"],
        )
    )

    story.append(PageBreak())
    story.append(heading("Anexo A. Comandos principales"))
    story.append(
        table(
            [
                ["Objetivo", "Comando"],
                ["Restaurar", "dotnet restore Efac.sln"],
                ["Compilar", "dotnet build Efac.sln --no-restore"],
                ["Pruebas API", "dotnet test Efac.Tests.Api --no-build"],
                ["Levantar WebAPI", "dotnet run --project Efac.WebAPI --launch-profile http"],
                ["Activar Selenium", "$env:EFAC_RUN_SELENIUM=\"true\""],
                ["Base URL Selenium", "$env:EFAC_BASE_URL=\"http://localhost:5100/\""],
                ["Chrome visible", "$env:EFAC_SELENIUM_HEADLESS=\"false\""],
                ["Pruebas Selenium", "dotnet test Efac.Tests.Selenium --no-build"],
                ["Ciclo completo", ".\\run-qa-tests.bat"],
            ],
            [4.2 * cm, 12.3 * cm],
        )
    )
    story.append(heading("Anexo B. Resultado esperado de ejecucion"))
    story.append(callout("Resumen esperado", "Restore: OK | Build: OK | Pruebas API: OK | Pruebas Selenium: OK | Suite completa: OK"))

    return story


def main():
    OUT_DIR.mkdir(exist_ok=True)
    doc = SimpleDocTemplate(
        str(PDF_PATH),
        pagesize=letter,
        leftMargin=1.6 * cm,
        rightMargin=1.6 * cm,
        topMargin=1.6 * cm,
        bottomMargin=1.5 * cm,
        title="Informe Consolidado QA EFAC",
        author="Codex",
    )
    doc.build(build_story(), onFirstPage=on_page, onLaterPages=on_page)
    print(PDF_PATH)


if __name__ == "__main__":
    main()
