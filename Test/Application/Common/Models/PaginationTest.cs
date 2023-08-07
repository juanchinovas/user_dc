using Application.Common.Models;
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
    public async Task Should_Create_Pagination_Object_CorrectlyAsync()
    {
        var querable = users.AsQueryable();
        int pageNumber = 1;
        int pageSize = 10;

        var result = await Pagination<User>.CreateAsync(querable, pageNumber, pageSize);

        result.Items.Count().Should().Be(10);
        result.PageCount.Should().Be(1);
    }

    [Fact]
    public async Task Should_Create_Pagination_Object_With_Many_Pages_CorrectlyAsync()
    {
        var querable = users.AsQueryable();
        int pageNumber = 1;
        int pageSize = 2;

        var result = await Pagination<User>.CreateAsync(querable, pageNumber, pageSize);

        result.Items.Count().Should().Be(2);
        result.PageCount.Should().Be(5);
        result.TotalItems.Should().Be(10);
    }

    [Fact]
    public async Task Should_Create_Pagination_Object_On_Pages_2_CorrectlyAsync()
    {
        var querable = users.AsQueryable();
        int pageNumber = 2;
        int pageSize = 5;

        var result = await Pagination<User>.CreateAsync(querable, pageNumber, pageSize);

        result.PageCount.Should().Be(2);
        result.PageIndex.Should().Be(2);
    }

    [Fact]
    public void Should_Create_Pagination_Object_Correctly()
    {
        int pageNumber = 1;
        int pageSize = 10;

        var result = Pagination<User>.Create(users, pageNumber, pageSize, users.Count);

        result.Items.Count().Should().Be(10);
        result.PageCount.Should().Be(1);
    }

    [Fact]
    public void Should_Create_Pagination_Object_With_Many_Pages_Correctly()
    {
        int pageNumber = 1;
        int pageSize = 2;

        var result = Pagination<User>.Create(users.Take(pageSize), pageNumber, pageSize, users.Count); ;

        result.Items.Count().Should().Be(2);
        result.PageCount.Should().Be(5);
        result.TotalItems.Should().Be(10);
    }

    [Fact]
    public void Should_Create_Pagination_Object_On_Pages_2_Correctly()
    {
        int pageNumber = 2;
        int pageSize = 5;

        var result = Pagination<User>.Create(users, pageNumber, pageSize, users.Count);

        result.PageCount.Should().Be(2);
        result.PageIndex.Should().Be(2);
    }
}
