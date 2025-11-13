# 🚀 DEPLOY COMPLETO - MARCOS CONSTRUÇÃO
# Git + Email Marketing + Hospedagem

param(
    [switch]$InitGit,
    [switch]$SendTestEmail,
    [switch]$DeployFull
)

Write-Host "🏗️  MARCOS CONSTRUÇÃO - DEPLOY COMPLETO" -ForegroundColor Cyan
Write-Host "=============================================`n" -ForegroundColor Cyan

$ErrorActionPreference = "Stop"
$rootPath = $PSScriptRoot

# ===== CONFIGURAÇÃO GIT =====
if ($InitGit -or $DeployFull) {
    Write-Host "📦 GIT - Inicializando repositório..." -ForegroundColor Yellow

    Push-Location $rootPath

    try {
        # Verificar se já é um repo git
        if (-not (Test-Path ".git")) {
            Write-Host "   Inicializando git..." -ForegroundColor Gray
            git init
        }

        # Criar .gitignore
        @"
# Dependencies
node_modules/
.venv/
packages/

# Environment
.env
.env.local
*.local

# Build outputs
bin/
obj/
dist/
.next/
out/

# Logs
logs/
*.log

# IDEs
.vs/
.vscode/
.idea/

# OS
.DS_Store
Thumbs.db
desktop.ini

# Temp
temp/
tmp/
*.tmp
"@ | Out-File -FilePath ".gitignore" -Encoding utf8

        # Add remote
        $remoteExists = git remote | Select-String "origin"
        if (-not $remoteExists) {
            Write-Host "   Adicionando remote origin..." -ForegroundColor Gray
            git remote add origin https://github.com/avilaops/construcao1.git
        }

        # Add files
        Write-Host "   Adicionando arquivos..." -ForegroundColor Gray
        git add .

        # Commit
        $commitMsg = "feat: Sistema completo Marcos Construção

- Backend API (ASP.NET Core 9)
- Frontend Dashboard (Next.js 15)
- Marketing (Email + WhatsApp)
- Deploy automatizado
- Documentação completa

Powered by Ávila Framework"

        git commit -m $commitMsg

        # Push
        Write-Host "   Enviando para GitHub..." -ForegroundColor Gray
        git branch -M main
        git push -u origin main

        Write-Host "✅ Repositório Git configurado!" -ForegroundColor Green
        Write-Host "   URL: https://github.com/avilaops/construcao1" -ForegroundColor Gray

    }
    catch {
        Write-Host "❌ Erro no Git: $_" -ForegroundColor Red
    }

    Pop-Location
    Write-Host ""
}

# ===== CONFIGURAÇÃO EMAIL MARKETING =====
if ($SendTestEmail -or $DeployFull) {
    Write-Host "📧 EMAIL - Configurando Porkbun..." -ForegroundColor Yellow

    $marketingPath = Join-Path $rootPath "Marketing"
    Push-Location $marketingPath

    try {
        # Verificar se Node.js está instalado
        $nodeVersion = node --version
        Write-Host "   Node.js: $nodeVersion" -ForegroundColor Gray

        # Instalar dependências se necessário
        if (-not (Test-Path "node_modules")) {
            Write-Host "   Instalando dependências..." -ForegroundColor Gray
            npm install
        }

        # Criar script de teste
        $testScript = @"
require('dotenv').config({ path: '../.env' });
const nodemailer = require('nodemailer');
const fs = require('fs');

async function sendTestEmail() {
    const transporter = nodemailer.createTransport({
        host: process.env.EMAIL_HOST || 'smtp.porkbun.com',
        port: parseInt(process.env.EMAIL_PORT) || 587,
        secure: false,
        auth: {
            user: process.env.EMAIL_USER || 'contato@httconstrucao1.avila.inc',
            pass: process.env.EMAIL_PASS
        }
    });

    const html = fs.readFileSync('email-proposta.html', 'utf8');

    const info = await transporter.sendMail({
        from: {
            name: process.env.EMAIL_FROM_NAME || 'Marcos Construção',
            address: process.env.EMAIL_USER || 'contato@httconstrucao1.avila.inc'
        },
        to: process.env.EMAIL_USER, // Envia para você mesmo como teste
        subject: '🏗️ [TESTE] Sistema Completo Marcos Construção',
        html: html
    });

    console.log('✅ Email de teste enviado!');
    console.log('   MessageId:', info.messageId);
    console.log('   Preview:', nodemailer.getTestMessageUrl(info));
}

sendTestEmail().catch(console.error);
"@

        $testScript | Out-File -FilePath "send-test.js" -Encoding utf8

        # Executar teste
        Write-Host "   Enviando email de teste..." -ForegroundColor Gray
        node send-test.js

        Write-Host "✅ Email configurado com Porkbun!" -ForegroundColor Green
        Write-Host "   Domínio: httconstrucao1.avila.inc" -ForegroundColor Gray

    }
    catch {
        Write-Host "⚠️  Erro no email (configure .env): $_" -ForegroundColor Yellow
    }

    Pop-Location
    Write-Host ""
}

