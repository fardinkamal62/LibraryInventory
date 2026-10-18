using LibraryInventory.Core.Entities;

namespace LibraryInventory.Api.Dtos.Books;

public record CreateBookDto(
    string BookName,
    string Author,
    string IsbnNumber,
    int PageCount,
    decimal Price,
    PageTypeEnum PageMaterial,
    Uri PhotoUrl);