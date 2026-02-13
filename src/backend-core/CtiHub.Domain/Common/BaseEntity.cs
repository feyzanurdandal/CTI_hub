namespace CtiHub.Domain.Common;

public abstract class BaseEntity
{
    // GUID: Benzersiz kimlik. Dağıtık sistemler için int'ten daha güvenli.
    public Guid Id { get; set; } = Guid.NewGuid();

    // Audit (izleme) Alanları
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Veriyi gerçekten silmeyiz, sadece "Silindi" işaretleriz (Soft Delete).
    public bool IsDeleted { get; set; } = false;
}