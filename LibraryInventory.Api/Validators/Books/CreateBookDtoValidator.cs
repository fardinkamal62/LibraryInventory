using FluentValidation;
using LibraryInventory.Api.Dtos.Books;

namespace LibraryInventory.Api.Validators.Books;

public class CreateBookDtoValidator : AbstractValidator<CreateBookDto>
{
    public CreateBookDtoValidator()
    {
        this.AddDetailsRules(x => x.BookName, x => x.Author, x => x.PageCount,
            x => x.Price, x => x.PageMaterial, x => x.PhotoUrl);

        RuleFor(x => x.IsbnNumber)
            .Matches(@"^(\d{9}[\dX]|\d{13})$")
            .WithMessage("ISBN must be 10 or 13 characters");
    }
}
