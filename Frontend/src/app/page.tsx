'use client';

import { useEffect, useState } from 'react';
import axios from 'axios';
import { BarChart3, Home, Users, DollarSign, AlertTriangle, TrendingUp } from 'lucide-react';

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:7001/api';

interface Obra {
    id: string;
    nome: string;
    percentualConcluido: number;
    diasRestantes: number;
    valorOrcado: number;
    gasto: number;
    status: string;
    cliente: string;
}

interface Dashboard {
    obrasAtivas: number;
    totalOrcado: number;
    totalGasto: number;
    margemMedia: number;
    obras: Obra[];
}

export default function DashboardPage() {
    const [dashboard, setDashboard] = useState<Dashboard | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        fetchDashboard();
    }, []);

    const fetchDashboard = async () => {
        try {
            const response = await axios.get(`${API_URL}/obras/dashboard`);
            setDashboard(response.data);
        } catch (error) {
            console.error('Erro ao buscar dashboard:', error);
        } finally {
            setLoading(false);
        }
    };

    if (loading) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <div className="text-center">
                    <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-orange-600 mx-auto"></div>
                    <p className="mt-4 text-gray-600">Carregando...</p>
                </div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gray-50">
            {/* Header */}
            <header className="bg-white shadow-sm border-b">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
                    <div className="flex items-center justify-between">
                        <div className="flex items-center space-x-3">
                            <div className="bg-orange-600 p-2 rounded-lg">
                                <Home className="h-6 w-6 text-white" />
                            </div>
                            <div>
                                <h1 className="text-2xl font-bold text-gray-900">Marcos Construção</h1>
                                <p className="text-sm text-gray-500">Sistema de Gestão de Obras</p>
                            </div>
                        </div>
                        <div className="text-right">
                            <p className="text-sm text-gray-500">Bem-vindo,</p>
                            <p className="font-semibold text-gray-900">Marcos</p>
                        </div>
                    </div>
                </div>
            </header>

            {/* Main Content */}
            <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
                {/* KPIs */}
                <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
                    <div className="bg-white rounded-lg shadow p-6">
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-sm font-medium text-gray-600">Obras Ativas</p>
                                <p className="text-3xl font-bold text-gray-900 mt-2">
                                    {dashboard?.obrasAtivas || 0}
                                </p>
                            </div>
                            <div className="bg-blue-100 p-3 rounded-full">
                                <Home className="h-6 w-6 text-blue-600" />
                            </div>
                        </div>
                    </div>

                    <div className="bg-white rounded-lg shadow p-6">
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-sm font-medium text-gray-600">Faturamento</p>
                                <p className="text-3xl font-bold text-gray-900 mt-2">
                                    {new Intl.NumberFormat('pt-BR', {
                                        style: 'currency',
                                        currency: 'BRL',
                                        minimumFractionDigits: 0,
                                    }).format(dashboard?.totalOrcado || 0)}
                                </p>
                            </div>
                            <div className="bg-green-100 p-3 rounded-full">
                                <DollarSign className="h-6 w-6 text-green-600" />
                            </div>
                        </div>
                    </div>

                    <div className="bg-white rounded-lg shadow p-6">
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-sm font-medium text-gray-600">Gastos</p>
                                <p className="text-3xl font-bold text-gray-900 mt-2">
                                    {new Intl.NumberFormat('pt-BR', {
                                        style: 'currency',
                                        currency: 'BRL',
                                        minimumFractionDigits: 0,
                                    }).format(dashboard?.totalGasto || 0)}
                                </p>
                            </div>
                            <div className="bg-red-100 p-3 rounded-full">
                                <TrendingUp className="h-6 w-6 text-red-600" />
                            </div>
                        </div>
                    </div>

                    <div className="bg-white rounded-lg shadow p-6">
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-sm font-medium text-gray-600">Margem Média</p>
                                <p className="text-3xl font-bold text-gray-900 mt-2">
                                    {dashboard?.margemMedia.toFixed(1)}%
                                </p>
                            </div>
                            <div className="bg-purple-100 p-3 rounded-full">
                                <BarChart3 className="h-6 w-6 text-purple-600" />
                            </div>
                        </div>
                    </div>
                </div>

                {/* Obras */}
                <div className="bg-white rounded-lg shadow">
                    <div className="px-6 py-4 border-b border-gray-200">
                        <h2 className="text-lg font-semibold text-gray-900">Obras em Andamento</h2>
                    </div>
                    <div className="p-6">
                        {dashboard?.obras && dashboard.obras.length > 0 ? (
                            <div className="space-y-6">
                                {dashboard.obras.map((obra) => (
                                    <div key={obra.id} className="border rounded-lg p-6 hover:shadow-md transition-shadow">
                                        <div className="flex items-start justify-between mb-4">
                                            <div>
                                                <h3 className="text-lg font-semibold text-gray-900">{obra.nome}</h3>
                                                <p className="text-sm text-gray-600">Cliente: {obra.cliente}</p>
                                            </div>
                                            <span className="px-3 py-1 bg-green-100 text-green-800 text-sm font-medium rounded-full">
                                                {obra.status}
                                            </span>
                                        </div>

                                        {/* Progress Bar */}
                                        <div className="mb-4">
                                            <div className="flex items-center justify-between mb-2">
                                                <span className="text-sm font-medium text-gray-700">Progresso</span>
                                                <span className="text-sm font-semibold text-gray-900">
                                                    {obra.percentualConcluido.toFixed(0)}%
                                                </span>
                                            </div>
                                            <div className="w-full bg-gray-200 rounded-full h-3">
                                                <div
                                                    className="bg-orange-600 h-3 rounded-full transition-all"
                                                    style={{ width: `${obra.percentualConcluido}%` }}
                                                />
                                            </div>
                                        </div>

                                        {/* Metrics */}
                                        <div className="grid grid-cols-3 gap-4">
                                            <div>
                                                <p className="text-xs text-gray-600">Orçado</p>
                                                <p className="text-sm font-semibold text-gray-900">
                                                    {new Intl.NumberFormat('pt-BR', {
                                                        style: 'currency',
                                                        currency: 'BRL',
                                                        minimumFractionDigits: 0,
                                                    }).format(obra.valorOrcado)}
                                                </p>
                                            </div>
                                            <div>
                                                <p className="text-xs text-gray-600">Gasto</p>
                                                <p className="text-sm font-semibold text-gray-900">
                                                    {new Intl.NumberFormat('pt-BR', {
                                                        style: 'currency',
                                                        currency: 'BRL',
                                                        minimumFractionDigits: 0,
                                                    }).format(obra.gasto)}
                                                </p>
                                            </div>
                                            <div>
                                                <p className="text-xs text-gray-600">Prazo</p>
                                                <p className="text-sm font-semibold text-gray-900">
                                                    {obra.diasRestantes} dias
                                                </p>
                                            </div>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        ) : (
                            <div className="text-center py-12">
                                <Home className="h-12 w-12 text-gray-400 mx-auto mb-4" />
                                <p className="text-gray-600">Nenhuma obra ativa no momento</p>
                            </div>
                        )}
                    </div>
                </div>
            </main>
        </div>
    );
}
