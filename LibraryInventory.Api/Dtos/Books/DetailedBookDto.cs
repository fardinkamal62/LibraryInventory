using LibraryInventory.Api.Dtos;
using LibraryInventory.Core.Entities;

namespace LibraryInventory.Api.Dtos.Books;

public record DetailedBookDto(
    int BookId,
    string BookName,
    string Author,
    string IsbnNumber,
    int PageCount,
    decimal Price,
    PageTypeEnum PageMaterial,
    Uri PhotoUrl,
    bool Available,
    IReadOnlyList<Link> Links);
