using Microsoft.AspNetCore.Http;
using Moq;
using Application;
using FluentAssertions;
using Application.Common.Models;
using Domain.Exceptions;
using System.Text.Json;
using System.Text;

namespace Test.Application;

public class CustomRequestMiddlewareTest
{
    [Fact]
    public void Should_Process_Request_Correctly()
    {
        HttpContext ctx = new DefaultHttpContext();
        RequestDelegate requestDelegate = (HttpContext ctx) => Task.CompletedTask;
        var middleware = new CustomRequestMiddleware(requestDelegate);

        middleware.Invoke(ctx).Should().BeOneOf(Task.CompletedTask);
        ctx.Response.StatusCode.Should().Be(200);
    }

    [Fact]
    public async void Should_Response_With_AppResponse_When_Processing_Request_And_Throw_Exception()
    {
        var exception = new Exception("Oops");
        string? actualValue = null;
        var expectedValue = JsonSerializer.Serialize(
                            AppResponse<AppException>.Fail(
                                new AppException(exception.Message)));

        RequestDelegate requestDelegate = (HttpContext ctx) => Task.FromException(exception);
        var middleware = new CustomRequestMiddleware(requestDelegate);
        var mockedHttpContext = new Mock<HttpContext>();
        mockedHttpContext.SetupProperty(x => x.Response.StatusCode);
        mockedHttpContext.Setup(
            context => context.Response.Body.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())
            )
            .Callback((byte[] data, int offset, int length, CancellationToken token) => {
                if (length > 0)
                {
                    actualValue = Encoding.UTF8.GetString(data);
                }
            })
            .Returns(Task.CompletedTask);

        await middleware.Invoke(mockedHttpContext.Object);

        actualValue.Should().Be(expectedValue);
        mockedHttpContext.Object.Response.StatusCode.Should().Be(409);

    }

    [Fact]
    public async void Should_Response_With_AppResponse_When_Processing_Request_And_Throw_AppException()
    {
        var exception = new AppException("Oops");
        string? actualValue = null;
        var expectedValue = JsonSerializer.Serialize(
                            AppResponse<AppException>.Fail(exception));

        RequestDelegate requestDelegate = (HttpContext ctx) => Task.FromException(exception);
        var middleware = new CustomRequestMiddleware(requestDelegate);
        var mockedHttpContext = new Mock<HttpContext>();
        mockedHttpContext.SetupProperty(x => x.Response.StatusCode);
        mockedHttpContext.Setup(
            context => context.Response.Body.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())
            )
            .Callback((byte[] data, int offset, int length, CancellationToken token) => {
                if (length > 0)
                {
                    actualValue = Encoding.UTF8.GetString(data);
                }
            })
            .Returns(Task.CompletedTask);

        await middleware.Invoke(mockedHttpContext.Object);

        actualValue.Should().Be(expectedValue);
        mockedHttpContext.Object.Response.StatusCode.Should().Be(409);
    }
}
