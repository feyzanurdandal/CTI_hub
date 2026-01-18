using CtiHub.Domain.Common;

namespace CtiHub.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    // Bir kullanýcýnýn birden fazla hedefi (Domain/IP) olabilir.
    public ICollection<Target> Targets { get; set; } = new List<Target>();
}