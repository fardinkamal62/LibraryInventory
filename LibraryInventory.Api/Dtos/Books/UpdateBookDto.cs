using LibraryInventory.Core.Entities;

namespace LibraryInventory.Api.Dtos.Books;

public record UpdateBookDto(
    string BookName,
    string Author,
    int PageCount,
    decimal Price,
    PageTypeEnum PageMaterial,
    Uri PhotoUrl);
