using Domain;

namespace Repository.Interface;

public interface IEmailRepository
{
    void Create(Email email);
}