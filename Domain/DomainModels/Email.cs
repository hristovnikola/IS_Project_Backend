namespace Domain;

public class Email : BaseEntity
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}