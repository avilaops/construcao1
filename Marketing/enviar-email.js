// ========================================
// ENVIO EMAIL - APRESENTAÇÃO ÁVILA
// ========================================

require("dotenv").config();
const nodemailer = require("nodemailer");
const fs = require("fs");
const path = require("path");

const transporter = nodemailer.createTransport({
  host: process.env.EMAIL_HOST,
  port: process.env.EMAIL_PORT,
  secure: false,
  auth: {
    user: process.env.EMAIL_USER,
    pass: process.env.EMAIL_PASS,
  },
});

async function enviarEmailApresentacao(emailDestinatario) {
  try {
    console.log("📧 Enviando email de apresentação...");

    const htmlContent = fs.readFileSync(
      path.join(__dirname, "email-apresentacao-avila.html"),
      "utf-8"
    );

    const mailOptions = {
      from: `"Ávila" <${process.env.EMAIL_USER}>`,
      to: emailDestinatario,
      subject: "🏗️ Ávila - Sistema para Construtoras",
      html: htmlContent,
    };

    const info = await transporter.sendMail(mailOptions);

    console.log("✅ Email enviado com sucesso!");
    console.log("📋 Message ID:", info.messageId);
    console.log("📧 Para:", emailDestinatario);
  } catch (error) {
    console.error("❌ Erro ao enviar email:", error.message);
    process.exit(1);
  }
}

// Pegar email da linha de comando
const emailDestino = process.argv[2];

if (!emailDestino) {
  console.error("❌ Uso: node enviar-email.js email@exemplo.com");
  process.exit(1);
}

enviarEmailApresentacao(emailDestino);

