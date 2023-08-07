using Domain.Entities;

namespace Application.Users.Dtos;

public class CreateUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public DateTime Date { get; set; }
    public string Country { get; set; }
    public string Province { get; set; }
    public string City { get; set; }

    public User ToUser()
    {
        return new User
        {
            FirstName = FirstName,
            LastName = LastName,
            City = City,
            Country = Country,
            Date = Date,
            Age = Age,
            Province = Province
        };
    }
}
