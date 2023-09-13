using Domain.Identity;

namespace Repository.Interface;

public interface IUserRepository
{
    User GetById(int id);
    User GetByUsername(string username);
    int GetUserIdByUsername(string username);
    User GetByRefreshToken(string refreshToken); // Add this method
    IEnumerable<User> GetAllUsers();
    void Add(User user);
    void Update(User user);
    void SaveChanges();
}