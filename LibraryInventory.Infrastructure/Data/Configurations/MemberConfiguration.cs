using LibraryInventory.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryInventory.Infrastructure.Data.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> b)
    {
        b.ToTable("Members");
        b.HasKey(x => x.MemberId);
        b.Property(x => x.MemberName).IsRequired().HasMaxLength(255);
        b.Property(x => x.Email).IsRequired().HasMaxLength(150);
        b.HasIndex(x => x.Email).IsUnique();
    }
}