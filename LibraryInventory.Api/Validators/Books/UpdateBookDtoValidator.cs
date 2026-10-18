using FluentValidation;
using LibraryInventory.Api.Dtos.Books;

namespace LibraryInventory.Api.Validators.Books;

public class UpdateBookDtoValidator : AbstractValidator<UpdateBookDto>
{
    public UpdateBookDtoValidator()
    {
        this.AddDetailsRules(x => x.BookName, x => x.Author, x => x.PageCount,
            x => x.Price, x => x.PageMaterial, x => x.PhotoUrl);
    }
}