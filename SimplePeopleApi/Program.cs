using System.Text;
using System.IO;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimplePeopleApi.Data;
using SimplePeopleApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Force Kestrel to listen on a known HTTP port so we can verify app is running locally
builder.WebHost.UseUrls("http://localhost:5002");

// Configuration
var configuration = builder.Configuration;

// Add services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JWT
var jwtSection = configuration.GetSection("Jwt");
var key = jwtSection.GetValue<string>("Key");
var issuer = jwtSection.GetValue<string>("Issuer");
var audience = jwtSection.GetValue<string>("Audience");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

builder.Services.AddSingleton<IJwtService>(new JwtService(key, issuer, audience));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = signingKey
    };
});

builder.Services.AddAuthorization();

// Serve static files (simple frontend)
// static files served from wwwroot (simple frontend)

var app = builder.Build();

// Ensure DB created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    // Ensure 'uf' column exists in 'pessoas' table (when upgrading schema without migrations)
    try
    {
        var sql = @"IF COL_LENGTH('pessoas','uf') IS NULL BEGIN ALTER TABLE pessoas ADD uf VARCHAR(2) NULL END";
        db.Database.ExecuteSqlRaw(sql);

        // Create ContasAPagar table if missing
        var sqlApagar = @"
IF OBJECT_ID('dbo.ContasAPagar','U') IS NULL
BEGIN
    CREATE TABLE dbo.ContasAPagar(
        Numero BIGINT NOT NULL PRIMARY KEY,
        CodigoFornecedor INT NOT NULL,
        DataVencimento DATETIME2 NOT NULL,
        DataProrrogacao DATETIME2 NULL,
        Valor DECIMAL(18,6) NOT NULL,
        Acrescimo DECIMAL(18,6) NULL,
        Desconto DECIMAL(18,6) NULL,
        InseridoPor VARCHAR(250) NULL,
        CONSTRAINT FK_ContasAPagar_Pessoas FOREIGN KEY (CodigoFornecedor) REFERENCES pessoas(Codigo)
    )
END
";
        db.Database.ExecuteSqlRaw(sqlApagar);

        // Create ContasPagas table if missing
        var sqlPagas = @"
IF OBJECT_ID('dbo.ContasPagas','U') IS NULL
BEGIN
    CREATE TABLE dbo.ContasPagas(
        Numero BIGINT NOT NULL PRIMARY KEY,
        CodigoFornecedor INT NOT NULL,
        DataVencimento DATETIME2 NOT NULL,
        DataPagamento DATETIME2 NOT NULL,
        Valor DECIMAL(18,6) NOT NULL,
        Acrescimo DECIMAL(18,6) NULL,
        Desconto DECIMAL(18,6) NULL,
        InseridoPor VARCHAR(250) NULL,
        CONSTRAINT FK_ContasPagas_Pessoas FOREIGN KEY (CodigoFornecedor) REFERENCES pessoas(Codigo)
    )
END
";
        db.Database.ExecuteSqlRaw(sqlPagas);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Schema upgrade (uf/contas) check failed: " + ex.Message);
    }

    // Seed a default admin user and sample pessoa if missing (useful for development/testing)
    try
    {
        if (!db.Usuarios.Any())
        {
            db.Usuarios.Add(new SimplePeopleApi.Models.Usuario
            {
                Nome = "admin",
                Senha = BCrypt.Net.BCrypt.HashPassword("Test@123"),
                DataCriacao = DateTime.Now
            });
            db.SaveChanges();
        }

        if (!db.Pessoas.Any())
        {
            db.Pessoas.Add(new SimplePeopleApi.Models.Pessoa
            {
                Nome = "João da Silva",
                CPF = "12345678901",
                DataDeNascimento = new DateTime(1990,1,1),
                DataDeCriacao = DateTime.Now,
                UF = "SP"
            });
            db.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        // swallow seed errors in development but write to console
        Console.WriteLine("Seed error: " + ex.Message);
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Serve index.html
app.UseDefaultFiles();
app.UseStaticFiles();

// crash / shutdown logging helper
void LogShutdown(Exception? ex, string source)
{
    try
    {
        var logDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");
        Directory.CreateDirectory(logDir);
        var path = Path.Combine(logDir, "shutdown.txt");
        var txt = $"{DateTime.Now:O} [{source}]\n" + (ex?.ToString() ?? "(no exception)") + "\n\n";
        File.AppendAllText(path, txt);
    }
    catch { /* swallow logging errors */ }
}

AppDomain.CurrentDomain.UnhandledException += (s, e) =>
{
    LogShutdown(e.ExceptionObject as Exception, "UnhandledException");
};

TaskScheduler.UnobservedTaskException += (s, e) =>
{
    LogShutdown(e.Exception, "UnobservedTaskException");
    e.SetObserved();
};

// register lifetime events to capture graceful shutdowns
var lifetime = app.Lifetime;
lifetime.ApplicationStarted.Register(() => LogShutdown(null, "ApplicationStarted"));
lifetime.ApplicationStopping.Register(() => LogShutdown(null, "ApplicationStopping"));
lifetime.ApplicationStopped.Register(() => LogShutdown(null, "ApplicationStopped"));

try
{
    app.Run();
}
catch (Exception ex)
{
    LogShutdown(ex, "RunException");
    throw;
}
