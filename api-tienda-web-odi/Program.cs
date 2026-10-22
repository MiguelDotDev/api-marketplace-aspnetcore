using api_tienda_web_odi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using api_tienda_web_odi.Infraestructure;
using api_tienda_web_odi.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Registrar DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default")
    );
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("MiPoliticaCors", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductosService, ProductosService>();
builder.Services.AddScoped<IChatsService, ChatsService>();
builder.Services.AddScoped<ICategoriasService, CategoriasService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplica automáticamente las migraciones pendientes de EF Core al arrancar.
// Así, en cualquier computadora nueva basta con "docker compose up -d":
// no hace falta tener instalado el SDK de .NET ni la herramienta dotnet-ef.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    const int maxIntentos = 10;
    for (int intento = 1; intento <= maxIntentos; intento++)
    {
        try
        {
            logger.LogInformation("Aplicando migraciones (intento {Intento}/{Max})...", intento, maxIntentos);
            db.Database.Migrate();
            logger.LogInformation("Migraciones aplicadas correctamente.");
            break;
        }
        catch (Exception ex)
        {
            if (intento == maxIntentos)
            {
                logger.LogError(ex, "No se pudieron aplicar las migraciones tras {Max} intentos.", maxIntentos);
                throw;
            }
            logger.LogWarning("SQL Server no está listo todavía. Reintentando en 5s...");
            Thread.Sleep(5000);
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseCors("MiPoliticaCors");
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
