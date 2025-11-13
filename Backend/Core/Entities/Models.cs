using MarcosConstrutora.Core.Entities;

namespace MarcosConstrutora.Core.Entities;

/// <summary>
/// Representa uma obra/projeto de construção
/// </summary>
public class Obra
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;

    // Cliente
    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    // Localização
    public string Endereco { get; set; } = string.Empty;
    public string Cidade { get; set; } = "São José do Rio Preto";
    public string Estado { get; set; } = "SP";
    public string CEP { get; set; } = string.Empty;

    // Dimensões
    public decimal AreaTotal { get; set; } // m²
    public decimal AreaConstruida { get; set; } // m²
    public int Pavimentos { get; set; }
    public int Quartos { get; set; }
    public int Banheiros { get; set; }

    // Financeiro
    public decimal ValorOrcado { get; set; }
    public decimal ValorRealizado { get; set; }
    public decimal MargemLucro => ValorOrcado > 0
        ? ((ValorOrcado - ValorRealizado) / ValorOrcado) * 100
        : 0;

    // Cronograma
    public DateTime DataInicio { get; set; }
    public DateTime DataPrevisaoTermino { get; set; }
    public DateTime? DataTerminoReal { get; set; }
    public int DiasDecorridos => (DateTime.Now - DataInicio).Days;
    public int DiasRestantes => DataTerminoReal.HasValue
        ? 0
        : (DataPrevisaoTermino - DateTime.Now).Days;

    // Progresso
    public decimal PercentualConcluido { get; set; }
    public StatusObra Status { get; set; } = StatusObra.Planejamento;

    // Relacionamentos
    public List<Medicao> Medicoes { get; set; } = new();
    public List<Despesa> Despesas { get; set; } = new();
    public List<RegistroProgresso> RegistrosProgresso { get; set; } = new();
    public List<Foto> Fotos { get; set; } = new();

    // Auditoria
    public DateTime CriadoEm { get; set; } = DateTime.Now;
    public DateTime? AtualizadoEm { get; set; }
    public string CriadoPor { get; set; } = "Sistema";
}

public enum StatusObra
{
    Planejamento,
    OrcamentoEnviado,
    Aprovado,
    EmAndamento,
    Pausada,
    Concluida,
    Cancelada
}

/// <summary>
/// Cliente da construtora
/// </summary>
public class Cliente
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string WhatsApp { get; set; } = string.Empty;

    // Endereço
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }

    // CRM
    public StatusCliente Status { get; set; } = StatusCliente.Lead;
    public int ProbabilidadeFechamento { get; set; } // 0-100%
    public decimal ValorPotencial { get; set; }

    // Relacionamentos
    public List<Obra> Obras { get; set; } = new();
    public List<Orcamento> Orcamentos { get; set; } = new();
    public List<InteracaoCliente> Interacoes { get; set; } = new();

    // Auditoria
    public DateTime CriadoEm { get; set; } = DateTime.Now;
}

public enum StatusCliente
{
    Lead,
    Qualificado,
    OrcamentoEnviado,
    EmNegociacao,
    Fechado,
    Perdido,
    Inativo
}

/// <summary>
/// Orçamento de obra
/// </summary>
public class Orcamento
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Numero { get; set; } = string.Empty; // ORC-2025-001

    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    // Dados da obra
    public string DescricaoObra { get; set; } = string.Empty;
    public decimal AreaTotal { get; set; }

    // Valores
    public decimal ValorMateriais { get; set; }
    public decimal ValorMaoObra { get; set; }
    public decimal ValorTotal => ValorMateriais + ValorMaoObra;

    // Itens detalhados
    public List<ItemOrcamento> Itens { get; set; } = new();

    // Status
    public StatusOrcamento Status { get; set; } = StatusOrcamento.Rascunho;
    public DateTime? DataEnvio { get; set; }
    public DateTime? DataAprovacao { get; set; }
    public DateTime? DataValidade { get; set; }

    // Prazo
    public int PrazoExecucaoDias { get; set; }

    // Observações
    public string? Observacoes { get; set; }

    // Auditoria
    public DateTime CriadoEm { get; set; } = DateTime.Now;
    public string CriadoPor { get; set; } = "Sistema";
}

public enum StatusOrcamento
{
    Rascunho,
    Enviado,
    EmAnalise,
    Aprovado,
    Rejeitado,
    Expirado
}

public class ItemOrcamento
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrcamentoId { get; set; }

    public string Descricao { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty; // Fundação, Alvenaria, etc
    public decimal Quantidade { get; set; }
    public string Unidade { get; set; } = string.Empty; // m², un, kg
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal => Quantidade * ValorUnitario;
}

/// <summary>
/// Medição de obra para pagamento
/// </summary>
public class Medicao
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Numero { get; set; } = string.Empty; // MED-001

    public Guid ObraId { get; set; }
    public Obra Obra { get; set; } = null!;

    // Valores
    public decimal PercentualMedido { get; set; } // 30%, 50%, etc
    public decimal ValorMedido { get; set; }

    // Status
    public StatusMedicao Status { get; set; } = StatusMedicao.Pendente;
    public DateTime DataMedicao { get; set; } = DateTime.Now;
    public DateTime? DataAprovacao { get; set; }
    public DateTime? DataPagamento { get; set; }

    // Itens medidos
    public List<ItemMedicao> Itens { get; set; } = new();

    // Observações
    public string? Observacoes { get; set; }

    // Fotos comprobatórias
    public List<string> FotosUrls { get; set; } = new();
}

public enum StatusMedicao
{
    Pendente,
    EmAnalise,
    Aprovada,
    Paga,
    Rejeitada
}

