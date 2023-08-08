using Api.Controllers;
using Application.Common.Models;
using Application.Users.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Text;

namespace Test.Api.Controllers
{
    public class UsersControllerTest
    {
        [Fact]
        public void Should_Save_A_User_Correctly()
        {
            var user = new User();
            var userCreate = new CreateUser();
            var saveable = new Mock<ISaveable<User>>();
            var quarable = new Mock<IQuerableFilterable<User, UserFilter>>();
            var fileProcessor = new Mock<IFileProcessor>();
            saveable.Setup(a => a.Save(It.IsAny<User>())).Returns(user);
            var usersController = new UsersController(
                saveable.Object, quarable.Object, fileProcessor.Object);

            var result = usersController.Post(userCreate);

            result.Should().NotBeNull();
            saveable.Verify(s => s.Save(It.IsAny<User>()), Times.Once);
        }
        
        [Fact]
        public void Should_Read_User_Info_Correctly()
        {
            var pagination = new Pagination<User>();
            var filter = new UserFilter();
            var saveable = new Mock<ISaveable<User>>();
            var quarable = new Mock<IQuerableFilterable<User, UserFilter>>();
            var fileProcessor = new Mock<IFileProcessor>();
            quarable.Setup(a => a.Get(It.IsAny<UserFilter>())).Returns(pagination);
            var usersController = new UsersController(
                saveable.Object, quarable.Object, fileProcessor.Object);

            var result = usersController.Get(filter);

            result.Should().NotBeNull();
            quarable.Verify(s => s.Get(It.IsAny<UserFilter>()), Times.Once);
        }
        
        [Fact]
        public async Task Should_Bulk_User_Info_CorrectlyAsync()
        {
            var pagination = new Pagination<User>();
            var memoryStream = new MemoryStream();
            var file = new FormFile(memoryStream, 0, 1, "Test", "test.csv")
            {
                Headers = new HeaderDictionary { { "Content-Type", "text/csv" } }
            };
            var saveable = new Mock<ISaveable<User>>();
            var quarable = new Mock<IQuerableFilterable<User, UserFilter>>();
            var fileProcessor = new Mock<IFileProcessor>();
            fileProcessor.Setup(a => a.Process(It.IsAny<Stream>())).Returns(Task.FromResult(true));
            var usersController = new UsersController(
                saveable.Object, quarable.Object, fileProcessor.Object);

            var result = await usersController.BatchUserInfoFromfile(file);

            result.Should().NotBeNull();
            fileProcessor.Verify(s => s.Process(It.IsAny<Stream>()), Times.Once);
        }

        [Fact]
        public async Task Should_Throws_When_File_Is_Not_CSV()
        {
            var pagination = new Pagination<User>();
            var memoryStream = new MemoryStream();
            var file = new FormFile(memoryStream, 0, 1, "Test", "test.dat")
            {
                Headers = new HeaderDictionary { { "Content-Type", "text/dat" } }
            };
            var saveable = new Mock<ISaveable<User>>();
            var quarable = new Mock<IQuerableFilterable<User, UserFilter>>();
            var fileProcessor = new Mock<IFileProcessor>();
            fileProcessor.Setup(a => a.Process(It.IsAny<Stream>())).Returns(Task.FromResult(true));
            var usersController = new UsersController(
                saveable.Object, quarable.Object, fileProcessor.Object);

            var exception = await Assert.ThrowsAsync<AppException>(() => usersController.BatchUserInfoFromfile(file));

            fileProcessor.Verify(s => s.Process(It.IsAny<Stream>()), Times.Never);
            exception.Message.Should().Be("Invalid file type");
        }

        [Fact]
        public async Task Should_Throws_When_File_Length_Is_Zero()
        {
            var pagination = new Pagination<User>();
            var memoryStream = new MemoryStream();
            var file = new FormFile(memoryStream, 0, 0, "Test", "test.csv")
            {
                Headers = new HeaderDictionary { { "Content-Type", "text/csv" } }
            };
            var saveable = new Mock<ISaveable<User>>();
            var quarable = new Mock<IQuerableFilterable<User, UserFilter>>();
            var fileProcessor = new Mock<IFileProcessor>();
            fileProcessor.Setup(a => a.Process(It.IsAny<Stream>())).Returns(Task.FromResult(true));
            var usersController = new UsersController(
                saveable.Object, quarable.Object, fileProcessor.Object);

            var exception = await Assert.ThrowsAsync<AppException>(() => usersController.BatchUserInfoFromfile(file));

            fileProcessor.Verify(s => s.Process(It.IsAny<Stream>()), Times.Never);
            exception.Message.Should().Be("The files is empty");
        }
    }
}
