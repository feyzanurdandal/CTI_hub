using CtiHub.Application.DTOs;
using CtiHub.Application.Validators;
using Xunit;

namespace CtiHub.Tests.Validators;

public class LoginDtoValidatorTests
{
    private readonly LoginDtoValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenEmailOrPasswordIsInvalid()
    {
        var dto = new LoginDto
        {
            Email = "not-an-email",
            Password = string.Empty
        };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(LoginDto.Email));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(LoginDto.Password));
    }

    [Fact]
    public void Validate_ShouldPass_WhenLoginDtoIsValid()
    {
        var dto = new LoginDto
        {
            Email = "recep@example.com",
            Password = "123456"
        };

        var result = _validator.Validate(dto);

        Assert.True(result.IsValid);
    }
}
