using Application.Users.Dtos;
using Domain.Entities;
using FluentAssertions;

namespace Test.Application.Users.Dtos
{
    public class CreateUserTest
    {
        [Fact]
        public void Should_Create_CreateUserDto_Object_Correctly()
        {
            var createUser = new CreateUser
            {
                FirstName = "Robert",
                LastName = "Casto",
                City = "London",
                Country = "United Kingdom",
                Date = DateTime.Now,
                Age = 21,
                Province = "London"
            };

            createUser.Should().NotBeNull();
            createUser.FirstName.Should().Be("Robert");
        }

        [Fact]
        public void Should_Map_CreateUserDto_To_User_Correctly()
        {
            var createUser = new CreateUser
            {
                FirstName = "Robert",
                LastName = "Casto",
                City = "London",
                Country = "United Kingdom",
                Date = DateTime.Now,
                Age = 21,
                Province = "London"
            };

            createUser.ToUser().Should().BeAssignableTo<User>();
        }
    }
}
