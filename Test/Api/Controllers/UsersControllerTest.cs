using Api.Controllers;
using Application.Common.Models;
using Application.Users.Dtos;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Test.Api.Controllers
{
    public class UsersControllerTest
    {
        [Fact]
        public void Should_Save_A_User_Correctly()
        {
            var user = new User();
            var userCreateDto = new Mock<CreateUser>();
            var saveable = new Mock<ISaveable<User>>();
            var quarable = new Mock<IQuerableFiltrable<User, UserFilter>>();
            var fileProcessor = new Mock<IFileProcessor>();
            userCreateDto.Setup(dto => dto.ToUser()).Returns(user);
            saveable.Setup(a => a.Save(It.IsAny<User>())).Returns(user);
            var usersController = new UsersController(
                saveable.Object, quarable.Object, fileProcessor.Object);

            var result = usersController.Post(userCreateDto.Object);

            result.Should().NotBeNull();
        }
    }
}
