using FluentValidation;
using LibraryInventory.Api.Dtos.Books;

namespace LibraryInventory.Api.Validators.Books;

public class CreateBookDtoValidator : AbstractValidator<CreateBookDto>
{
    public CreateBookDtoValidator()
    {
        RuleFor(x => x.BookName)
            .NotEmpty().WithMessage("Book name is required")
            .MaximumLength(255);

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Author is required")
            .MaximumLength(150);

        RuleFor(x => x.IsbnNumber)
            .NotEmpty()
            .Length(10, 13).WithMessage("ISBN must be 10 or 13 characters")
            .Matches(@"^\d{10}(\d{3})?$").WithMessage("ISBN must contain only digits");

        RuleFor(x => x.PageCount)
            .GreaterThan(0).WithMessage("Page count must be positive")
            .LessThanOrEqualTo(10_000);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be positive")
            .LessThan(10_000);

        RuleFor(x => x.PageMaterial)
            .IsInEnum().WithMessage("Page material must be Paperback or Hardcover");

        RuleFor(x => x.PhotoUrl)
            .NotNull()
            .Must(uri => uri == null || (uri.IsAbsoluteUri && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)))
            .WithMessage("PhotoUrl must be a valid absolute http/https URL");

        RuleFor(x => x.TotalCopies)
            .InclusiveBetween(1, 1000).WithMessage("Total copies must be between 1 and 1000");
    }
}
