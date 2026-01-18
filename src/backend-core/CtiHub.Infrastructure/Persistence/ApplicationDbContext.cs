using Microsoft.EntityFrameworkCore;
using CtiHub.Domain.Entities;
using CtiHub.Domain.Common;

namespace CtiHub.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    // Constructor: Ayarlarý dýþarýdan (Program.cs'ten) alýr.
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Bu listeler veritabanýnda birer tabloya dönüþecek.
    public DbSet<User> Users { get; set; }
    public DbSet<Target> Targets { get; set; }
    public DbSet<Scan> Scans { get; set; }

    // SENIOR DOKUNUÞU: SaveChangesAsync metodunu eziyoruz (Override).
    // Neden? Her kayýt iþleminde CreatedAt/UpdatedAt alanlarýný elle girmek yerine
    // burada otomatik dolduruyoruz. Hata yapma riskini sýfýrlar.
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}