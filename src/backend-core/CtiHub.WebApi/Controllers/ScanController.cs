using CtiHub.Application.Common.Interfaces;
using CtiHub.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CtiHub.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ScanController : ControllerBase
{
    private readonly IRabbitMqService _rabbitMqService;

    // RabbitMQ Servisini buraya çağırıyoruz (Dependency Injection)
    public ScanController(IRabbitMqService rabbitMqService)
    {
        _rabbitMqService = rabbitMqService;
    }

    [HttpPost("start-scan")]
    // [Authorize] // Şimdilik kapalı tutalım, test ederken token ile uğraşmayalım
    public async Task<IActionResult> StartScan([FromBody] ScanRequestDto request)
    {
        // 1. Basit bir validasyon
        if (string.IsNullOrEmpty(request.TargetUrl))
        {
            return BadRequest("Lütfen bir hedef URL girin.");
        }

        // 2. Mesajı hazırla (İleride buraya UserID, Tarih vs. de ekleyeceğiz)
        var message = new 
        { 
            Url = request.TargetUrl, 
            RequestedAt = DateTime.UtcNow,
            Status = "Pending"
        };

        // 3. RabbitMQ Kuyruğuna Gönder! 🐇
        // "scan_queue" adında bir kuyruk oluşturup içine atacak.
        await _rabbitMqService.SendMessageAsync(message, "scan_queue");

        // 4. Kullanıcıya hemen cevap dön (Bekletmek yok!)
        return Ok(new 
        { 
            message = "Tarama isteği alındı ve kuyruğa eklendi.", 
            target = request.TargetUrl 
        });
    }
}