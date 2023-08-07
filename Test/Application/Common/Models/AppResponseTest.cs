using Application.Common.Models;
using Domain.Exceptions;
using FluentAssertions;

namespace Test.Application.Common.Models;

public class AppResponseTest
{
    [Fact]
    public void Should_Create_Empty_Success_AppResponse_Correctly()
    {
        var appResponse = AppResponse<object>.Success();
        
        appResponse.Should().NotBeNull();
        appResponse.Succeeded.Should().BeTrue();
        appResponse.Message.Should().BeNull();
        appResponse.Data.Should().BeNull();
    }

    [Theory]
    [InlineData("Done")]
    [InlineData(null)]
    public void Should_Success_AppResponse_Has_Null_ErrorList(string? message)
    {
        var appResponse = AppResponse<object>.Success(message);

        appResponse.Errors.Should().BeNull();
    }

    [Fact]
    public void Should_Success_AppResponse_Has_Data_NotNull()
    {
        var appResponse = AppResponse<string>.Success("Done");

        appResponse.Data.Should().Be("Done");
    }

    [Fact]
    public void Should_Success_AppResponse_Has_Data_Message_NotNull()
    {
        var appResponse = AppResponse<string>.Success("Done", "Yey!");

        appResponse.Data.Should().NotBeNull();
        appResponse.Message.Should().NotBeNull();
        appResponse.Message.Should().Be("Yey!");
    }

    [Fact]
    public void Should_Create_Fail_AppResponse_With_ErrorList()
    {
        var errorList = new string[] { "Error 1", "Error 1" };
        var appResponse = AppResponse<string>.Fail(errorList);

        appResponse.Errors.Should().BeEquivalentTo(errorList);
    }

    [Fact]
    public void Should_Create_Fail_AppResponse_With_Message()
    {
        var errorList = new string[] { "Error 1", "Error 1" };
        var appResponse = AppResponse<string>.Fail(errorList, "Message");

        appResponse.Message.Should().Be("Message");
    }

    [Fact]
    public void Should_Create_Fail_AppResponse_From_AppException()
    {
        var exception = new AppException("Exception Message");
        var appResponse = AppResponse<string>.Fail(exception);

        appResponse.Message.Should().Be("Exception Message");
        appResponse.Succeeded.Should().BeFalse();
    }
}
