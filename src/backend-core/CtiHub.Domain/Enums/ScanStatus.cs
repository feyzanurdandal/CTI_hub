namespace CtiHub.Domain.Enums;

public enum ScanStatus
{
    Pending = 1,    // Sýrada bekliyor
    Running = 2,    // Tarama yapýlýyor
    Completed = 3,  // Bitti
    Failed = 4      // Hata oluþtu
}