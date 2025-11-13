# ⚡ GUIA RÁPIDO - MARCOS CONSTRUÇÃO

## 🚀 DEPLOY EM 3 COMANDOS

```powershell
# 1. Navegue até a pasta
cd "c:\Users\nicol\OneDrive\Avila\Avilaops\Products\MarcosConstrutora"

# 2. Execute o deploy automático
.\deploy-marcos.ps1

# 3. Pronto! Sistema no ar 🎉
```

**URLs:**
- 🔌 API: https://localhost:7001/swagger
- 🎨 Dashboard: http://localhost:3000

---

## 📋 O QUE ENTREGAREMOS HOJE

### ✅ Sistema Completo
- ✅ Backend API REST (ASP.NET Core 9)
- ✅ Frontend Dashboard (Next.js 15)
- ✅ Banco de Dados (SQL Server LocalDB)
- ✅ Deploy Automático (1 comando)

### ✅ Funcionalidades Implementadas

1. **📊 Dashboard Executivo**
   - Visão geral de todas as obras
   - KPIs em tempo real
   - Gráficos de progresso

2. **🏗️ Gestão de Obras**
   - Cadastro de obras
   - Controle de progresso
   - Upload de fotos
   - Cronograma

3. **💰 Gestão Financeira**
   - Orçamentos
   - Medições
   - Controle de despesas
   - Margem de lucro

4. **👷 Gestão de Equipe**
   - Cadastro de funcionários
   - Controle de presença
   - Produtividade

5. **📦 Controle de Materiais**
   - Estoque
   - Alertas de reposição
   - Histórico de preços

6. **👤 CRM de Clientes**
   - Gestão de leads
   - Histórico de interações
   - Funil de vendas

---

## 🎯 PRÓXIMOS PASSOS (Enquanto Almoçam)

### Passo 1: Configurar Obra Atual (5 min)
```
1. Acesse http://localhost:3000
2. Vá em "Obras" > "Nova Obra"
3. Preencha:
   - Nome: Casa 350m² - João Silva
   - Área: 350m²
   - Valor: R$ 420.000
   - Prazo: 6 meses
4. Salvar
```

### Passo 2: Cadastrar Equipe (3 min)
```
1. Vá em "Equipe" > "Novo Funcionário"
2. Adicione os pedreiros e ajudantes
3. Configure diárias
```

### Passo 3: Configurar Materiais (2 min)
```
1. Vá em "Materiais"
2. Sistema já vem com cimento e areia
3. Adicione outros conforme necessário
```

---

## 📱 FEATURES PREMIUM (Já Incluídas)

### WhatsApp Business Integration
```typescript
// Exemplo de uso
POST /api/whatsapp/send
{
  "telefone": "17991642412",
  "mensagem": "Olá! Seu orçamento está pronto."
}
```

### Gerador de Orçamentos PDF
```csharp
// Já implementado
POST /api/orcamentos
// Gera PDF automaticamente
```

### Dashboard Analytics
- Progresso em tempo real
- Alertas automáticos
- Previsões de conclusão

---

## 🔧 CONFIGURAÇÕES ADICIONAIS

### WhatsApp (Opcional - para automação)
1. Criar conta Twilio: https://twilio.com
2. Obter Account SID e Auth Token
3. Atualizar `appsettings.json`:
```json
{
  "WhatsApp": {
    "AccountSid": "SEU_SID",
    "AuthToken": "SEU_TOKEN",
    "PhoneNumber": "+5517991642412"
  }
}
```

### Banco de Dados Externo (Opcional)
Para usar SQL Server completo ao invés do LocalDB:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Database=MarcosConstrutora;User Id=sa;Password=SUA_SENHA;"
  }
}
```

---

## 📊 MÉTRICAS QUE O SISTEMA RASTREIA

### Por Obra
- ✅ Percentual concluído
- ✅ Dias restantes vs planejado
- ✅ Orçado vs realizado
- ✅ Margem de lucro
- ✅ Progresso por etapa

### Por Equipe
- ✅ Horas trabalhadas
- ✅ Produtividade (m²/dia)
- ✅ Custo de mão de obra
- ✅ Taxa de presença

### Financeiro
- ✅ Faturamento total
- ✅ Gastos por categoria
- ✅ Fluxo de caixa
- ✅ Medições pendentes

---

## 🎨 PERSONALIZAÇÃO

### Logo e Cores
Edite `Frontend/src/app/layout.tsx`:
```typescript
// Trocar "Marcos Construção" pelo nome desejado
title: "SUA EMPRESA - Gestão de Obras"
```

### Relatórios Customizados
Adicione em `Backend/API/Controllers/RelatoriosController.cs`

---

## 📞 SUPORTE

**Durante almoço (próximas 2h):**
- WhatsApp: [SEU NÚMERO]
- Email: suporte@avila.ops

**Pós-almoço:**
- Base de conhecimento: README.md completo
- Vídeos tutoriais: A criar

---

## 🎁 BÔNUS SURPRESA

### Aplicativo Mobile (PWA)
O dashboard funciona como app no celular:
1. Abra http://localhost:3000 no celular
2. Chrome: Menu > "Adicionar à tela inicial"
3. Pronto! App instalado 📱

### Modo Offline
- Funciona sem internet
- Sincroniza quando conectar
- Fotos salvas localmente

### Notificações Push
- Alertas de material acabando
- Medições pendentes
- Novos leads

---

## ⚡ COMANDOS ÚTEIS

```powershell
# Apenas backend
.\deploy-marcos.ps1 -SkipFrontend

# Apenas frontend
.\deploy-marcos.ps1 -SkipBackend

# Deploy produção
.\deploy-marcos.ps1 -ProductionMode

# Resetar banco de dados
cd Backend\API
dotnet ef database drop
dotnet ef database update
```

---

## 🎯 CHECKLIST PRONTIDÃO

- [x] Backend API funcionando
- [x] Frontend Dashboard funcionando
- [x] Banco de dados configurado
- [x] Obra inicial cadastrada
- [x] Dashboard com dados reais
- [x] Deploy automatizado
- [ ] WhatsApp configurado (opcional)
- [ ] Logo personalizado (opcional)

---

## 💡 DICAS PROFISSIONAIS

### Para Marcos
1. Use o dashboard diariamente às 8h e 18h
2. Configure alertas de materiais
3. Acompanhe margem de lucro semanalmente

### Para Equipe
1. Check-in diário pelo app
2. Fotos de progresso toda sexta
3. Solicitação de materiais pelo sistema

### Para Crescimento
1. Use métricas para precificar melhor
2. Compare produtividade entre obras
3. Identifique gargalos no processo

---

**🏗️ Sistema pronto para uso profissional!**
**⚡ Deploy < 15 minutos**
**💰 ROI: Imediato**

*Powered by Ávila Framework 🔷*
