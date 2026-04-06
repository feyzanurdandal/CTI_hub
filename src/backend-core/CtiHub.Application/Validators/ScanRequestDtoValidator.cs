using CtiHub.Application.DTOs;
using FluentValidation;

namespace CtiHub.Application.Validators;

public class ScanRequestDtoValidator : AbstractValidator<ScanRequestDto>
{
    public ScanRequestDtoValidator()
    {
        RuleFor(x => x.TargetUrl)
            .NotEmpty().WithMessage("Hedef URL bos olamaz.")
            .MaximumLength(255).WithMessage("Hedef URL 255 karakterden uzun olamaz.");
    }
}
