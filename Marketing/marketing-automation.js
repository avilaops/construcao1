// ========================================
// AUTOMAÇÃO DE MARKETING - MARCOS CONSTRUÇÃO
// Sistema de envio automatizado de emails e WhatsApp
// ========================================

const nodemailer = require("nodemailer");
const fs = require("fs");
const path = require("path");

// ==================
// CONFIGURAÇÃO EMAIL
// ==================

const emailConfig = {
  host: "smtp.gmail.com", // ou smtp.sendgrid.net
  port: 587,
  secure: false,
  auth: {
    user: process.env.EMAIL_USER || "propostas@avila.ops",
    pass: process.env.EMAIL_PASS || "sua-senha-app",
  },
};

const transporter = nodemailer.createTransport(emailConfig);

// ==================
// TEMPLATES EMAIL
// ==================

function loadEmailTemplate(templateName) {
  const templatePath = path.join(__dirname, `email-${templateName}.html`);
  return fs.readFileSync(templatePath, "utf8");
}

function personalizeEmail(template, data) {
  let personalized = template;

  // Substituir variáveis
  const replacements = {
    "{{nome}}": data.nome || "Cliente",
    "{{empresa}}": data.empresa || "sua empresa",
    "{{telefone}}": data.telefone || "",
    "{{cidade}}": data.cidade || "São José do Rio Preto",
    "{{valor_obra}}": data.valorObra
      ? `R$ ${data.valorObra.toLocaleString("pt-BR")}`
      : "R$ 420.000",
    "{{data_expiracao}}": data.dataExpiracao || "20/11/2025",
  };

  Object.keys(replacements).forEach((key) => {
    personalized = personalized.replace(
      new RegExp(key, "g"),
      replacements[key]
    );
  });

  return personalized;
}

// ==================
// FUNÇÕES DE ENVIO
// ==================

async function sendEmail(tipo, destinatario, dadosPersonalizacao = {}) {
  try {
    const template = loadEmailTemplate(tipo);
    const htmlPersonalizado = personalizeEmail(template, dadosPersonalizacao);

    const assuntos = {
      proposta: "🏗️ Sistema Completo para Marcos Construção - Oferta Exclusiva",
      urgencia: "⏰ ÚLTIMA CHANCE - Oferta expira em 48h",
      "boas-vindas": "🎉 Bem-vindo ao Sistema Marcos Construção!",
    };

    const mailOptions = {
      from: {
        name: "Ávila",
        address: emailConfig.auth.user,
      },
      to: destinatario.email,
      subject: assuntos[tipo],
      html: htmlPersonalizado,
      // Tracking
      headers: {
        "X-Campaign": "marcos-construcao",
        "X-Template": tipo,
      },
    };

    const info = await transporter.sendMail(mailOptions);

    console.log(`✅ Email "${tipo}" enviado para ${destinatario.nome}`);
    console.log(`   MessageId: ${info.messageId}`);

    // Log para analytics
    logEnvio({
      tipo: "email",
      template: tipo,
      destinatario: destinatario.email,
      status: "enviado",
      timestamp: new Date(),
    });

    return { success: true, messageId: info.messageId };
  } catch (error) {
    console.error(`❌ Erro ao enviar email para ${destinatario.nome}:`, error);

    logEnvio({
      tipo: "email",
      template: tipo,
      destinatario: destinatario.email,
      status: "erro",
      erro: error.message,
      timestamp: new Date(),
    });

    return { success: false, error: error.message };
  }
}

// ==================
// AUTOMAÇÃO WHATSAPP
// ==================

const whatsappMessages = require("./whatsapp-sequence");

