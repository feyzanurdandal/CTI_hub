using System.Linq.Expressions;
using CtiHub.Domain.Common;

namespace CtiHub.Application.Common.Interfaces;

// <T>: Buradaki 'T' harfi "Type" (Tür) demektir. Yani bu kalıp jokerdir.
// Yarın buraya 'User' koyarsan User deposu olur, 'Scan' koyarsan Scan deposu olur.
// where T : BaseEntity: Güvenlik önlemi. "Bu T yerine her şeyi koyamazsın, sadece veritabanı tablosu olanları (BaseEntity) koyabilirsin" diyoruz.
public interface IGenericRepository<T> where T : BaseEntity
{
    // Task: İşlemin asenkron (arkada) çalışacağını söyler. API'yi kilitlemez.
    // List<T>: Geriye bir liste dönecek (Örn: Kullanıcılar Listesi).
    Task<List<T>> GetAllAsync(); // Hepsini getir

    // T?: Soru işareti "Null olabilir" demek. Belki o ID'ye sahip kullanıcı yoktur?
    Task<T?> GetByIdAsync(Guid id); // ID'ye göre getir
    
    // Filtreleyerek Getirme (Örn: Email'i 'ali@mail.com' olanları getir)
    // Expression<Func<T, bool>>: Bu, SQL'deki "WHERE" şartıdır. Kod yazarken parantez içine (x => x.Email == "ali@mail.com") yazabilmemizi sağlar.
    Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);

    // Ekleme, Silme, Güncelleme
    // Veri tabanına bir şey eklerken geriye veri dönmeye gerek yok (void yerine Task).
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
