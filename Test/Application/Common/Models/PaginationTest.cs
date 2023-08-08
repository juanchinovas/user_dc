using Application.Common.Models;
using Core.Models;
using Domain.Entities;
using FluentAssertions;

namespace Test.Application.Common.Models;

public class PaginationTest
{
    private List<User> users = new List<User>();
    public PaginationTest() { 
        for (int i = 0; i < 10; i++)
        {
            users.Add(new User());
        }
    }

    [Fact]
    public void Should_Create_Pagination_Object_Correctly_From_IQuarable()
    {
        var querable = users.AsQueryable();
        int pageNumber = 1;
        int pageSize = 10;

        var result = Pagination<User>.Create(querable, pageNumber, pageSize);

        result.Items.Count().Should().Be(10);
        result.PageCount.Should().Be(1);
    }

    [Fact]
    public void Should_Create_Pagination_Object_With_Many_Pages_Correctly_From_IQuarable()
    {
        var querable = users.AsQueryable();
        int pageNumber = 1;
        int pageSize = 2;

        var result = Pagination<User>.Create(querable, pageNumber, pageSize);

        result.Items.Count().Should().Be(2);
        result.PageCount.Should().Be(5);
        result.TotalItems.Should().Be(10);
    }

    [Fact]
    public void Should_Create_Pagination_Object_On_Pages_2_Correctly_From_IQuarable()
    {
        var querable = users.AsQueryable();
        int pageNumber = 2;
        int pageSize = 5;

        var result = Pagination<User>.Create(querable, pageNumber, pageSize);

        result.PageCount.Should().Be(2);
        result.PageIndex.Should().Be(2);
    }

    [Fact]
    public void Should_Create_Pagination_Object_Correctly()
    {
        var resume = new DataResum<User>
        {
            Items = users,
            TotalItems = users.Count,
        };
        int pageNumber = 1;
        int pageSize = 10;

        var result = Pagination<User>.Create(resume, pageNumber, pageSize);

        result.Items.Count().Should().Be(10);
        result.PageCount.Should().Be(1);
    }

    [Fact]
    public void Should_Create_Pagination_Object_With_Many_Pages_Correctly()
    {
        var resume = new DataResum<User>
        {
            Items = users,
            TotalItems = users.Count,
        };
        int pageNumber = 2;
        int pageSize = 2;

        var result = Pagination<User>.Create(resume, pageNumber, pageSize);

        result.Items.Count().Should().Be(2);
        result.PageCount.Should().Be(5);
        result.TotalItems.Should().Be(10);
    }

    [Fact]
    public void Should_Create_Pagination_Object_On_Pages_2_Correctly()
    {
        var resume = new DataResum<User>
        {
            Items = users,
            TotalItems = users.Count,
        };
        int pageNumber = 2;
        int pageSize = 5;

        var result = Pagination<User>.Create(resume, pageNumber, pageSize);

        result.PageCount.Should().Be(2);
        result.PageIndex.Should().Be(2);
    }

    [Fact]
    public void When_Data_Is_Already_Filtered_Should_Create_Pagination_Object_Correctly()
    {
        int pageNumber = 2;
        int pageSize = 10;
        var resume = new DataResum<User>
        {
            Items = users,
            TotalItems = 2000,
        };

        var result = Pagination<User>.Create(resume, pageNumber, pageSize);

        result.PageCount.Should().Be(200);
        result.PageIndex.Should().Be(pageNumber);
        result.Items.Count().Should().Be(pageSize);
    }
}
