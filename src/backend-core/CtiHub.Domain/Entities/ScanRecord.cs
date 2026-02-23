namespace CtiHub.Domain.Entities;

public class ScanRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // Taranacak Hedef (Örn: google.com)
    public string TargetUrl { get; set; } = string.Empty;
    
    // Taramanın Durumu (Pending, InProgress, Completed, Failed)
    public string Status { get; set; } = "Pending"; 
    
    // İleride OSINT/Nmap sonuçlarını buraya JSON olarak basacağız
    public string? ResultData { get; set; } 
    
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}