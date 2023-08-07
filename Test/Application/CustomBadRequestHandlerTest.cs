using Application;
using Application.Common.Models;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace Test.Application;

public class CustomBadRequestHandlerTest
{
    [Fact]
    public void Should_ActionContext_ModelState_Be_Valid()
    {
        var httpContextMock = new Mock<HttpContext>();
        var dict = new ModelStateDictionary();

        var actionContext = new ActionContext(
            httpContextMock.Object,
            new RouteData(),
            new ActionDescriptor(),
            dict);
  

        CustomBadRequestHandler.IsRequestStateInvalid(actionContext).Should().BeTrue();
    }

    [Fact]
    public void Should_ActionContext_ModelState_Be_Invalid()
    {
        var httpContextMock = new Mock<HttpContext>();
        var dict = new ModelStateDictionary();
        dict.AddModelError("Age", "The User is too young");

        var actionContext = new ActionContext(
            httpContextMock.Object,
            new RouteData(),
            new ActionDescriptor(),
            dict);

        CustomBadRequestHandler.IsRequestStateInvalid(actionContext).Should().BeFalse();
    }

    [Fact]
    public void Should_Return_Failed_AppResponse_When_ActionContext_ModelState_Is_Invalid()
    {
        var httpContextMock = new Mock<HttpContext>();
        var dict = new ModelStateDictionary();
        dict.AddModelError("Age", "The User is too young");

        var actionContext = new ActionContext(
            httpContextMock.Object,
            new RouteData(),
            new ActionDescriptor(),
            dict);

        var result = CustomBadRequestHandler.ConstructAppResponse(actionContext);
        
        result.Should().BeAssignableTo<AppResponse<object>>();
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain(error => error == "Age: The User is too young");
    }
}
