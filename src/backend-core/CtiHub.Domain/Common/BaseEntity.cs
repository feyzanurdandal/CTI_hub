namespace CtiHub.Domain.Common;

public abstract class BaseEntity
{
    // GUID: Benzersiz kimlik. Daðýtýk sistemler için int'ten daha güvenli.
    public Guid Id { get; set; } = Guid.NewGuid();

    // Audit (Ýzleme) Alanlarý
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Veriyi gerçekten silmeyiz, sadece "Silindi" iþaretleriz (Soft Delete).
    public bool IsDeleted { get; set; } = false;
}