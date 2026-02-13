using CtiHub.Infrastructure; // Bunu unutma
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using FluentValidation.AspNetCore;
using CtiHub.Infrastructure.Persistence; // Migration için gerekli
using Microsoft.EntityFrameworkCore;     // Migration için gerekli

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVİS KAYITLARI (Dependency Injection) ---

// Controller ve Validasyon
builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CtiHub.Application.Validators.CreateUserDtoValidator>());

// *** TEK SATIRDA TÜM ALTYAPIYI YÜKLE ***
// (Veritabanı, Repository, RabbitMQ hepsi bunun içinde artık)
builder.Services.AddInfrastructureServices(builder.Configuration);

// JWT Ayarları
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
        };
    });

// Swagger Ayarları
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CtiHub.WebApi", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Token giriniz",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
       { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, new string[] { } }
    });
});


// --- DEBUG KODU BAŞLANGICI ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"\n\n---------------------------------------------------------");
Console.WriteLine($" UYGULAMANIN GÖRDÜĞÜ BAĞLANTI ADRESİ: \n{connectionString}");
Console.WriteLine($"---------------------------------------------------------\n\n");
// --- DEBUG KODU BİTİŞİ ---

var app = builder.Build();

// --- 2. MIDDLEWARE (Boru Hattı) ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// --- 3. AUTO MIGRATION ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
        Console.WriteLine("--> Veritabani migration basarili.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"--> Migration Hatasi: {ex.Message}");
    }
}

app.Run();