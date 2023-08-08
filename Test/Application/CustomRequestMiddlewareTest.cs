using Microsoft.AspNetCore.Http;
using Moq;
using Application;
using FluentAssertions;
using Application.Common.Models;
using Domain.Exceptions;
using System.Text.Json;
using System.Text;
using Azure;

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
        var expectedValue = JsonSerializer.Serialize(
                            AppResponse<AppException>.Fail(
                                new AppException(exception.Message)));

        RequestDelegate requestDelegate = (HttpContext ctx) => Task.FromException(exception);
        var middleware = new CustomRequestMiddleware(requestDelegate);
        var mockedHttpContext = new DefaultHttpContext();
        mockedHttpContext.Response.Body = new MemoryStream();
        mockedHttpContext.Response.Body.Seek(0, SeekOrigin.Begin);

        await middleware.Invoke(mockedHttpContext);

        mockedHttpContext.Response.StatusCode.Should().Be(409);
        mockedHttpContext.Response.Body.Length.Should().Be(expectedValue.Length);
    }

    [Fact]
    public async void Should_Response_With_AppResponse_When_Processing_Request_And_Throw_AppException()
    {
        var exception = new AppException("Oops");
        var expectedValue = JsonSerializer.Serialize(
                            AppResponse<AppException>.Fail(exception));

        RequestDelegate requestDelegate = (HttpContext ctx) => Task.FromException(exception);
        var middleware = new CustomRequestMiddleware(requestDelegate);
        var mockedHttpContext = new DefaultHttpContext();
        mockedHttpContext.Response.Body = new MemoryStream();
        mockedHttpContext.Response.Body.Seek(0, SeekOrigin.Begin);

        await middleware.Invoke(mockedHttpContext);

        mockedHttpContext.Response.StatusCode.Should().Be(409);
        mockedHttpContext.Response.Body.Length.Should().Be(expectedValue.Length);
    }
}
