using Microsoft.EntityFrameworkCore;
using CtiHub.Domain.Entities;
using CtiHub.Domain.Common;

namespace CtiHub.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    // Constructor: Ayarları dışarıdan (Program.cs'ten) alır.
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Bu listeler veritabanında birer tabloya dönüşecek.
    public DbSet<User> Users { get; set; }
    public DbSet<Target> Targets { get; set; }
    public DbSet<Scan> Scans { get; set; }

    // SENIOR DOKUNUşU: SaveChangesAsync metodunu eziyoruz (Override).
    // Neden? Her kayıt izleminde CreatedAt/UpdatedAt alanlarını elle girmek yerine
    // burada otomatik dolduruyoruz. Hata yapma riskini sıfırlar.
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