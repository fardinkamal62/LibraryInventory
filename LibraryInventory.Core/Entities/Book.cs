using LibraryInventory.Core.Common;

namespace LibraryInventory.Core.Entities;

public enum PageTypeEnum
{
    Paperback,
    Hardcover
}

public class Book : BaseEntity
{
    public string BookName { get; private set; }

    public string Author { get; private set; }

    public string IsbnNumber { get; private set; } = null!;

    public int PageCount { get; private set; }

    public decimal Price { get; private set; }

    public PageTypeEnum PageMaterial { get; private set; }

    public Uri PhotoUrl { get; private set; }

    public int TotalCopies { get; private set; }

    public int AvailableCopies { get; private set; }

    public Book(
        string bookName,
        string author,
        string isbnNumber,
        int pageCount,
        decimal price,
        PageTypeEnum pageMaterial,
        Uri photoUrl,
        int totalCopies)
    {
        if (string.IsNullOrWhiteSpace(bookName))
            throw new ArgumentException("Book's name required", nameof(bookName));

        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Author required", nameof(author));

        if (string.IsNullOrWhiteSpace(isbnNumber))
            throw new ArgumentException("ISBN required", nameof(isbnNumber));

        isbnNumber = isbnNumber.Trim();

        if (pageCount <= 0)
            throw new ArgumentException("Page count must be positive", nameof(pageCount));

        if (price <= 0)
            throw new ArgumentException("Price must be positive", nameof(price));

        if (photoUrl is null)
            throw new ArgumentNullException(nameof(photoUrl), "Photo URL cannot be null");

        if (!photoUrl.IsAbsoluteUri)
            throw new ArgumentException("Photo URL must be an absolute URI", nameof(photoUrl));

        if (totalCopies < 1)
            throw new ArgumentException("Total copies must be at least 1", nameof(totalCopies));

        BookName = bookName;
        Author = author;
        IsbnNumber = isbnNumber;
        PageCount = pageCount;
        Price = price;
        PageMaterial = pageMaterial;
        PhotoUrl = photoUrl;
        TotalCopies = totalCopies;
        AvailableCopies = totalCopies;
    }

    public void UpdateDetails(string bookName, PageTypeEnum pageMaterial, Uri photoUrl, int pageCount, decimal price, int totalCopies, int availableCopies)
    {
        if (string.IsNullOrWhiteSpace(bookName))
            throw new ArgumentException("Book's name required", nameof(bookName));

        if (pageCount <= 0)
            throw new ArgumentException("Page count must be positive", nameof(pageCount));

        if (photoUrl is null)
            throw new ArgumentNullException(nameof(photoUrl), "Photo URL cannot be null");

        if (!photoUrl.IsAbsoluteUri)
            throw new ArgumentException("Photo URL must be an absolute URI", nameof(photoUrl));

        BookName = bookName;
        PageMaterial = pageMaterial;
        PhotoUrl = photoUrl;
        PageCount = pageCount;
        Price = price;
        TotalCopies = totalCopies;
        AvailableCopies = availableCopies;
    }

    public void ChangePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new ArgumentException("Price must be positive", nameof(newPrice));

        Price = newPrice;
    }
}
