using CtiHub.Application.Common.Interfaces;
using CtiHub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CtiHub.WebApi.Controllers;

// [Route]: Bu Controller'a nasıl ulaşılacak? -> api/users
// [ApiController]: Bu sınıfın bir API olduğunu belirtir (Otomatik validasyon vb. sağlar).
[Route("api/[controller]")]
[ApiController]
[Authorize] // <--- İŞTE BU SATIR KAPIYI KİLİTLER! 🔒
public class UsersController : ControllerBase
{
    // 1. ADIM: Garsonumuzu (Repository) tanımlıyoruz.
    // Dikkat: GenericRepository yerine IGenericRepository (Interface) kullanıyoruz.
    // Böylece "Ahmet'e" değil "Garson'a" bağlı oluyoruz.
    private readonly IGenericRepository<User> _userRepository;

    // 2. ADIM: CONSTRUCTOR INJECTION (Sihirli Kısım) ✨
    // Biz burada "new GenericRepository()" DEMİYORUZ.
    // Program.cs'e "Biri senden IGenericRepository isterse ver" demiştik ya, işte o burada devreye giriyor.
    // Uygulama çalışınca otomatik olarak buraya o sınıfı gönderiyor.
    public UsersController(IGenericRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    // GET: api/users
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // 1. Veritabanından HAM verileri (Entity) çek (İçinde şifre var)
        var users = await _userRepository.GetAllAsync();

        // 2. Güvenli kutuya (DTO) aktar (Şifreyi eledik)
        // (Select komutu, bir listeyi başka bir listeye dönüştürür)
        var userDtos = users.Select(user => new CtiHub.Application.DTOs.UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        }).ToList();
        
        // 3. Artık şifresiz listeyi gönder
        return Ok(userDtos);
    }

    // POST: api/users
    [HttpPost]
    public async Task<IActionResult> Create(CtiHub.Application.DTOs.CreateUserDto request) // Değişen Kısım: User yerine CreateUserDto
    {
        // 1. DTO'yu Entity'e Çevir (Mapping)
        // İleride bunu AutoMapper ile tek satırda yapacağız, şimdilik elle yapalım mantığı anla.
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            // NOT: Şifreyi asla düz metin (plain text) saklamamalıyız ama şimdilik böyle kalsın.
            PasswordHash = request.Password, 
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            IsDeleted = false
        };

        // 2. Veritabanına Kaydet
        await _userRepository.AddAsync(newUser);
        
        // 3. Sonuç Dön
        return Ok(new { message = "Kullanıcı başarıyla oluşturuldu", userId = newUser.Id });
    }

    // DELETE: api/users/{id}
    // Örnek kullanım: api/users/b960d14c-bb39...
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        // 1. Önce silinecek kullanıcı gerçekten var mı diye bak.
        var user = await _userRepository.GetByIdAsync(id);
        
        // Eğer yoksa "Bulunamadı" (404) hatası dön.
        if (user == null)
        {
            return NotFound(new { message = "Silinecek kullanıcı bulunamadı." });
        }

        // 2. Varsa garsona "Bunu sil" de.
        await _userRepository.DeleteAsync(user);

        // 3. İşlem başarılı mesajı dön.
        return Ok(new { message = "Kullanıcı başarıyla silindi." });
    }
}