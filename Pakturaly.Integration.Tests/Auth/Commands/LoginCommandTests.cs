using FluentAssertions;
using FluentValidation;
using Pakturaly.Application.Auth.Commands;
using Pakturaly.Integration.Tests.Abstractions;

namespace Pakturaly.Integration.Tests.Auth.Commands {
    public class LoginCommandTests : BaseIntegrationTest<LoginCommandTests, LoginCommand,
        LoginCommandResult, LoginCommandHandler> {
        [Fact]
        public async Task LoginCommand_Without_RefreshToken_Runs_Successfully() {
            Arrange(param => {
                param.Email = "";
                param.Password = "";
            })
            .Act()
            .Assert(result => {
                result.Should()
                    .NotBeNull();

                result.Id
                    .Should()
                    .Be(1);

                result.TenantId
                    .Should()
                    .NotBeNullOrEmpty()
                    .And
                    .BeEquivalentTo("");

                result.AccessToken
                    .Should()
                    .NotBeNullOrEmpty();

                result.RefreshToken
                    .Should()
                    .NotBeNullOrEmpty();

                result.ExpiresIn
                    .Should()
                    .BeGreaterThan(0)
                    .And
                    .Be(3600);
            });
        }

        [Fact]
        public void LoginCommand_With_RefreshToken_Runs_Successfully() {
            Arrange(param => {
                param.Email = "";
                param.Password = "";
                param.RefreshToken = "";
            })
            .Act()
            .Assert(result => {
                Xunit.Assert.NotNull(result);
            });
        }

        [Fact]
        public void LoginComand_Email_Validation_Throws_NotEmpty_Error_Message() {
            Arrange(param => {
                param.Email = string.Empty;
                param.Password = "";
            })
            .Act()
            .AssertThrows<ValidationException>(ex => {
                Xunit.Assert.NotEmpty(ex.Errors);
                Xunit.Assert.Contains("should not be empty.", ex.Message);
            });
        }

        [Fact]
        public void LoginComand_Email_Validation_Throws_InvalidFormat_Error_Message() {
            Arrange(param => {
                param.Email = "";
                param.Password = "";
            })
            .Act()
            .AssertThrows<ValidationException>(ex => {
                Xunit.Assert.NotEmpty(ex.Errors);
                Xunit.Assert.Contains("is not a valid email address.", ex.Message);
            });
        }

        [Fact]
        public void LoginComand_Email_Validation_Throws_MaxCharactersLimit_Error_Message() {
            Arrange(param => {
                param.Email = "";
                param.Password = "";
            })
            .Act()
            .AssertThrows<ValidationException>(ex => {
                Xunit.Assert.NotEmpty(ex.Errors);
                Xunit.Assert.Contains("has a maximum characters limit of 64 only.", ex.Message);
            });
        }

        [Fact]
        public void LoginCommand_Password_Validation_Throws_NotEmpty_Error_Message() {
            Arrange(param => {
                param.Email = "";
                param.Password = string.Empty;
            })
            .Act()
            .AssertThrows<ValidationException>(ex => {
                Xunit.Assert.NotEmpty(ex.Errors);
                Xunit.Assert.Contains("should not be empty.", ex.Message);
            });
        }

        [Fact]
        public void LoginCommand_Throws_UnauthorizedAccess_When_User_Not_Found() {
            Arrange(param => {
                param.Email = "";
                param.Password = string.Empty;
            })
            .Act()
            .AssertThrows<UnauthorizedAccessException>(ex => {
                //Xunit.Assert.NotEmpty(ex);
                //Xunit.Assert.Contains("should not be empty.", ex.Message);
            });
        }
    }
}
