using CtiHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- SERVÝSLER ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// VERÝTABANI BAÐLANTISI (Dependency Injection)
// Uygulama her "ApplicationDbContext" istendiðinde, bu ayarlarla bir tane üretip verecek.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// --- HTTP AYARLARI ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// --- OTOMATÝK MIGRATION (UYGULAMA BAÞLARKEN DB'YÝ KUR) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Bu komut, "dotnet ef database update" komutunun kod karþýlýðýdýr.
        // Veritabaný yoksa oluþturur, varsa eksik tablolarý ekler.
        context.Database.Migrate();
        Console.WriteLine("--> Veritabaný baþarýyla migrate edildi.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"--> Veritabaný migration hatasý: {ex.Message}");
    }
}

app.Run();