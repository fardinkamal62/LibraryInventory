using LibraryInventory.Core;

namespace LibraryInventory.Api.Dtos.Books;

/// <summary>
/// Used by PUT /api/books/{id}. All fields required (full replacement).
/// For PATCH use a separate UpdateBookPartialDto with optional fields.
/// </summary>
public record UpdateBookDto(
    string BookName,
    string Author,
    string IsbnNumber,
    int PageCount,
    decimal Price,
    PageTypeEnum PageMaterial,
    Uri PhotoUrl
);