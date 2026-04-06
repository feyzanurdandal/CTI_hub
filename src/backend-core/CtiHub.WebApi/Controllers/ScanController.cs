using CtiHub.Application.Common.Interfaces;
using CtiHub.Application.DTOs;
using CtiHub.Infrastructure.Persistence; // Veritabanı bağlantısı için eklendi
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CtiHub.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ScanController : ControllerBase
{
    private readonly IRabbitMqService _rabbitMqService;
    private readonly ApplicationDbContext _dbContext; // Veritabanı nesnemiz

    // Constructor (Kurucu Metot) - DbContext'i de içeri alıyoruz
    public ScanController(IRabbitMqService rabbitMqService, ApplicationDbContext dbContext)
    {
        _rabbitMqService = rabbitMqService;
        _dbContext = dbContext;
    }

    [HttpPost("start-scan")]
    public async Task<IActionResult> StartScan([FromBody] ScanRequestDto request)
    {
        if (string.IsNullOrEmpty(request.TargetUrl))
            return BadRequest("Lütfen bir hedef URL girin.");

        var message = new { Url = request.TargetUrl, RequestedAt = DateTime.UtcNow, Status = "Pending" };

        // Mesajı kuyruğa gönder
        await _rabbitMqService.SendMessageAsync(message, "scan_queue");

        return Ok(new { message = "Tarama isteği alındı ve kuyruğa eklendi.", target = request.TargetUrl });
    }

    // YENİ EKLENEN KISIM: Geçmiş taramaları listeleme (GET)
    [HttpGet("history")]
    public async Task<IActionResult> GetScanHistory()
    {
        // Veritabanındaki "ScanRecords" tablosunu tarihe göre (en yeni en üstte) sıralayıp getiriyoruz
        var history = await _dbContext.ScanRecords
            .OrderByDescending(x => x.RequestedAt)
            .ToListAsync();

        return Ok(history);
    }
}