async function sendWhatsApp(tipo, destinatario, dadosPersonalizacao = {}) {
  try {
    // NOTA: Aqui você integraria com Twilio, WhatsApp Business API, etc
    // Este é um exemplo de estrutura

    let mensagem = whatsappMessages.mensagens[tipo];

    // Personalizar mensagem
    mensagem = mensagem
      .replace("{{nome}}", dadosPersonalizacao.nome || destinatario.nome)
      .replace("{{valor_obra}}", dadosPersonalizacao.valorObra || "R$ 420.000")
      .replace(
        "{{cidade}}",
        dadosPersonalizacao.cidade || "São José do Rio Preto"
      );

    // Exemplo com Twilio (descomente para usar)
    /*
        const twilio = require('twilio')(
            process.env.TWILIO_ACCOUNT_SID,
            process.env.TWILIO_AUTH_TOKEN
        );

        await twilio.messages.create({
            body: mensagem,
            from: 'whatsapp:+14155238886', // Seu número Twilio
            to: `whatsapp:${destinatario.telefone}`
        });
        */

    console.log(`✅ WhatsApp "${tipo}" preparado para ${destinatario.nome}`);
    console.log(`   Telefone: ${destinatario.telefone}`);
    console.log(`   Preview: ${mensagem.substring(0, 100)}...`);

    // Log para analytics
    logEnvio({
      tipo: "whatsapp",
      template: tipo,
      destinatario: destinatario.telefone,
      status: "enviado",
      timestamp: new Date(),
    });

    return { success: true, mensagem };
  } catch (error) {
    console.error(
      `❌ Erro ao enviar WhatsApp para ${destinatario.nome}:`,
      error
    );

    logEnvio({
      tipo: "whatsapp",
      template: tipo,
      destinatario: destinatario.telefone,
      status: "erro",
      erro: error.message,
      timestamp: new Date(),
    });

    return { success: false, error: error.message };
  }
}

// ==================
// SEQUÊNCIAS AUTOMATIZADAS
// ==================

async function sequenciaCompleta(lead, config = {}) {
  const {
    enviarEmail = true,
    enviarWhatsApp = true,
    intervaloHoras = 24,
  } = config;

  console.log(`\n🚀 Iniciando sequência para ${lead.nome}...`);

  try {
    // DIA 0: Primeiro contato
    if (enviarEmail) {
      await sendEmail("proposta", lead, lead);
      await delay(5000); // 5 segundos entre envios
    }

    if (enviarWhatsApp) {
      await sendWhatsApp("gancho", lead, lead);
    }

    // Agendar follow-ups
    agendarFollowUp(lead, intervaloHoras);

    console.log(`✅ Sequência iniciada com sucesso!\n`);
  } catch (error) {
    console.error(`❌ Erro na sequência:`, error);
  }
}

function agendarFollowUp(lead, horasAtraso) {
  // Agendar email de urgência
  setTimeout(async () => {
    console.log(`\n⏰ Follow-up automático - ${lead.nome}`);
    await sendEmail("urgencia", lead, lead);
  }, horasAtraso * 60 * 60 * 1000);

  // Agendar WhatsApp de recuperação
  setTimeout(async () => {
    console.log(`\n⏰ Recuperação WhatsApp - ${lead.nome}`);
    await sendWhatsApp("recuperacao", lead, lead);
  }, (horasAtraso + 24) * 60 * 60 * 1000);
}

// ==================
// CAMPANHA EM LOTE
// ==================

async function campanhaEmLote(leads, config = {}) {
  console.log(`\n📊 Iniciando campanha para ${leads.length} leads...\n`);

  const resultados = {
    total: leads.length,
    emailsEnviados: 0,
    whatsappEnviados: 0,
    erros: 0,
  };

  for (const lead of leads) {
    try {
      await sequenciaCompleta(lead, config);
      resultados.emailsEnviados++;
      resultados.whatsappEnviados++;

      // Delay entre leads para evitar spam
      await delay(2000);
    } catch (error) {
      resultados.erros++;
      console.error(`Erro com lead ${lead.nome}:`, error);
    }
  }

  console.log(`\n✅ Campanha concluída!`);
  console.log(`   Total: ${resultados.total}`);
  console.log(`   Emails: ${resultados.emailsEnviados}`);
  console.log(`   WhatsApp: ${resultados.whatsappEnviados}`);
  console.log(`   Erros: ${resultados.erros}\n`);

  return resultados;
}

// ==================
// ANALYTICS E LOGGING
// ==================

function logEnvio(dados) {
  const logFile = path.join(__dirname, "logs", "envios.json");

  try {
    let logs = [];
    if (fs.existsSync(logFile)) {
      logs = JSON.parse(fs.readFileSync(logFile, "utf8"));
    }

    logs.push(dados);

    fs.writeFileSync(logFile, JSON.stringify(logs, null, 2));
  } catch (error) {
    console.error("Erro ao salvar log:", error);
  }
}

