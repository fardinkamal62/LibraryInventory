using LibraryInventory.Core;
using Microsoft.EntityFrameworkCore;

namespace LibraryInventory.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();

    public DbSet<Member> Members => Set<Member>();

    public DbSet<Checkout> Checkouts => Set<Checkout>();

    public DbSet<CheckoutItem> CheckoutItems => Set<CheckoutItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---------- Book ----------
        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("Books");
            entity.HasKey(b => b.BookId);
            entity.Property(b => b.BookId).ValueGeneratedOnAdd();

            entity.Property(b => b.BookName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(b => b.Author)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(b => b.IsbnNumber)
                .IsRequired()
                .HasMaxLength(13);

            entity.HasIndex(b => b.IsbnNumber).IsUnique();

            entity.Property(b => b.PageCount).IsRequired();
            entity.Property(b => b.Price).HasColumnType("decimal(10,2)").IsRequired();
            entity.Property(b => b.PageMaterial).HasConversion<string>();
            entity.Property(b => b.PhotoUrl).IsRequired();
        });

        // ---------- Member ----------
        modelBuilder.Entity<Member>(entity =>
        {
            entity.ToTable("Members");
            entity.HasKey(m => m.MemberId);
            entity.Property(m => m.MemberId).ValueGeneratedOnAdd();

            entity.Property(m => m.MemberName)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(m => m.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasIndex(m => m.Email).IsUnique();
        });

        // ---------- Checkout ----------
        modelBuilder.Entity<Checkout>(entity =>
        {
            entity.ToTable("Checkouts");
            entity.HasKey(c => c.CheckOutId);
            entity.Property(c => c.CheckOutId).ValueGeneratedOnAdd();

            entity.Property(c => c.CheckedOutAt).IsRequired();
            entity.Property(c => c.ReturnedAt).IsRequired(false);

            // Checkout -> Member (many-to-one)
            entity.HasOne(c => c.Member)
                .WithMany()
                .HasForeignKey(c => c.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            // Checkout -> CheckoutItems (one-to-many)
            entity.HasMany(c => c.Items)
                .WithOne(i => i.Checkout)
                .HasForeignKey(i => i.CheckoutId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ---------- CheckoutItem (join) ----------
        modelBuilder.Entity<CheckoutItem>(entity =>
        {
            entity.ToTable("CheckoutItems");
            entity.HasKey(i => i.CheckoutItemId);
            entity.Property(i => i.CheckoutItemId).ValueGeneratedOnAdd();

            // CheckoutItem -> Book (many-to-one)
            entity.HasOne(i => i.Book)
                .WithMany()
                .HasForeignKey(i => i.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent duplicate (Checkout, Book) rows
            entity.HasIndex(i => new { i.CheckoutId, i.BookId }).IsUnique();
        });
    }
}