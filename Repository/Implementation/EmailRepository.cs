using Domain;
using Repository.Interface;

namespace Repository.Implementation;

public class EmailRepository : IEmailRepository
{
    private readonly AppDbContext _context;

    public EmailRepository(AppDbContext context) =>
        _context = context;

    public void Create(Email email)
    {
        _context.Emails.Add(email);
        _context.SaveChanges();
    }
    
}