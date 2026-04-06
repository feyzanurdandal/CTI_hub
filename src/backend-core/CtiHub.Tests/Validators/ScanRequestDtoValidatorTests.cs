using CtiHub.Application.DTOs;
using CtiHub.Application.Validators;
using Xunit;

namespace CtiHub.Tests.Validators;

public class ScanRequestDtoValidatorTests
{
    private readonly ScanRequestDtoValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenTargetUrlIsEmpty()
    {
        var dto = new ScanRequestDto
        {
            TargetUrl = string.Empty
        };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ScanRequestDto.TargetUrl));
    }

    [Fact]
    public void Validate_ShouldPass_WhenTargetUrlIsProvided()
    {
        var dto = new ScanRequestDto
        {
            TargetUrl = "google.com"
        };

        var result = _validator.Validate(dto);

        Assert.True(result.IsValid);
    }
}
