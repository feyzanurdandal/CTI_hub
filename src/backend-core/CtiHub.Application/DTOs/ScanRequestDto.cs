namespace CtiHub.Application.DTOs;

public class ScanRequestDto
{
    // Kullanıcının taratmak istediği hedef (Örn: google.com)
    public string TargetUrl { get; set; } = string.Empty;
}