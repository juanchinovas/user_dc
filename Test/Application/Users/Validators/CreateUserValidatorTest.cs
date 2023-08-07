using Application.Users.Dtos;
using Application.Users.Validators;
using FluentAssertions;

namespace Test.Application.Users.Validators
{
    public class CreateUserValidatorTest
    {
        [Fact]
        public void Should_Return_Message_user_to_young_When_Age_Is_LessThen_21()
        {
            var createUserDto = new CreateUser
            {
                FirstName = "Raymond",
                LastName = "Carmona",
                City = "Santo Domingo",
                Country = "Dominican Republic",
                Date = DateTime.Now,
                Age = 12,
                Province = "Distrito Nacional"
            };
            var validator = new CreateUserValidator();

            var validationResult = validator.Validate(createUserDto);

            validationResult.Errors.Should().Contain(validation =>
            validation.ErrorMessage == "User too young" && validation.PropertyName == "Age");
        }

        [Fact]
        public void Should_Return_Message_user_to_old_When_Age_Is_GreaterThen_100()
        {
            var createUserDto = new CreateUser
            {
                FirstName = "Raymond",
                LastName = "Carmona",
                City = "Santo Domingo",
                Country = "Dominican Republic",
                Date = DateTime.Now,
                Age = 101,
                Province = "Distrito Nacional"
            };
            var validator = new CreateUserValidator();

            var validationResult = validator.Validate(createUserDto);

            validationResult.Errors.Should().Contain(validation =>
            validation.ErrorMessage == "User too old" && validation.PropertyName == "Age");
        }

        [Fact]
        public void Should_Payload_CreateUserDto_Be_Valid_When_All_Props_Are_Valid()
        {
            var createUserDto = new CreateUser
            {
                FirstName = "Raymond",
                LastName = "Carmona",
                City = "Santo Domingo",
                Country = "Dominican Republic",
                Date = DateTime.Now,
                Age = 100,
                Province = "Distrito Nacional"
            };
            var validator = new CreateUserValidator();

            var validationResult = validator.Validate(createUserDto);

            validationResult.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Should_Payload_CreateUserDto_Be_Invalid_When_All_Props_Are_Invalid()
        {
            var createUserDto = new CreateUser();
            var validator = new CreateUserValidator();

            var validationResult = validator.Validate(createUserDto);

            validationResult.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Should_Return_Messages_prop_is_too_long_When_Prop_Length_is_GreaterThan_30_Charaters()
        {
            var createUserDto = new CreateUser
            {
                FirstName = "Raymond Raymond Raymond Raymond",
                LastName = "Carmona Carmona Carmona Carmona ",
                City = "Santo Domingo Santo Domingo Santo Domingo",
                Country = "Dominican Republic",
                Date = DateTime.Now,
                Age = 99,
                Province = "Distrito Nacional"
            };
            var validator = new CreateUserValidator();

            var validationResult = validator.Validate(createUserDto);

            validationResult.Errors.Should().Contain(validation => validation.ErrorMessage == "The LastName is too long");
        }
    }
}
