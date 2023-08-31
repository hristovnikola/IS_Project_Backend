using Domain;

namespace Repository.Interface;

public interface IRepository<T> where T : BaseEntity
{
    IEnumerable<T> GetAll();
    T Get(int? id);
    void Insert(T entity);
    bool Update(T entity);
    void Delete(T entity);
    bool ProductExists(int id);
    // void AttachProduct(Product product);
}