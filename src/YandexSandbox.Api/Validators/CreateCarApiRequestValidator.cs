using FluentValidation;
using YandexSandbox.Api.Models;

namespace YandexSandbox.Api.Validators;

public class CreateCarApiRequestValidator : AbstractValidator<CreateCarApiRequest>
{
    public CreateCarApiRequestValidator()
    {
        RuleFor(x => x.Make)
            .NotEmpty().WithMessage("Make is required.")
            .MaximumLength(100);

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required.")
            .MaximumLength(100);

        RuleFor(x => x.Color)
            .NotEmpty().WithMessage("Color is required.")
            .MaximumLength(50);

        RuleFor(x => x.Mileage)
            .GreaterThanOrEqualTo(0).WithMessage("Mileage cannot be negative.");
    }
}
