using FluentValidation;
using Pakturaly.Application.Auth.Commands;

namespace Pakturaly.Integration.Tests.Auth.Commands {
    public class LoginCommandTests : BaseIntegrationTest<LoginCommandTests, LoginCommand,
        LoginCommandResult, LoginCommandHandler> {
        [Fact]
        public void LoginCommand_Without_RefreshToken_Runs_Successfully() {
            Arrange(param => {
                param.Email = "";
                param.Password = "";
            })
            .Act()
            .Assert(result => {
                Xunit.Assert.NotNull(result);
                Xunit.Assert.Equal(1, result.Id);
                Xunit.Assert.True(!string.IsNullOrEmpty(result.TenantId));
                Xunit.Assert.Equal("", result.TenantId);
                Xunit.Assert.True(!string.IsNullOrEmpty(result.AccessToken));
                Xunit.Assert.True(!string.IsNullOrEmpty(result.RefreshToken));
                Xunit.Assert.True(result.ExpiresIn > 0);
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