public class ItemMedicao
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MedicaoId { get; set; }

    public string Descricao { get; set; } = string.Empty;
    public decimal PercentualExecutado { get; set; }
    public decimal ValorItem { get; set; }
}

/// <summary>
/// Membro da equipe (pedreiro, ajudante, etc)
/// </summary>
public class Funcionario
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;

    // Função
    public TipoFuncionario Funcao { get; set; }
    public decimal SalarioDiaria { get; set; }

    // Status
    public bool Ativo { get; set; } = true;
    public DateTime DataAdmissao { get; set; } = DateTime.Now;
    public DateTime? DataDemissao { get; set; }

    // Produtividade
    public List<RegistroPresenca> Presencas { get; set; } = new();
    public List<RegistroProdutividade> Produtividade { get; set; } = new();
}

public enum TipoFuncionario
{
    Pedreiro,
    Ajudante,
    Mestre,
    Eletricista,
    Encanador,
    Pintor,
    Servente
}

/// <summary>
/// Registro de presença diária
/// </summary>
public class RegistroPresenca
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid FuncionarioId { get; set; }
    public Funcionario Funcionario { get; set; } = null!;

    public Guid ObraId { get; set; }
    public Obra Obra { get; set; } = null!;

    public DateTime Data { get; set; } = DateTime.Today;
    public TimeSpan HoraEntrada { get; set; }
    public TimeSpan? HoraSaida { get; set; }
    public decimal HorasTrabalhadas => HoraSaida.HasValue
        ? (decimal)(HoraSaida.Value - HoraEntrada).TotalHours
        : 0;

    public bool Presente { get; set; } = true;
}

/// <summary>
/// Registro de produtividade (ex: m² de alvenaria)
/// </summary>
public class RegistroProdutividade
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid FuncionarioId { get; set; }
    public Guid ObraId { get; set; }

    public DateTime Data { get; set; } = DateTime.Today;
    public string Atividade { get; set; } = string.Empty; // "Alvenaria", "Reboco"
    public decimal Quantidade { get; set; }
    public string Unidade { get; set; } = string.Empty; // "m²", "m"
}

/// <summary>
/// Material de construção
/// </summary>
public class Material
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty; // Cimento, Areia, etc
    public string Unidade { get; set; } = string.Empty; // kg, m³, un

    // Estoque
    public decimal EstoqueAtual { get; set; }
    public decimal EstoqueMinimo { get; set; }
    public bool EstoqueBaixo => EstoqueAtual <= EstoqueMinimo;

    // Preço médio
    public decimal PrecoUnitario { get; set; }

    // Relacionamentos
    public List<MovimentoEstoque> Movimentos { get; set; } = new();
}

/// <summary>
/// Movimento de estoque (entrada/saída)
/// </summary>
public class MovimentoEstoque
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MaterialId { get; set; }
    public Material Material { get; set; } = null!;

    public Guid? ObraId { get; set; }
    public Obra? Obra { get; set; }

    public TipoMovimento Tipo { get; set; }
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal => Quantidade * ValorUnitario;

    public DateTime Data { get; set; } = DateTime.Now;
    public string? Observacao { get; set; }
}

public enum TipoMovimento
{
    Entrada,
    Saida,
    Ajuste
}

/// <summary>
/// Despesa da obra
/// </summary>
public class Despesa
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ObraId { get; set; }
    public Obra Obra { get; set; } = null!;

    public string Descricao { get; set; } = string.Empty;
    public CategoriaDespesa Categoria { get; set; }
    public decimal Valor { get; set; }

    public DateTime Data { get; set; } = DateTime.Now;
    public bool Paga { get; set; } = false;
    public DateTime? DataPagamento { get; set; }

    public string? NotaFiscal { get; set; }
    public string? Fornecedor { get; set; }
}

public enum CategoriaDespesa
{
    Material,
    MaoDeObra,
    Equipamento,
    Transporte,
    Alimentacao,
    Administrativa,
    Outra
}

/// <summary>
/// Registro de progresso da obra
/// </summary>
public class RegistroProgresso
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ObraId { get; set; }
    public Obra Obra { get; set; } = null!;

    public DateTime Data { get; set; } = DateTime.Now;
    public EtapaObra Etapa { get; set; }
    public decimal PercentualEtapa { get; set; }
    public string Descricao { get; set; } = string.Empty;

    public string RegistradoPor { get; set; } = string.Empty;
}

public enum EtapaObra
{
    Fundacao,
    Alvenaria,
    Cobertura,
    InstalacoesEletricas,
    InstalacoesHidraulicas,
    Reboco,
    Piso,
    Pintura,
    Acabamento,
    Limpeza
}

/// <summary>
/// Foto da obra
/// </summary>
public class Foto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ObraId { get; set; }
    public Obra Obra { get; set; } = null!;

    public string Url { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public EtapaObra? Etapa { get; set; }

    public DateTime DataFoto { get; set; } = DateTime.Now;
    public string TiradaPor { get; set; } = string.Empty;
}

/// <summary>
/// Interação com cliente (WhatsApp, email, visita)
/// </summary>
public class InteracaoCliente
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public TipoInteracao Tipo { get; set; }
    public string Assunto { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;

    public DateTime Data { get; set; } = DateTime.Now;
    public string ResponsavelAvila { get; set; } = string.Empty;

    // Para WhatsApp
    public string? MensagemEnviada { get; set; }
    public string? MensagemRecebida { get; set; }
}

public enum TipoInteracao
{
    WhatsApp,
    Telefone,
    Email,
    VisitaTecnica,
    Reuniao,
    Outra
}
