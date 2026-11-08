using FluentValidation;
using LibraryInventory.Api.Dtos;
using LibraryInventory.Api.Dtos.Books;
using LibraryInventory.Api.Extensions;
using LibraryInventory.Core.Commands;
using LibraryInventory.Core.Common;
using LibraryInventory.Core.Entities;
using LibraryInventory.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController(IBookService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(string? search, bool? available, int page = 1,
        int pageSize = 20, CancellationToken ct = default)
    {
        var result = await service.GetAllAsync(new BookQuery(search, available, page, pageSize), ct);
        return Ok(new { items = result.Items.Select(ToListDto), result.Page, result.PageSize, result.TotalCount });
    }

    [HttpGet("{id:int}", Name = "GetBook")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await service.GetByIdAsync(id, ct);
        return result.IsSuccess
            ? Ok(ToDetailedDto(result.Data!))
            : this.ToFailure(result.Error, result.Message);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Librarian)]
    public async Task<IActionResult> Create(CreateBookDto dto, IValidator<CreateBookDto> validator,
        CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(dto, ct);
        if (!validation.IsValid) return this.ToValidationProblem(validation);

        var result = await service.CreateAsync(
            new CreateBookCommand(dto.BookName, dto.Author, dto.IsbnNumber, dto.PageCount, dto.Price, dto.PageMaterial,
                dto.PhotoUrl), ct);
        return result.IsSuccess
            ? CreatedAtRoute("GetBook", new { id = result.Data!.Book.BookId }, ToResponseDto(result.Data.Book))
            : this.ToFailure(result.Error, result.Message);
    }

    [HttpPut("{id:int}", Name = "UpdateBook")]
    [Authorize(Roles = Roles.Librarian)]
    public async Task<IActionResult> Update(int id, UpdateBookDto dto, IValidator<UpdateBookDto> validator,
        CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(dto, ct);
        if (!validation.IsValid) return this.ToValidationProblem(validation);

        var result = await service.UpdateAsync(id,
            new UpdateBookCommand(dto.BookName, dto.Author, dto.PageCount, dto.Price, dto.PageMaterial, dto.PhotoUrl),
            ct);
        return result.IsSuccess
            ? Ok(ToResponseDto(result.Data!.Book))
            : this.ToFailure(result.Error, result.Message);
    }

    [HttpDelete("{id:int}", Name = "DeleteBook")]
    [Authorize(Roles = Roles.Librarian)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await service.DeleteAsync(id, ct);
        return result.IsSuccess ? NoContent() : this.ToFailure(result.Error, result.Message);
    }

    private static BookListItemDto ToListDto(BookWithAvailability item)
    {
        return new BookListItemDto(item.Book.BookId, item.Book.BookName, item.Book.Author, item.Book.IsbnNumber,
            item.Book.PageCount, item.Book.Price, item.Book.PageMaterial, item.Book.PhotoUrl, item.Available);
    }

    private DetailedBookDto ToDetailedDto(BookWithAvailability item)
    {
        var id = item.Book.BookId;
        var links = new List<Link>
        {
            new("self", Url.Link("GetBook", new { id })!, "GET"),
            new("update", Url.Link("UpdateBook", new { id })!, "PUT"),
            new("delete", Url.Link("DeleteBook", new { id })!, "DELETE")
        };
        if (item.Available) links.Add(new("checkout", Url.Link("CreateCheckout", null)!, "POST"));

        return new DetailedBookDto(item.Book.BookId, item.Book.BookName, item.Book.Author, item.Book.IsbnNumber,
            item.Book.PageCount, item.Book.Price, item.Book.PageMaterial, item.Book.PhotoUrl, item.Available, links);
    }

    private static BookResponseDto ToResponseDto(Book book)
    {
        return new BookResponseDto(book.BookId, book.BookName, book.Author, book.IsbnNumber, book.PageCount,
            book.Price, book.PageMaterial, book.PhotoUrl);
    }
}