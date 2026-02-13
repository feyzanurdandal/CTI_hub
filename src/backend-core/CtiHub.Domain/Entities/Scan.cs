using CtiHub.Domain.Common;
using CtiHub.Domain.Enums;

namespace CtiHub.Domain.Entities;

public class Scan : BaseEntity
{
    // Hangi hedef taranıyor?
    public Guid TargetId { get; set; }
    public Target Target { get; set; } = null!;

    // Tarama durumu (Sırada, Bitti vs.)
    public ScanStatus Status { get; set; } = ScanStatus.Pending;

    // Tarama ne kadar sürdü?
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }

    // Go motorundan gelen sonuç buraya yazılacak.
    // JSON formatında tutacağız.
    public string? ResultJson { get; set; }
}