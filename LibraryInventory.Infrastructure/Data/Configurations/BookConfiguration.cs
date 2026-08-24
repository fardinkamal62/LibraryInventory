using LibraryInventory.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryInventory.Infrastructure.Data.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");
        builder.HasKey(b => b.BookId);
        builder.Property(b => b.BookId).ValueGeneratedOnAdd();

        builder.Property(b => b.BookName).IsRequired().HasMaxLength(255);
        builder.Property(b => b.Author).IsRequired().HasMaxLength(150);
        builder.Property(b => b.IsbnNumber).IsRequired().HasMaxLength(13);
        builder.HasIndex(b => b.IsbnNumber).IsUnique();

        builder.Property(b => b.PageCount).IsRequired();
        builder.Property(b => b.Price).HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(b => b.PageMaterial).HasConversion<string>();
        builder.Property(b => b.PhotoUrl).IsRequired();
    }
}
