using Application.Users.Dtos;
using FluentAssertions;

namespace Test.Application.Users.Dtos
{
    public class UserFilterTest
    {
        [Fact]
        public void Should_Has_pagination_1_10_When_UserFilter_Has_Null_Properties()
        {
            var createUser = new UserFilter();

            createUser.ToString().Should().Be("pagination:1_10");
        }

        [Fact]
        public void Should_Has_Badboy_25_When_UserFilter_Has_Null_Properties()
        {
            var createUser = new UserFilter() {
                Age = 25,
                LastName = "Badboy"
            };

            createUser.ToString().Should().Be("userfilter:Badboy_25");
        }
    }
}
