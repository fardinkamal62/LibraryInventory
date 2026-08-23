using LibraryInventory.Core.Common;

namespace LibraryInventory.Core;

public enum PageTypeEnum
{
    Paperback,
    Hardcover
}


public class Book(string bookName, string author, string isbnNumber, int pageCount, decimal price, PageTypeEnum pageMaterial, Uri photoUrl): BaseEntity
{
    public int BookId { get; init; }

    public string BookName { get; private set; } = bookName;

    public string Author { get; private set; } = author;

    public string IsbnNumber { get; init; } = isbnNumber;

    public int PageCount { get; private set; } = pageCount;

    public decimal Price { get; private set; } = price;

    public PageTypeEnum PageMaterial { get; private set; } = pageMaterial;

    public Uri PhotoUrl { get; private set; } = photoUrl;

    public void UpdateDetails(string IsbnNumber, string bookName, PageTypeEnum pageMaterial, Uri photoUrl, int pageCount)
      {
          if (string.IsNullOrWhiteSpace(isbnNumber))
              throw new ArgumentException("Book's ISBN number required", nameof(bookName));
          BookName = bookName;
          PageMaterial = pageMaterial;
          PhotoUrl = photoUrl;
          PageCount = pageCount;
      }

      public void ChangePrice(decimal newPrice)
      {
          if (newPrice <= 0) throw new ArgumentException("Price must be positive", nameof(newPrice));
          Price = newPrice;
      }
}
