using Domain;
using Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
namespace Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private DbSet<User> entities;

        public UserRepository(AppDbContext context)
        {
            _context = context;
            entities = _context.Set<User>(); 
        }

        public User GetById(int id)
        {
            return entities.FirstOrDefault(u => u.Id == id);
        }

        public User GetByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        public int GetUserIdByUsername(string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user != null)
            {
                return user.Id;
            }

            return -1;
        }

        public User GetByRefreshToken(string refreshToken)
        {
            return _context.Users.FirstOrDefault(u => u.RefreshToken == refreshToken);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return entities.ToList();
        }

        public void Add(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            _context.Users.Add(user);
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
