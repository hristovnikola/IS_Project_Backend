using Domain.Identity;

namespace Service.Interface;

public interface IUserService
{
    string GetMyName();

    IEnumerable<User> GetAll();
}