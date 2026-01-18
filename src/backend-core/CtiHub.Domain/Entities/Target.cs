using CtiHub.Domain.Common;

namespace CtiHub.Domain.Entities;

// Hedef: Taranacak site veya IP (örn: google.com)
public class Target : BaseEntity
{
    public string Name { get; set; } = string.Empty; // Örn: "Müþteri Web Sitesi"
    public string Host { get; set; } = string.Empty; // Örn: "192.168.1.1"

    // Ýliþki: Bu hedef kime ait?
    public Guid UserId { get; set; }
    public User User { get; set; } = null!; // Navigation Property

    // Bu hedefe yapýlmýþ taramalarýn listesi
    public ICollection<Scan> Scans { get; set; } = new List<Scan>();
}