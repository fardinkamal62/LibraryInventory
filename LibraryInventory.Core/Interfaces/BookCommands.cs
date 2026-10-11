using LibraryInventory.Core.Entities;

namespace LibraryInventory.Core.Interfaces;

/// <summary>
/// Core-friendly request payloads for book operations. Decoupled from Api DTOs
/// so the service contract doesn't depend on the HTTP layer.
/// </summary>
public record CreateBookCommand(
    string BookName,
    string Author,
    string IsbnNumber,
    int PageCount,
    decimal Price,
    PageTypeEnum PageMaterial,
    Uri PhotoUrl,
    int TotalCopies
);

public record UpdateBookCommand(
    string BookName,
    string Author,
    string IsbnNumber,
    int PageCount,
    decimal Price,
    PageTypeEnum PageMaterial,
    Uri PhotoUrl,
    int TotalCopies
);
