using Domain.Entities;
using FluentAssertions;

namespace Test.Domain.Entities;

public class UserTest
{
    [Fact]
    public void Should_Create_User_Object_Correctly()
    {
        var user = new User { 
            Id = 1,
            FirstName = "Robert",
            LastName = "Casto",
            City = "London",
            Country = "United Kingdom",
            Date = DateTime.Now,
            Age = 21,
            Province = "London"
        };

        user.Should().NotBeNull();
        user.FirstName.Should().Be("Robert");
    }
}
