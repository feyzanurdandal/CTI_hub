using CtiHub.Application.Common.Interfaces;
using CtiHub.Application.DTOs;
using CtiHub.Domain.Entities;
using CtiHub.Infrastructure.Persistence;
using CtiHub.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CtiHub.Tests.Controllers;

public class ScanControllerTests
{
    [Fact]
    public async Task StartScan_ShouldReturnBadRequest_WhenTargetUrlIsEmpty()
    {
        await using var dbContext = CreateDbContext();
        var rabbitMq = new FakeRabbitMqService();
        var controller = new ScanController(rabbitMq, dbContext);

        var result = await controller.StartScan(new ScanRequestDto { TargetUrl = string.Empty });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Lütfen bir hedef URL girin.", badRequest.Value);
        Assert.False(rabbitMq.WasCalled);
    }

    [Fact]
    public async Task StartScan_ShouldSendMessageAndReturnOk_WhenRequestIsValid()
    {
        await using var dbContext = CreateDbContext();
        var rabbitMq = new FakeRabbitMqService();
        var controller = new ScanController(rabbitMq, dbContext);

        var result = await controller.StartScan(new ScanRequestDto { TargetUrl = "google.com" });

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        Assert.True(rabbitMq.WasCalled);
        Assert.Equal("scan_queue", rabbitMq.LastQueueName);
    }

    [Fact]
    public async Task GetScanHistory_ShouldReturnRecordsOrderedByRequestedAtDescending()
    {
        await using var dbContext = CreateDbContext();
        dbContext.ScanRecords.AddRange(
            new ScanRecord { TargetUrl = "old.example", RequestedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ScanRecord { TargetUrl = "new.example", RequestedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
        await dbContext.SaveChangesAsync();

        var controller = new ScanController(new FakeRabbitMqService(), dbContext);

        var result = await controller.GetScanHistory();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var records = Assert.IsAssignableFrom<IEnumerable<ScanRecord>>(okResult.Value);
        var recordList = records.ToList();

        Assert.Equal(2, recordList.Count);
        Assert.Equal("new.example", recordList[0].TargetUrl);
        Assert.Equal("old.example", recordList[1].TargetUrl);
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"ctihub-tests-{Guid.NewGuid()}")
            .Options;

        return new ApplicationDbContext(options);
    }

    private sealed class FakeRabbitMqService : IRabbitMqService
    {
        public bool WasCalled { get; private set; }
        public string? LastQueueName { get; private set; }

        public Task SendMessageAsync<T>(T message, string queueName)
        {
            WasCalled = true;
            LastQueueName = queueName;
            return Task.CompletedTask;
        }
    }
}
