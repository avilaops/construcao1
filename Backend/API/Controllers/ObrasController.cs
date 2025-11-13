using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarcosConstrutora.Infrastructure.Data;
using MarcosConstrutora.Core.Entities;

namespace MarcosConstrutora.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ObrasController : ControllerBase
{
    private readonly MarcosDbContext _context;
    private readonly ILogger<ObrasController> _logger;

    public ObrasController(MarcosDbContext context, ILogger<ObrasController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Listar todas as obras
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Obra>>> GetObras()
    {
        var obras = await _context.Obras
            .Include(o => o.Cliente)
            .Include(o => o.Medicoes)
            .Include(o => o.Despesas)
            .OrderByDescending(o => o.DataInicio)
            .ToListAsync();

        return Ok(obras);
    }

    /// <summary>
    /// Buscar obra por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Obra>> GetObra(Guid id)
    {
        var obra = await _context.Obras
            .Include(o => o.Cliente)
            .Include(o => o.Medicoes)
            .Include(o => o.Despesas)
            .Include(o => o.RegistrosProgresso)
            .Include(o => o.Fotos)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (obra == null)
            return NotFound();

        return Ok(obra);
    }

    /// <summary>
    /// Criar nova obra
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Obra>> CreateObra(Obra obra)
    {
        obra.Id = Guid.NewGuid();
        obra.CriadoEm = DateTime.Now;

        _context.Obras.Add(obra);
        await _context.SaveChangesAsync();

        _logger.LogInformation("🏗️ Nova obra criada: {Nome}", obra.Nome);

        return CreatedAtAction(nameof(GetObra), new { id = obra.Id }, obra);
    }

    /// <summary>
    /// Atualizar obra
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateObra(Guid id, Obra obra)
    {
        if (id != obra.Id)
            return BadRequest();

        obra.AtualizadoEm = DateTime.Now;
        _context.Entry(obra).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ObraExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Deletar obra
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteObra(Guid id)
    {
        var obra = await _context.Obras.FindAsync(id);
        if (obra == null)
            return NotFound();

        _context.Obras.Remove(obra);
        await _context.SaveChangesAsync();

        _logger.LogWarning("🗑️ Obra deletada: {Nome}", obra.Nome);

        return NoContent();
    }

    /// <summary>
    /// Atualizar progresso da obra
    /// </summary>
    [HttpPost("{id}/progresso")]
    public async Task<IActionResult> UpdateProgresso(Guid id, [FromBody] RegistroProgressoDto dto)
    {
        var obra = await _context.Obras.FindAsync(id);
        if (obra == null)
            return NotFound();

        var registro = new RegistroProgresso
        {
            ObraId = id,
            Etapa = dto.Etapa,
            PercentualEtapa = dto.PercentualEtapa,
            Descricao = dto.Descricao,
            RegistradoPor = dto.RegistradoPor
        };

        _context.RegistrosProgresso.Add(registro);

        // Atualizar percentual geral da obra
        var progressos = await _context.RegistrosProgresso
            .Where(r => r.ObraId == id)
            .ToListAsync();

        obra.PercentualConcluido = progressos.Average(p => p.PercentualEtapa);
        obra.AtualizadoEm = DateTime.Now;

        await _context.SaveChangesAsync();

        _logger.LogInformation("📈 Progresso atualizado - Obra: {Nome}, Etapa: {Etapa}, {Percentual}%",
            obra.Nome, dto.Etapa, dto.PercentualEtapa);

        return Ok(new { obra.PercentualConcluido, registro });
    }

    /// <summary>
    /// Upload de foto da obra
    /// </summary>
    [HttpPost("{id}/fotos")]
    public async Task<IActionResult> AddFoto(Guid id, [FromBody] FotoDto dto)
    {
        var obra = await _context.Obras.FindAsync(id);
        if (obra == null)
            return NotFound();

        var foto = new Foto
        {
            ObraId = id,
            Url = dto.Url,
            Descricao = dto.Descricao,
            Etapa = dto.Etapa,
            TiradaPor = dto.TiradaPor
        };

        _context.Fotos.Add(foto);
        await _context.SaveChangesAsync();

        return Ok(foto);
    }

    /// <summary>
    /// Dashboard - resumo de todas obras
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<ActionResult> GetDashboard()
    {
        var obrasAtivas = await _context.Obras
            .Where(o => o.Status == StatusObra.EmAndamento)
            .Include(o => o.Despesas)
            .ToListAsync();

        var totalOrcado = obrasAtivas.Sum(o => o.ValorOrcado);
        var totalGasto = obrasAtivas.Sum(o => o.Despesas.Sum(d => d.Valor));
        var margemMedia = obrasAtivas.Any() ? obrasAtivas.Average(o => o.MargemLucro) : 0;

        var dashboard = new
        {
            ObrasAtivas = obrasAtivas.Count,
            TotalOrcado = totalOrcado,
            TotalGasto = totalGasto,
            MargemMedia = margemMedia,
            Obras = obrasAtivas.Select(o => new
            {
                o.Id,
                o.Nome,
                o.PercentualConcluido,
                o.DiasRestantes,
                o.ValorOrcado,
                Gasto = o.Despesas.Sum(d => d.Valor),
                o.Status,
                Cliente = o.Cliente.Nome
            }).ToList()
        };

        return Ok(dashboard);
    }

    private async Task<bool> ObraExists(Guid id)
    {
        return await _context.Obras.AnyAsync(e => e.Id == id);
    }
}

// DTOs
public record RegistroProgressoDto(
    EtapaObra Etapa,
    decimal PercentualEtapa,
    string Descricao,
    string RegistradoPor
);

public record FotoDto(
    string Url,
    string? Descricao,
    EtapaObra? Etapa,
    string TiradaPor
);
