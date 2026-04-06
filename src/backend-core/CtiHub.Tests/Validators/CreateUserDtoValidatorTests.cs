using CtiHub.Application.DTOs;
using CtiHub.Application.Validators;
using Xunit;

namespace CtiHub.Tests.Validators;

public class CreateUserDtoValidatorTests
{
    private readonly CreateUserDtoValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenRequiredFieldsAreEmpty()
    {
        var dto = new CreateUserDto
        {
            Username = string.Empty,
            Email = string.Empty,
            Password = string.Empty,
            FirstName = "Recep",
            LastName = "Yilmaz"
        };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateUserDto.Username));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateUserDto.Email));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateUserDto.Password));
    }

    [Fact]
    public void Validate_ShouldPass_WhenDtoIsValid()
    {
        var dto = new CreateUserDto
        {
            Username = "recep",
            Email = "recep@example.com",
            Password = "123456",
            FirstName = "Recep",
            LastName = "Yilmaz"
        };

        var result = _validator.Validate(dto);

        Assert.True(result.IsValid);
    }
}
