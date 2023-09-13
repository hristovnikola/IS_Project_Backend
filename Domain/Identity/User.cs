namespace Domain.Identity;

public class User : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public string RefreshToken { get; set; }
    public DateTime TokenCreated { get; set; }
    public DateTime TokenExpires { get; set; }
    public string Role { get; set; }
    
    public new int Id
    {
        get { return base.Id; }
        set { base.Id = value; }
    }
    
    public ShoppingCart ShoppingCart { get; set; }
    public ICollection<Order> Orders { get; set; }
}