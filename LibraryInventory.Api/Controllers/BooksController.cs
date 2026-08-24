using FluentValidation;
using LibraryInventory.Api.Dtos.Books;
using LibraryInventory.Api.Services;
using LibraryInventory.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LibraryInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController(IBookService bookService) : ControllerBase
{
    /// <summary>GET /api/books — list all books (with pagination).</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookResponseDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var books = await bookService.GetAllAsync(page, pageSize, ct);
        return Ok(books.Select(ToDto));
    }

    /// <summary>GET /api/books/{id} — fetch a single book.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookResponseDto>> GetById(int id, CancellationToken ct)
    {
        var result = await bookService.GetByIdAsync(id, ct);
        return result.IsSuccess
            ? Ok(ToDto(result.Data!))
            : NotFound(new { error = result.Message });
    }

    /// <summary>POST /api/books — create a new book.</summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateBookDto dto,
        [FromServices] IValidator<CreateBookDto> validator,
        CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(dto, ct);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        var result = await bookService.CreateAsync(dto, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.BookId }, ToDto(result.Data))
            : MapFailure(result.Message);
    }

    /// <summary>PUT /api/books/{id} — full update.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateBookDto dto,
        [FromServices] IValidator<UpdateBookDto> validator,
        CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(dto, ct);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        var result = await bookService.UpdateAsync(id, dto, ct);
        return result.IsSuccess
            ? Ok(ToDto(result.Data!))
            : MapFailure(result.Message);
    }

    /// <summary>DELETE /api/books/{id} — remove a book.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await bookService.DeleteAsync(id, ct);
        return result.IsSuccess
            ? NoContent()
            : MapFailure(result.Message);
    }

    // Translates a service failure message to the right HTTP status code.
    private IActionResult MapFailure(string message) => message switch
    {
        var m when m.Contains("not found", StringComparison.OrdinalIgnoreCase) =>
            NotFound(new { error = message }),
        var m when m.Contains("already exists", StringComparison.OrdinalIgnoreCase) ||
                  m.Contains("currently checked out", StringComparison.OrdinalIgnoreCase) =>
            Conflict(new { error = message }),
        _ => BadRequest(new { error = message })
    };

    private static BookResponseDto ToDto(Book b) =>
        new(b.BookId, b.BookName, b.Author, b.IsbnNumber,
            b.PageCount, b.Price, b.PageMaterial, b.PhotoUrl);
}
