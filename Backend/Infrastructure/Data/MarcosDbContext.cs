using Microsoft.EntityFrameworkCore;
using MarcosConstrutora.Core.Entities;

namespace MarcosConstrutora.Infrastructure.Data;

public class MarcosDbContext : DbContext
{
    public MarcosDbContext(DbContextOptions<MarcosDbContext> options) : base(options) { }

    // DbSets
    public DbSet<Obra> Obras => Set<Obra>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Orcamento> Orcamentos => Set<Orcamento>();
    public DbSet<ItemOrcamento> ItensOrcamento => Set<ItemOrcamento>();
    public DbSet<Medicao> Medicoes => Set<Medicao>();
    public DbSet<ItemMedicao> ItensMedicao => Set<ItemMedicao>();
    public DbSet<Funcionario> Funcionarios => Set<Funcionario>();
    public DbSet<RegistroPresenca> RegistrosPresenca => Set<RegistroPresenca>();
    public DbSet<RegistroProdutividade> RegistrosProdutividade => Set<RegistroProdutividade>();
    public DbSet<Material> Materiais => Set<Material>();
    public DbSet<MovimentoEstoque> MovimentosEstoque => Set<MovimentoEstoque>();
    public DbSet<Despesa> Despesas => Set<Despesa>();
    public DbSet<RegistroProgresso> RegistrosProgresso => Set<RegistroProgresso>();
    public DbSet<Foto> Fotos => Set<Foto>();
    public DbSet<InteracaoCliente> InteracoesClientes => Set<InteracaoCliente>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ===== OBRA =====
        modelBuilder.Entity<Obra>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Endereco).HasMaxLength(300);
            entity.Property(e => e.ValorOrcado).HasPrecision(18, 2);
            entity.Property(e => e.ValorRealizado).HasPrecision(18, 2);
            entity.Property(e => e.AreaTotal).HasPrecision(10, 2);
            entity.Property(e => e.PercentualConcluido).HasPrecision(5, 2);

            entity.HasOne(e => e.Cliente)
                .WithMany(c => c.Obras)
                .HasForeignKey(e => e.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.DataInicio);
        });

        // ===== CLIENTE =====
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CPF).HasMaxLength(14);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Telefone).HasMaxLength(20);
            entity.Property(e => e.WhatsApp).HasMaxLength(20);
            entity.Property(e => e.ValorPotencial).HasPrecision(18, 2);

            entity.HasIndex(e => e.CPF).IsUnique();
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.Status);
        });

        // ===== ORÇAMENTO =====
        modelBuilder.Entity<Orcamento>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Numero).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ValorMateriais).HasPrecision(18, 2);
            entity.Property(e => e.ValorMaoObra).HasPrecision(18, 2);

            entity.HasOne(e => e.Cliente)
                .WithMany(c => c.Orcamentos)
                .HasForeignKey(e => e.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.Numero).IsUnique();
            entity.HasIndex(e => e.Status);
        });

        // ===== ITEM ORÇAMENTO =====
        modelBuilder.Entity<ItemOrcamento>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Descricao).IsRequired().HasMaxLength(300);
            entity.Property(e => e.Quantidade).HasPrecision(10, 2);
            entity.Property(e => e.ValorUnitario).HasPrecision(18, 2);

            entity.HasOne<Orcamento>()
                .WithMany(o => o.Itens)
                .HasForeignKey(e => e.OrcamentoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== MEDIÇÃO =====
        modelBuilder.Entity<Medicao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Numero).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PercentualMedido).HasPrecision(5, 2);
            entity.Property(e => e.ValorMedido).HasPrecision(18, 2);

            entity.HasOne(e => e.Obra)
                .WithMany(o => o.Medicoes)
                .HasForeignKey(e => e.ObraId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Status);
        });

        // ===== ITEM MEDIÇÃO =====
        modelBuilder.Entity<ItemMedicao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ValorItem).HasPrecision(18, 2);
            entity.Property(e => e.PercentualExecutado).HasPrecision(5, 2);

            entity.HasOne<Medicao>()
                .WithMany(m => m.Itens)
                .HasForeignKey(e => e.MedicaoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== FUNCIONÁRIO =====
        modelBuilder.Entity<Funcionario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CPF).HasMaxLength(14);
            entity.Property(e => e.SalarioDiaria).HasPrecision(10, 2);

            entity.HasIndex(e => e.CPF);
            entity.HasIndex(e => e.Ativo);
        });

        // ===== PRESENÇA =====
        modelBuilder.Entity<RegistroPresenca>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Funcionario)
                .WithMany(f => f.Presencas)
                .HasForeignKey(e => e.FuncionarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Obra)
                .WithMany()
                .HasForeignKey(e => e.ObraId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.FuncionarioId, e.Data });
        });

        // ===== PRODUTIVIDADE =====
        modelBuilder.Entity<RegistroProdutividade>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantidade).HasPrecision(10, 2);

            entity.HasOne<Funcionario>()
                .WithMany(f => f.Produtividade)
                .HasForeignKey(e => e.FuncionarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Data);
        });

        // ===== MATERIAL =====
        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.EstoqueAtual).HasPrecision(10, 2);
            entity.Property(e => e.EstoqueMinimo).HasPrecision(10, 2);
            entity.Property(e => e.PrecoUnitario).HasPrecision(10, 2);

            entity.HasIndex(e => e.Categoria);
        });

        // ===== MOVIMENTO ESTOQUE =====
        modelBuilder.Entity<MovimentoEstoque>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantidade).HasPrecision(10, 2);
            entity.Property(e => e.ValorUnitario).HasPrecision(10, 2);

            entity.HasOne(e => e.Material)
                .WithMany(m => m.Movimentos)
                .HasForeignKey(e => e.MaterialId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Obra)
                .WithMany()
                .HasForeignKey(e => e.ObraId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.Data);
        });

        // ===== DESPESA =====
        modelBuilder.Entity<Despesa>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Valor).HasPrecision(18, 2);

            entity.HasOne(e => e.Obra)
                .WithMany(o => o.Despesas)
                .HasForeignKey(e => e.ObraId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Categoria);
            entity.HasIndex(e => e.Data);
        });

        // ===== PROGRESSO =====
        modelBuilder.Entity<RegistroProgresso>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PercentualEtapa).HasPrecision(5, 2);

            entity.HasOne(e => e.Obra)
                .WithMany(o => o.RegistrosProgresso)
                .HasForeignKey(e => e.ObraId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Data);
        });

        // ===== FOTO =====
        modelBuilder.Entity<Foto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Url).IsRequired().HasMaxLength(500);

            entity.HasOne(e => e.Obra)
                .WithMany(o => o.Fotos)
                .HasForeignKey(e => e.ObraId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== INTERAÇÃO CLIENTE =====
        modelBuilder.Entity<InteracaoCliente>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Cliente)
                .WithMany(c => c.Interacoes)
                .HasForeignKey(e => e.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Data);
            entity.HasIndex(e => e.Tipo);
        });

        // ===== SEED DATA =====
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Cliente inicial
        var clienteId = Guid.NewGuid();
        modelBuilder.Entity<Cliente>().HasData(new Cliente
        {
            Id = clienteId,
            Nome = "João Silva",
            CPF = "123.456.789-00",
            Email = "joao@email.com",
            Telefone = "17 99164-2412",
            WhatsApp = "17991642412",
            Cidade = "São José do Rio Preto",
            Estado = "SP",
            Status = StatusCliente.Fechado,
            ProbabilidadeFechamento = 100,
            ValorPotencial = 420000,
            CriadoEm = DateTime.Now
        });

        // Obra inicial - Casa 350m²
        var obraId = Guid.NewGuid();
        modelBuilder.Entity<Obra>().HasData(new Obra
        {
            Id = obraId,
            Nome = "Casa 350m² - João Silva",
            Descricao = "Construção residencial completa",
            ClienteId = clienteId,
            Endereco = "Rua das Flores, 123",
            Cidade = "São José do Rio Preto",
            Estado = "SP",
            CEP = "15000-000",
            AreaTotal = 500,
            AreaConstruida = 350,
            Pavimentos = 2,
            Quartos = 4,
            Banheiros = 3,
            ValorOrcado = 420000,
            ValorRealizado = 0,
            DataInicio = DateTime.Now,
            DataPrevisaoTermino = DateTime.Now.AddMonths(6),
            PercentualConcluido = 0,
            Status = StatusObra.EmAndamento,
            CriadoEm = DateTime.Now,
            CriadoPor = "Marcos"
        });

        // Materiais básicos
        var cimentoId = Guid.NewGuid();
        var areiaId = Guid.NewGuid();

        modelBuilder.Entity<Material>().HasData(
            new Material
            {
                Id = cimentoId,
                Nome = "Cimento CP-II 50kg",
                Categoria = "Cimento",
                Unidade = "saco",
                EstoqueAtual = 100,
                EstoqueMinimo = 50,
                PrecoUnitario = 32.50m
            },
            new Material
            {
                Id = areiaId,
                Nome = "Areia Média",
                Categoria = "Areia",
                Unidade = "m³",
                EstoqueAtual = 20,
                EstoqueMinimo = 10,
                PrecoUnitario = 120.00m
            }
        );
    }
}
