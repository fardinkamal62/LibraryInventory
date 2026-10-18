using FluentValidation;
using LibraryInventory.Api.Dtos.Checkouts;

namespace LibraryInventory.Api.Validators.Checkouts;

public class CreateCheckoutDtoValidator : AbstractValidator<CreateCheckoutDto>
{
    public CreateCheckoutDtoValidator()
    {
        RuleFor(x => x.BookId).GreaterThan(0);
        RuleFor(x => x.MemberId).GreaterThan(0);
    }
}