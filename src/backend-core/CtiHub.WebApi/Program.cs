using CtiHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore; // Entity Framework (ORM) araçları
using CtiHub.Application.Common.Interfaces; //  (Interface) burada
using CtiHub.Infrastructure.Repositories;   //  (Implementation) burada

// "Builder" nesnesi, uygulamayı inşa etmeye yarar
var builder = WebApplication.CreateBuilder(args);

// --- SERVİSLER --- (DEPENDENCY INJECTION Kısımları) : uygulamanın ihtiyacı olacak her şey burada
builder.Services.AddControllers(); // Controller mekanizmasını (API) ekle.
// Swagger (API Dokümantasyonu) için gerekli servisler.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Yetkilendirme (İleride Login yapınca burası devreye girecek).
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