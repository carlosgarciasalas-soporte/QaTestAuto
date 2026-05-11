$ErrorActionPreference = "Stop"

$RootDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $RootDir

$RunId = Get-Date -Format "yyyyMMdd_HHmmss"
$ReportRoot = Join-Path $RootDir "test-assets\evidence\reports"
$RunDir = Join-Path $ReportRoot "$($RunId)_qa-run"
$LogFile = Join-Path $RunDir "qa-execution.log"
$WebApiLog = Join-Path $RunDir "webapi.log"
$WebApiUrl = "http://localhost:5100/"
$WebApiJob = $null
$LogWriter = $null

New-Item -ItemType Directory -Path $RunDir -Force | Out-Null
$LogWriter = [System.IO.StreamWriter]::new($LogFile, $true, [System.Text.UTF8Encoding]::new($false))
$LogWriter.AutoFlush = $true

function Write-QALog {
    param([string]$Message = "")

    Write-Host $Message
    $script:LogWriter.WriteLine($Message)
}

function Invoke-QACommand {
    param(
        [string]$Title,
        [string]$FilePath,
        [string[]]$Arguments
    )

    Write-QALog
    Write-QALog "[INFO] $Title"
    Write-QALog "[CMD] $FilePath $($Arguments -join ' ')"

    & $FilePath @Arguments 2>&1 | ForEach-Object {
        $line = $_.ToString()
        Write-Host $line
        $script:LogWriter.WriteLine($line)
    }

    $exitCode = if ($null -eq $LASTEXITCODE) { 0 } else { $LASTEXITCODE }

    if ($exitCode -ne 0) {
        Write-QALog "[ERROR] $Title fallo con codigo $exitCode"
        throw "Fallo la etapa: $Title"
    }

    Write-QALog "[OK] $Title"
}

function Start-EfacWebApi {
    $existingPort = Get-NetTCPConnection -LocalPort 5100 -State Listen -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($existingPort) {
        throw "El puerto 5100 ya esta en uso. Cierre la aplicacion existente y vuelva a ejecutar."
    }

    Write-QALog "[INFO] Levantando WebAPI en $WebApiUrl"

    $scriptBlock = {
        param($WorkingDirectory, $OutputFile)

        Set-Location $WorkingDirectory
        $env:ASPNETCORE_ENVIRONMENT = "Development"
        dotnet "Efac.WebAPI\bin\Debug\net9.0\Efac.WebAPI.dll" --urls "http://localhost:5100" *> $OutputFile
    }

    $script:WebApiJob = Start-Job -ScriptBlock $scriptBlock -ArgumentList $RootDir, $WebApiLog
    Write-QALog "[INFO] WebAPI Job Id: $($script:WebApiJob.Id)"
}

function Wait-EfacWebApi {
    Write-QALog "[INFO] Esperando disponibilidad de la WebAPI..."

    $deadline = (Get-Date).AddSeconds(45)

    do {
        try {
            $response = Invoke-WebRequest -Uri $WebApiUrl -UseBasicParsing -TimeoutSec 2
            if ($response.StatusCode -eq 200) {
                Write-QALog "[OK] WebAPI disponible"
                return
            }
        }
        catch {
            Start-Sleep -Seconds 1
        }
    } while ((Get-Date) -lt $deadline)

    Write-QALog "[ERROR] La WebAPI no respondio dentro del tiempo esperado."
    if (Test-Path $WebApiLog) {
        Write-QALog "[INFO] Ultimas lineas de webapi.log:"
        Get-Content -LiteralPath $WebApiLog -Tail 80 | ForEach-Object { Write-QALog $_ }
    }

    throw "La WebAPI no quedo disponible."
}

function Stop-EfacWebApi {
    if ($null -eq $script:WebApiJob) {
        return
    }

    Write-QALog "[INFO] Cerrando WebAPI Job Id: $($script:WebApiJob.Id)"
    Stop-Job -Job $script:WebApiJob -ErrorAction SilentlyContinue
    Remove-Job -Job $script:WebApiJob -Force -ErrorAction SilentlyContinue
    $script:WebApiJob = $null
}

try {
    Write-QALog "========================================"
    Write-QALog "EFAC - EJECUCION QA AUTOMATICA"
    Write-QALog "Fecha/Hora: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
    Write-QALog "Directorio: $RootDir"
    Write-QALog "Evidencia: $RunDir"
    Write-QALog "========================================"

    Invoke-QACommand "Restaurando paquetes NuGet" "dotnet" @("restore", "Efac.sln")
    Invoke-QACommand "Compilando solucion" "dotnet" @("build", "Efac.sln", "--no-restore")
    Invoke-QACommand "Ejecutando pruebas API" "dotnet" @(
        "test",
        "Efac.Tests.Api\Efac.Tests.Api.csproj",
        "--no-build",
        "--logger",
        "trx;LogFileName=api-tests.trx",
        "--results-directory",
        $RunDir
    )

    Start-EfacWebApi
    Wait-EfacWebApi

    $env:EFAC_RUN_SELENIUM = "true"
    $env:EFAC_BASE_URL = $WebApiUrl
    Invoke-QACommand "Ejecutando pruebas Selenium" "dotnet" @(
        "test",
        "Efac.Tests.Selenium\Efac.Tests.Selenium.csproj",
        "--no-build",
        "--logger",
        "trx;LogFileName=selenium-tests.trx",
        "--results-directory",
        $RunDir
    )

    Invoke-QACommand "Ejecutando suite completa" "dotnet" @(
        "test",
        "Efac.sln",
        "--no-build",
        "--logger",
        "trx",
        "--results-directory",
        $RunDir
    )

    Stop-EfacWebApi

    Write-QALog "========================================"
    Write-QALog "RESUMEN QA AUTOMATICO"
    Write-QALog "Restore: OK"
    Write-QALog "Build: OK"
    Write-QALog "Pruebas API: OK"
    Write-QALog "Pruebas Selenium: OK"
    Write-QALog "Suite completa: OK"
    Write-QALog "Log: $LogFile"
    Write-QALog "Reportes TRX: $RunDir"
    Write-QALog "========================================"
    exit 0
}
catch {
    Write-QALog "========================================"
    Write-QALog "EJECUCION QA FINALIZADA CON ERROR"
    Write-QALog $_.Exception.Message
    Write-QALog "Revise el log y reportes en: $RunDir"
    Write-QALog "========================================"
    Stop-EfacWebApi
    if ($null -ne $script:LogWriter) {
        $script:LogWriter.Dispose()
    }
    exit 1
}
finally {
    if ($null -ne $script:LogWriter) {
        $script:LogWriter.Dispose()
    }
}
