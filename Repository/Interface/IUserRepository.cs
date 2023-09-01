using Domain.Identity;

namespace Repository.Interface;

public interface IUserRepository
{
    User GetById(int id);
    User GetByUsername(string username);
    User GetByRefreshToken(string refreshToken); // Add this method

    void Add(User user);
    void Update(User user);
    void SaveChanges();
}