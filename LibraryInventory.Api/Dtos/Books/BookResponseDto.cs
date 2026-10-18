using LibraryInventory.Core.Entities;

namespace LibraryInventory.Api.Dtos.Books;

public record BookResponseDto(
    int BookId,
    string BookName,
    string Author,
    string IsbnNumber,
    int PageCount,
    decimal Price,
    PageTypeEnum PageMaterial,
    Uri PhotoUrl);

public record BookListItemDto(
    int BookId,
    string BookName,
    string Author,
    string IsbnNumber,
    int PageCount,
    decimal Price,
    PageTypeEnum PageMaterial,
    Uri PhotoUrl,
    bool Available);