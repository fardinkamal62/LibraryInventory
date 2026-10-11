using FluentValidation;
using LibraryInventory.Api.Dtos.Books;

namespace LibraryInventory.Api.Validators.Books;

public class UpdateBookDtoValidator : AbstractValidator<UpdateBookDto>
{
    public UpdateBookDtoValidator()
    {
        RuleFor(x => x.BookName).NotEmpty().MaximumLength(255)
            .WithMessage("Book name is required and must be at most 255 characters");

        RuleFor(x => x.Author).NotEmpty().MaximumLength(150)
            .WithMessage("Author is required and must be at most 150 characters");

        RuleFor(x => x.IsbnNumber).NotEmpty().Length(10, 13).Matches(@"^\d{10}(\d{3})?$")
            .WithMessage("ISBN must be 10 or 13 characters and contain only digits");

        RuleFor(x => x.PageCount).GreaterThan(0).LessThanOrEqualTo(10_000)
            .WithMessage("Page count must be between 1 and 10,000");

        RuleFor(x => x.Price).GreaterThan(0).LessThan(10_000)
            .WithMessage("Price must be between 0 and 10,000");

        RuleFor(x => x.PageMaterial).IsInEnum().WithMessage("Page material must be Paperback or Hardcover");

        RuleFor(x => x.PhotoUrl).NotNull()
            .Must(uri =>
                uri == null || (uri.IsAbsoluteUri &&
                                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)))
            .WithMessage("PhotoUrl must be a valid absolute http/https URL");

        RuleFor(x => x.TotalCopies)
            .InclusiveBetween(1, 1000).WithMessage("Total copies must be between 1 and 1000");
    }
}
