using CtiHub.Application.DTOs;
using FluentValidation;

namespace CtiHub.Application.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email adresi bos olamaz.")
            .EmailAddress().WithMessage("Gecerli bir email adresi giriniz.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Sifre bos olamaz.")
            .MinimumLength(6).WithMessage("Sifre en az 6 karakter olmalidir.");
    }
}
