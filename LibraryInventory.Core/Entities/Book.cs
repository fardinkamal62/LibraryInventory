using LibraryInventory.Core.Common;

namespace LibraryInventory.Core.Entities;

public enum PageTypeEnum
{
    Paperback,
    Hardcover
}

public class Book : BaseEntity
{
    private Book()
    {
    }

    public Book(string bookName, string author, string isbnNumber, int pageCount, decimal price,
        PageTypeEnum pageMaterial, Uri photoUrl)
    {
        if (string.IsNullOrWhiteSpace(isbnNumber)) throw new ArgumentException("ISBN required", nameof(isbnNumber));
        IsbnNumber = isbnNumber.Trim();
        UpdateDetails(bookName, author, pageCount, pageMaterial, photoUrl);
        ChangePrice(price);
    }

    public int BookId { get; private set; }
    public string BookName { get; private set; } = null!;
    public string Author { get; private set; } = null!;
    public string IsbnNumber { get; private set; } = null!;
    public int PageCount { get; private set; }
    public decimal Price { get; private set; }
    public PageTypeEnum PageMaterial { get; private set; }
    public Uri PhotoUrl { get; private set; } = null!;

    public void UpdateDetails(string bookName, string author, int pageCount, PageTypeEnum pageMaterial, Uri photoUrl)
    {
        if (string.IsNullOrWhiteSpace(bookName)) throw new ArgumentException("Book name required", nameof(bookName));
        if (string.IsNullOrWhiteSpace(author)) throw new ArgumentException("Author required", nameof(author));
        if (pageCount <= 0) throw new ArgumentException("Page count must be positive", nameof(pageCount));
        ArgumentNullException.ThrowIfNull(photoUrl);
        BookName = bookName.Trim();
        Author = author.Trim();
        PageCount = pageCount;
        PageMaterial = pageMaterial;
        PhotoUrl = photoUrl;
    }

    public void ChangePrice(decimal price)
    {
        if (price <= 0) throw new ArgumentException("Price must be positive", nameof(price));
        Price = price;
    }
}