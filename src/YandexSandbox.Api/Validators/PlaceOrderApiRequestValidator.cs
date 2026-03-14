using FluentValidation;
using YandexSandbox.Api.Requests;

namespace YandexSandbox.Api.Validators;

public class PlaceOrderApiRequestValidator : AbstractValidator<PlaceOrderApiRequest>
{
    public PlaceOrderApiRequestValidator()
    {
        RuleFor(x => x.CarId)
            .GreaterThan(0).WithMessage("CarId must be a positive integer.");
    }
}
