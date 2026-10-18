using FluentValidation;
using LibraryInventory.Core.Entities;
using System.Linq.Expressions;

namespace LibraryInventory.Api.Validators.Books;

internal static class BookDtoValidationRules
{
    public static void AddDetailsRules<T>(this AbstractValidator<T> validator,
        Expression<Func<T, string>> bookName,
        Expression<Func<T, string>> author,
        Expression<Func<T, int>> pageCount,
        Expression<Func<T, decimal>> price,
        Expression<Func<T, PageTypeEnum>> pageMaterial,
        Expression<Func<T, Uri>> photoUrl)
    {
        validator.RuleFor(bookName).NotEmpty().MaximumLength(255);
        validator.RuleFor(author).NotEmpty().MaximumLength(150);
        validator.RuleFor(pageCount).GreaterThan(0);
        validator.RuleFor(price).GreaterThan(0);
        validator.RuleFor(pageMaterial).IsInEnum();
        validator.RuleFor(photoUrl).Must(IsHttpUrl);
    }

    private static bool IsHttpUrl(Uri? uri) =>
        uri is not null &&
        uri.IsAbsoluteUri &&
        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
