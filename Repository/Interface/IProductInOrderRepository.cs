using Domain.Relations;

namespace Repository.Interface;

public interface IProductInOrderRepository
{
    void Insert(ProductInOrder entity);
}