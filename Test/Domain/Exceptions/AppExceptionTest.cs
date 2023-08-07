using Domain.Exceptions;
using FluentAssertions;

namespace Test.Domain.Exceptions;

public class AppExceptionTest
{
    [Fact]
    public void Should_Create_AppException_Correctly()
    {
        var appException = new AppException("Oops");

        appException.Should().NotBeNull();
    }

    [Fact]
    public void Should_Create_AppException_With_Empty_Message()
    {
        var appException = new AppException(string.Empty);

        appException.Message.Should().Be(string.Empty);
    }

    [Fact]
    public void Should_Create_AppException_With_NotEmpty_Message()
    {
        var appException = new AppException("Oops");

        appException.Message.Should().Be("Oops");
    }

    [Fact]
    public void Should_Create_AppException_With_Null_Errors_List()
    {
        var appException = new AppException("Oops");

        appException.Errors.Should().BeNull();
    }

    [Fact]
    public void Should_Create_AppException_With_Errors_List()
    {
        var errorList = new string[2] { "Oop", "Oop" };
        var appException = new AppException("Oops", errorList);

        appException.Errors.Should().BeSameAs(errorList);
    }
}
