using LibraryInventory.Core.Entities;

namespace LibraryInventory.Api.Dtos.Books;

/// <summary>
/// Used by POST /api/books. Client cannot set BookId (server generates it).
/// </summary>
public record CreateBookDto(
    string BookName,
    string Author,
    string IsbnNumber,
    int PageCount,
    decimal Price,
    PageTypeEnum PageMaterial,
    Uri PhotoUrl,
    int TotalCopies
);
