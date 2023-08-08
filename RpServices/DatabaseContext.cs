using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RbModels.Entity;

namespace RpServices;

public class DatabaseContext : IdentityDbContext<IdentityUser>
{
    public DbSet<Card> Cards { get; set; }
    public DbSet<UserCards> UserCards { get; set; }
    
    public DatabaseContext(DbContextOptions<DatabaseContext> opt) : base(opt)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Card>()
            .HasIndex(t => t.Digits)
            .IsUnique();

        builder.Entity<UserCards>()
            .HasMany(t => t.Cards)
            .WithOne(t => t.UserCards)
            .HasForeignKey(t => t.UserCardId)
            .IsRequired();

        builder.Entity<UserCards>()
            .HasOne<IdentityUser<Guid>>()
            .WithOne()
            .HasForeignKey<UserCards>(t => t.UserId)
            .IsRequired();
    }
}