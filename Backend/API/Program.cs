using Microsoft.EntityFrameworkCore;
using MarcosConstrutora.Infrastructure.Data;
using MarcosConstrutora.API.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ===== LOGGING =====
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/marcos-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ===== DATABASE =====
builder.Services.AddDbContext<MarcosDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Server=(localdb)\\mssqllocaldb;Database=MarcosConstrutora;Trusted_Connection=true;MultipleActiveResultSets=true";
    options.UseSqlServer(connectionString);
});

// ===== CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ===== CONTROLLERS =====
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ===== SWAGGER =====
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Marcos Construção API",
        Version = "v1",
        Description = "Sistema completo de gestão de obras e construção civil",
        Contact = new()
        {
            Name = "Ávila Framework",
            Email = "suporte@avila.ops"
        }
    });
});

// ===== AUTOMAPPER =====
builder.Services.AddAutoMapper(typeof(Program));

// ===== DEPENDENCY INJECTION =====
// Repositories
builder.Services.AddScoped<IObrasRepository, ObrasRepository>();
builder.Services.AddScoped<IOrcamentosRepository, OrcamentosRepository>();
builder.Services.AddScoped<IMedicoesRepository, MedicoesRepository>();
builder.Services.AddScoped<IEquipeRepository, EquipeRepository>();
builder.Services.AddScoped<IMateriaisRepository, MateriaisRepository>();
builder.Services.AddScoped<IFinanceiroRepository, FinanceiroRepository>();
builder.Services.AddScoped<IClientesRepository, ClientesRepository>();

// Services
builder.Services.AddScoped<IObrasService, ObrasService>();
builder.Services.AddScoped<IOrcamentosService, OrcamentosService>();
builder.Services.AddScoped<IWhatsAppService, WhatsAppService>();
builder.Services.AddScoped<IPdfService, PdfService>();

var app = builder.Build();

// ===== MIDDLEWARE =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Marcos Construção API v1");
        c.RoutePrefix = string.Empty; // Swagger na raiz
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

// ===== DATABASE MIGRATION =====
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MarcosDbContext>();
    db.Database.Migrate();
}

Log.Information("🏗️ Marcos Construção API iniciada em {Time}", DateTime.Now);

app.Run();
