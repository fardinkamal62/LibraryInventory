using LibraryInventory.Core.Entities;

namespace LibraryInventory.Api.Dtos.Books;

/// <summary>
/// Read-only DTO sent back to API consumers. Never accept this as input.
/// </summary>
public record BookResponseDto(
    int BookId,
    string BookName,
    string Author,
    string IsbnNumber,
    int PageCount,
    decimal Price,
    PageTypeEnum PageMaterial,
    Uri PhotoUrl
);
