using Service.Interface;
using System.Security.Claims;
using Domain.Identity;
using Microsoft.AspNetCore.Http;
using Repository.Interface;

namespace Service.Implementation;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    
    public UserService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
    }
    
    public string GetMyName()
    {
        var result = string.Empty;

        if (_httpContextAccessor.HttpContext != null)
        {
            result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        }

        return result;
    }

    public IEnumerable<User> GetAll()
    {
        return _userRepository.GetAllUsers();
    }
}