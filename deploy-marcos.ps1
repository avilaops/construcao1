# 🚀 Deploy Automático - Marcos Construção
# Execute este script para fazer o deploy completo do sistema

param(
    [switch]$SkipBackend,
    [switch]$SkipFrontend,
    [switch]$ProductionMode
)

Write-Host "🏗️  MARCOS CONSTRUÇÃO - DEPLOY AUTOMÁTICO" -ForegroundColor Cyan
Write-Host "================================================`n" -ForegroundColor Cyan

$ErrorActionPreference = "Stop"
$rootPath = $PSScriptRoot

# ===== VERIFICAÇÕES INICIAIS =====
Write-Host "📋 Verificando pré-requisitos..." -ForegroundColor Yellow

# Verificar .NET
try {
    $dotnetVersion = dotnet --version
    Write-Host "✅ .NET SDK: $dotnetVersion" -ForegroundColor Green
}
catch {
    Write-Host "❌ .NET SDK não encontrado! Instale em: https://dotnet.microsoft.com/download" -ForegroundColor Red
    exit 1
}

# Verificar Node.js
try {
    $nodeVersion = node --version
    Write-Host "✅ Node.js: $nodeVersion" -ForegroundColor Green
}
catch {
    Write-Host "❌ Node.js não encontrado! Instale em: https://nodejs.org" -ForegroundColor Red
    exit 1
}

Write-Host ""

# ===== BACKEND (API) =====
if (-not $SkipBackend) {
    Write-Host "🔧 BACKEND - Configurando API..." -ForegroundColor Cyan
    Write-Host "--------------------------------`n" -ForegroundColor Cyan

    $backendPath = Join-Path $rootPath "Backend\API"

    if (Test-Path $backendPath) {
        Push-Location $backendPath

        try {
            Write-Host "📦 Restaurando pacotes NuGet..." -ForegroundColor Yellow
            dotnet restore

            Write-Host "🏗️  Compilando projeto..." -ForegroundColor Yellow
            dotnet build --configuration Release

            Write-Host "🗄️  Aplicando migrações do banco de dados..." -ForegroundColor Yellow
            dotnet ef database update --no-build

            if ($ProductionMode) {
                Write-Host "📦 Publicando para produção..." -ForegroundColor Yellow
                dotnet publish --configuration Release --output "$rootPath\Deploy\Backend"
                Write-Host "✅ Backend publicado em: $rootPath\Deploy\Backend" -ForegroundColor Green
            }
            else {
                Write-Host "🚀 Iniciando API em modo desenvolvimento..." -ForegroundColor Yellow
                Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$backendPath'; dotnet run"
                Write-Host "✅ API iniciada em: https://localhost:7001" -ForegroundColor Green
            }
        }
        catch {
            Write-Host "❌ Erro no backend: $_" -ForegroundColor Red
            Pop-Location
            exit 1
        }

        Pop-Location
    }
    else {
        Write-Host "⚠️  Pasta do backend não encontrada: $backendPath" -ForegroundColor Yellow
    }

    Write-Host ""
}

# ===== FRONTEND (Next.js) =====
if (-not $SkipFrontend) {
    Write-Host "🎨 FRONTEND - Configurando Dashboard..." -ForegroundColor Cyan
    Write-Host "--------------------------------------`n" -ForegroundColor Cyan

    $frontendPath = Join-Path $rootPath "Frontend"

    if (Test-Path $frontendPath) {
        Push-Location $frontendPath

        try {
            # Criar arquivo de ambiente se não existir
            $envFile = ".env.local"
            if (-not (Test-Path $envFile)) {
                Write-Host "📝 Criando arquivo de configuração..." -ForegroundColor Yellow
                @"
NEXT_PUBLIC_API_URL=https://localhost:7001/api
NEXT_PUBLIC_APP_NAME=Marcos Construção
"@ | Out-File -FilePath $envFile -Encoding utf8
            }

            Write-Host "📦 Instalando dependências..." -ForegroundColor Yellow
            npm install

            if ($ProductionMode) {
                Write-Host "🏗️  Compilando para produção..." -ForegroundColor Yellow
                npm run build
                Write-Host "✅ Build concluído!" -ForegroundColor Green

                Write-Host "`n📤 Próximos passos para deploy:" -ForegroundColor Cyan
                Write-Host "   1. Vercel: npm i -g vercel && vercel" -ForegroundColor White
                Write-Host "   2. Azure: Usar extensão do VS Code" -ForegroundColor White
                Write-Host "   3. Netlify: netlify deploy --prod" -ForegroundColor White
            }
            else {
                Write-Host "🚀 Iniciando servidor de desenvolvimento..." -ForegroundColor Yellow
                Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$frontendPath'; npm run dev"
                Write-Host "✅ Frontend iniciado em: http://localhost:3000" -ForegroundColor Green
            }
        }
        catch {
            Write-Host "❌ Erro no frontend: $_" -ForegroundColor Red
            Pop-Location
            exit 1
        }

        Pop-Location
    }
    else {
        Write-Host "⚠️  Pasta do frontend não encontrada: $frontendPath" -ForegroundColor Yellow
    }

    Write-Host ""
}

# ===== RESUMO FINAL =====
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "✅ DEPLOY CONCLUÍDO COM SUCESSO!" -ForegroundColor Green
Write-Host "================================================`n" -ForegroundColor Cyan

if (-not $ProductionMode) {
    Write-Host "🌐 URLs do Sistema:" -ForegroundColor Cyan
    Write-Host "   Backend API: https://localhost:7001/swagger" -ForegroundColor White
    Write-Host "   Frontend:    http://localhost:3000`n" -ForegroundColor White

    Write-Host "📚 Próximos Passos:" -ForegroundColor Cyan
    Write-Host "   1. Abra https://localhost:7001/swagger para testar a API" -ForegroundColor White
    Write-Host "   2. Abra http://localhost:3000 para acessar o dashboard" -ForegroundColor White
    Write-Host "   3. Faça login com: marcos@construcao.com`n" -ForegroundColor White

    Write-Host "🔧 Comandos Úteis:" -ForegroundColor Cyan
    Write-Host "   Deploy produção:  .\deploy-marcos.ps1 -ProductionMode" -ForegroundColor White
    Write-Host "   Apenas backend:   .\deploy-marcos.ps1 -SkipFrontend" -ForegroundColor White
    Write-Host "   Apenas frontend:  .\deploy-marcos.ps1 -SkipBackend`n" -ForegroundColor White
}
else {
    Write-Host "📦 Arquivos de produção gerados:" -ForegroundColor Cyan
    Write-Host "   Backend:  $rootPath\Deploy\Backend" -ForegroundColor White
    Write-Host "   Frontend: $frontendPath\.next`n" -ForegroundColor White
}

Write-Host "📞 Suporte: suporte@avila.ops" -ForegroundColor Cyan
Write-Host "🌐 Documentação: README.md`n" -ForegroundColor Cyan

Write-Host "Pressione qualquer tecla para fechar..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