function gerarRelatorio() {
  const logFile = path.join(__dirname, "logs", "envios.json");

  if (!fs.existsSync(logFile)) {
    console.log("Nenhum log encontrado.");
    return;
  }

  const logs = JSON.parse(fs.readFileSync(logFile, "utf8"));

  const relatorio = {
    totalEnvios: logs.length,
    emails: logs.filter((l) => l.tipo === "email").length,
    whatsapp: logs.filter((l) => l.tipo === "whatsapp").length,
    sucessos: logs.filter((l) => l.status === "enviado").length,
    erros: logs.filter((l) => l.status === "erro").length,
    porTemplate: {},
  };

  logs.forEach((log) => {
    if (!relatorio.porTemplate[log.template]) {
      relatorio.porTemplate[log.template] = 0;
    }
    relatorio.porTemplate[log.template]++;
  });

  console.log("\n📊 RELATÓRIO DE ENVIOS\n");
  console.log(`Total: ${relatorio.totalEnvios}`);
  console.log(`Emails: ${relatorio.emails}`);
  console.log(`WhatsApp: ${relatorio.whatsapp}`);
  console.log(`Sucessos: ${relatorio.sucessos}`);
  console.log(`Erros: ${relatorio.erros}`);
  console.log("\nPor Template:");
  console.log(relatorio.porTemplate);
  console.log("");

  return relatorio;
}

// ==================
// UTILITÁRIOS
// ==================

function delay(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

function validarLead(lead) {
  const erros = [];

  if (!lead.nome) erros.push("Nome é obrigatório");
  if (!lead.email && !lead.telefone)
    erros.push("Email ou telefone é obrigatório");
  if (lead.email && !validarEmail(lead.email)) erros.push("Email inválido");
  if (lead.telefone && !validarTelefone(lead.telefone))
    erros.push("Telefone inválido");

  return { valido: erros.length === 0, erros };
}

function validarEmail(email) {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
}

function validarTelefone(telefone) {
  // Formato: +55 (17) 99164-2412 ou 17991642412
  return /^(\+55)?[\s-]?\(?[0-9]{2}\)?[\s-]?9?[0-9]{4}[\s-]?[0-9]{4}$/.test(
    telefone
  );
}

// ==================
// EXEMPLO DE USO
// ==================

async function exemploUso() {
  // Lead único
  const lead = {
    nome: "Marcos Silva",
    email: "marcos@construcao.com",
    telefone: "+5517991642412",
    empresa: "Marcos Construção",
    cidade: "São José do Rio Preto",
    valorObra: 420000,
  };

  // Validar
  const validacao = validarLead(lead);
  if (!validacao.valido) {
    console.error("Lead inválido:", validacao.erros);
    return;
  }

  // Enviar sequência
  await sequenciaCompleta(lead, {
    enviarEmail: true,
    enviarWhatsApp: true,
    intervaloHoras: 24,
  });

  // OU campanha em lote
  /*
    const leads = [
        { nome: 'Lead 1', email: 'lead1@email.com', ... },
        { nome: 'Lead 2', email: 'lead2@email.com', ... },
        // ...
    ];

    await campanhaEmLote(leads);
    */

  // Gerar relatório
  setTimeout(() => {
    gerarRelatorio();
  }, 5000);
}

// ==================
// CLI INTERFACE
// ==================

if (require.main === module) {
  const comando = process.argv[2];

  switch (comando) {
    case "teste":
      exemploUso();
      break;

    case "relatorio":
      gerarRelatorio();
      break;

    case "help":
    default:
      console.log(`
🚀 AUTOMAÇÃO DE MARKETING - MARCOS CONSTRUÇÃO

Comandos disponíveis:

  node marketing-automation.js teste      - Testar envio para lead exemplo
  node marketing-automation.js relatorio  - Gerar relatório de envios
  node marketing-automation.js help       - Mostrar esta ajuda

Configuração:

  1. Copie .env.example para .env
  2. Configure EMAIL_USER e EMAIL_PASS
  3. Configure TWILIO_* para WhatsApp (opcional)
  4. Execute: npm install
  5. Teste: node marketing-automation.js teste

Mais informações: README-MARKETING.md
            `);
  }
}

// ==================
// EXPORTS
// ==================

module.exports = {
  sendEmail,
  sendWhatsApp,
  sequenciaCompleta,
  campanhaEmLote,
  gerarRelatorio,
  validarLead,
};
