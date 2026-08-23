using FluentValidation;
using LibraryInventory.Api.Dtos.Books;

namespace LibraryInventory.Api.Validators.Books;

public class UpdateBookDtoValidator : AbstractValidator<UpdateBookDto>
{
    public UpdateBookDtoValidator()
    {
        // Same rules as Create — DRY in real projects via a shared base class
        RuleFor(x => x.BookName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Author).NotEmpty().MaximumLength(150);
        RuleFor(x => x.IsbnNumber).NotEmpty().Length(10, 13).Matches(@"^\d{10}(\d{3})?$");
        RuleFor(x => x.PageCount).GreaterThan(0).LessThanOrEqualTo(10_000);
        RuleFor(x => x.Price).GreaterThan(0).LessThan(10_000);
        RuleFor(x => x.PageMaterial).IsInEnum();
        RuleFor(x => x.PhotoUrl).NotNull();
    }
}