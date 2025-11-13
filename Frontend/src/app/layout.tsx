import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
    title: "Marcos Construção - Gestão de Obras",
    description: "Sistema completo de gestão para construção civil",
};

export default function RootLayout({
    children,
}: Readonly<{
    children: React.ReactNode;
}>) {
    return (
        <html lang="pt-BR">
            <body className="antialiased bg-gray-50">
                {children}
            </body>
        </html>
    );
}
