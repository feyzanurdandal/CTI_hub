namespace CtiHub.Domain.Enums;

public enum ScanStatus
{
    Pending = 1,    // Sırada bekliyor
    Running = 2,    // Tarama yapılıyor
    Completed = 3,  // Bitti
    Failed = 4      // Hata oluştu
}