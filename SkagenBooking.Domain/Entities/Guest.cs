namespace SkagenBooking.Core.Entities;

public class Guest
{
    public int Id { get; private set; }
    public string FullName { get; private set; }
    public string Email { get; private set; }

    public Guest(int id, string fullName, string email)
    {
        Id = id;
        FullName = fullName;
        Email = email;
    }
}