# ===== DEPLOY COMPLETO =====
if ($DeployFull) {
    Write-Host "🚀 DEPLOY - Preparando produção..." -ForegroundColor Yellow

    # Backend
    $backendPath = Join-Path $rootPath "Backend\API"
    if (Test-Path $backendPath) {
        Push-Location $backendPath
        Write-Host "   Compilando backend..." -ForegroundColor Gray
        dotnet publish -c Release -o "$rootPath\Deploy\Backend"
        Pop-Location
    }

    # Frontend
    $frontendPath = Join-Path $rootPath "Frontend"
    if (Test-Path $frontendPath) {
        Push-Location $frontendPath
        Write-Host "   Compilando frontend..." -ForegroundColor Gray
        npm install
        npm run build
        Pop-Location
    }

    Write-Host "✅ Deploy compilado!" -ForegroundColor Green
    Write-Host "   Backend: $rootPath\Deploy\Backend" -ForegroundColor Gray
    Write-Host "   Frontend: $frontendPath\.next" -ForegroundColor Gray
    Write-Host ""
}

# ===== RESUMO FINAL =====
Write-Host "==============================================" -ForegroundColor Cyan
Write-Host "✅ CONFIGURAÇÃO CONCLUÍDA!" -ForegroundColor Green
Write-Host "==============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "🌐 URLs do Projeto:" -ForegroundColor Cyan
Write-Host "   Website: https://httconstrucao1.avila.inc" -ForegroundColor White
Write-Host "   GitHub: https://github.com/avilaops/construcao1" -ForegroundColor White
Write-Host "   Email: contato@httconstrucao1.avila.inc" -ForegroundColor White
Write-Host ""
Write-Host "📝 Próximos Passos:" -ForegroundColor Cyan
Write-Host "   1. Configure senha do email em .env (EMAIL_PASS)" -ForegroundColor White
Write-Host "   2. Execute: .\deploy-completo.ps1 -SendTestEmail" -ForegroundColor White
Write-Host "   3. Envie proposta: node Marketing/marketing-automation.js teste" -ForegroundColor White
Write-Host ""
Write-Host "🎯 Comandos Úteis:" -ForegroundColor Cyan
Write-Host "   Git + Email:  .\deploy-completo.ps1 -InitGit -SendTestEmail" -ForegroundColor White
Write-Host "   Deploy full:  .\deploy-completo.ps1 -DeployFull" -ForegroundColor White
Write-Host "   Apenas Git:   .\deploy-completo.ps1 -InitGit" -ForegroundColor White
Write-Host ""

if (-not $InitGit -and -not $SendTestEmail -and -not $DeployFull) {
    Write-Host "💡 Execute com -InitGit, -SendTestEmail ou -DeployFull" -ForegroundColor Yellow
    Write-Host "   Exemplo: .\deploy-completo.ps1 -InitGit -SendTestEmail" -ForegroundColor Gray
}
