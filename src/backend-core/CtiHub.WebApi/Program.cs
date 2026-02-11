using CtiHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore; // Entity Framework (ORM) araçları
using CtiHub.Application.Common.Interfaces; //  (Interface) burada
using CtiHub.Infrastructure.Repositories;   //  (Implementation) burada
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using FluentValidation.AspNetCore;

// "Builder" nesnesi, uygulamayı inşa etmeye yarar
var builder = WebApplication.CreateBuilder(args);

// --- SERVİSLER --- (DEPENDENCY INJECTION Kısımları) : uygulamanın ihtiyacı olacak her şey burada
 // Controller mekanizmasını (API) ekle.
builder.Services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CtiHub.Application.Validators.CreateUserDtoValidator>());

// --- JWT AUTHENTICATION AYARLARI ---
// 1. Ayarları appsettings.json'dan oku
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

// 2. Sisteme "Biz JWT kullanacağız" de.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // 3. Token doğrulama kuralları
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Kartı kim bastı? (Biz mi?)
        ValidateAudience = true, // Kart kime verildi?
        ValidateLifetime = true, // Kartın süresi doldu mu?
        ValidateIssuerSigningKey = true, // İmza (Mühür) doğru mu?
        
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
    };
});

// Swagger (API Dokümantasyonu) için gerekli servisler.
builder.Services.AddEndpointsApiExplorer();
// --- SWAGGER AYARLARI (Kilit Butonu İçin) ---
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CtiHub.WebApi", Version = "v1" });

    // 1. Kilit butonunu tanımlıyoruz
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Lütfen kutuya 'Bearer <token>' şeklinde token yapıştırın",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    // 2. Kilit gereksinimini ekliyoruz
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
       {
         new OpenApiSecurityScheme
         {
           Reference = new OpenApiReference
           {
             Type = ReferenceType.SecurityScheme,
             Id = "Bearer"
           }
          },
          new string[] { }
       }
    });
});

// VERİTABANI BAİLANTISI (Dependency Injection)
// Uygulamaya diyoruz ki: "Veritabanı olarak PostgreSQL kullanacaksın."
// Bağlantı cümlesini (Connection String) de appsettings.json'dan veya User Secrets'tan al.
// Uygulama her "ApplicationDbContext" istendiğinde, bu ayarlarla bir tane üretip verecek.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- DEPENDENCY INJECTION (SERVİS KAYITLARI) ---
// Scoped: Her gelen HTTP isteği (Request) için yeni bir tane oluşturur.
// "AddScoped" demek: Her HTTP isteği (Request) geldiğinde yeni bir tane üret, istek bitince sil.
// Anlamı: "Biri senden IGenericRepository isterse, ona GenericRepository ver."
// typeof(IGenericRepository<>): Generic olduğu için <> içini boş bırakarak "Tüm tipler için geçerli" diyoruz.
// "typeof" kullanıyoruz çünkü Generic (<T>) bir yapı.
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

//"Build" diyerek uygulamayı (app) oluşturuyoruz.
var app = builder.Build();

// --- 5. HTTP İSTEK BORU HATTI (MIDDLEWARE PIPELINE) ---
// Gelen bir istek (Request) sırasıyla bu kapılardan geçer.

// Eğer Geliştirme (Development) ortamındaysak Swagger'ı aç.
// (Canlı ortamda güvenlik için kapatılır).
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   // Swagger JSON dosyasını oluştur.
    app.UseSwaggerUI(); // arayüzü göster.
}

// HTTP isteklerini HTTPS'e yönlendir (Güvenlik).
app.UseHttpsRedirection();

// Önce kimlik sor (Authentication)
app.UseAuthentication(); 

// Sonra yetkisine bak (Authorization)
app.UseAuthorization();

// Gelen isteği ilgili Controller'a yönlendir (Rotayı bul).
app.MapControllers();

// --- OTOMATİK VERİTABANI GÜNCELLEME (AUTO-MIGRATION) ---
// Uygulama her başladığında çalışacak özel kod bloğumuz.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Kutudan DbContext'i istiyoruz.
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Veritabanına bak, eksik tablo varsa oluştur (dotnet ef database update'in kod hali).
        context.Database.Migrate(); 
        Console.WriteLine("--> Veritabani basariyla migrate edildi.");
    }
    catch (Exception ex)
    {
        // Bir hata olursa konsola yaz.
        Console.WriteLine($"--> Veritabani migration hatasi: {ex.Message}");
    }
}

// --- BAŞLAT ---
// Uygulamayı çalıştır ve istekleri dinlemeye başla.
app.Run();