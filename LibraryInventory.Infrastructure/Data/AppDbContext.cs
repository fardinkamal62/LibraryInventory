using LibraryInventory.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryInventory.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Checkout> Checkouts => Set<Checkout>();
    public DbSet<CheckoutItem> CheckoutItems => Set<CheckoutItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
