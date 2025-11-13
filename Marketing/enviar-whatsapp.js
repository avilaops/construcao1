// ========================================
// ENVIO WHATSAPP - MARCOS CONSTRUÇÃO
// ========================================

require("dotenv").config();
const twilio = require("twilio");

const client = twilio(
  process.env.TWILIO_ACCOUNT_SID,
  process.env.TWILIO_AUTH_TOKEN
);

const mensagem = `🏗️ Oi Marcos!

Parabéns pela casa de 350m²! 👏

Somos da *Ávila* e desenvolvemos um sistema completo de gestão de obras.

Funciona assim:
• Dashboard em tempo real
• Controle financeiro automático
• App mobile para equipe
• Tudo integrado

Posso te enviar por email uma apresentação rápida?

É só me passar seu melhor email 📧`;

async function enviarWhatsApp() {
  try {
    console.log("📱 Enviando WhatsApp para Marcos...");

    const message = await client.messages.create({
      body: mensagem,
      from: process.env.TWILIO_PHONE_NUMBER,
      to: "+5517999999999", // COLOCAR NÚMERO DO MARCOS AQUI
    });

    console.log("✅ WhatsApp enviado com sucesso!");
    console.log("📋 SID:", message.sid);
    console.log("📊 Status:", message.status);
  } catch (error) {
    console.error("❌ Erro ao enviar WhatsApp:", error.message);
    process.exit(1);
  }
}

enviarWhatsApp();
