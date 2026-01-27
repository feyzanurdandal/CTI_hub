using System.Linq.Expressions;
using CtiHub.Application.Common.Interfaces;
using CtiHub.Domain.Common;
using CtiHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CtiHub.Infrastructure.Repositories;
// Bu sınıf, yukarıdaki interface'in (sözleşmenin) şartlarını yerine getirmek ZORUNDADIR.

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    // Veritabanı bağlantı nesnemiz (Daha önce kurduğumuz DbContext).
    private readonly ApplicationDbContext _context;

    // DbSet: Veritabanındaki spesifik tablo (Örn: Users tablosu).
    private readonly DbSet<T> _dbSet;

    // Constructor: Veritabanı bağlantısını (DbContext) içeri alır. Bu sınıf oluşturulduğunda çalışır.
    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        // _context.Set<T>() diyerek, T neyse (User mı? Target mı?) otomatik olarak o tabloyu seçer.
        _dbSet = _context.Set<T>(); 
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity); // Entity Framework'ün "Ekle" komutu. Henüz veritabanına gitmedi, RAM'de bekliyor.
        await _context.SaveChangesAsync(); // SaveChangesAsync: İşte şimdi "Commit" yapıldı ve veri PostgreSQL'e yazıldı.
    }

    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    // FindAsync: Kullanıcının gönderdiği filtreyi (predicate) alıp SQL'e çevirir ve çalıştırır.
    public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }
}
