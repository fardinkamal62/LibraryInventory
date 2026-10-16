using LibraryInventory.Core.Entities;
namespace LibraryInventory.Core.Commands;
public record CreateBookCommand(string BookName,string Author,string IsbnNumber,int PageCount,decimal Price,PageTypeEnum PageMaterial,Uri PhotoUrl);
public record UpdateBookCommand(string BookName,string Author,int PageCount,decimal Price,PageTypeEnum PageMaterial,Uri PhotoUrl);
public record BookQuery(string? Search,bool? Available,int Page=1,int PageSize=20);
public record BookWithAvailability(Book Book,bool Available);
