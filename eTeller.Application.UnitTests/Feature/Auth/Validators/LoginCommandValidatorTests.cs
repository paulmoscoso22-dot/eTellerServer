using eTeller.Auth.Features.Login;
using System.Linq;

namespace eTeller.Application.UnitTests.Feature.Auth.Validators;

public class LoginCommandValidatorTests
{
    // V-L-01
    [Fact]
    [Trait("Category", "Validator")]
    public void Validate_CommandValido_PassaValidazione()
    {
        var validator = new LoginCommandValidator();
        var command = new LoginCommand("user1", "secret", "ip");

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }

    // V-L-02
    [Fact]
    [Trait("Category", "Validator")]
    public void Validate_UserIdVuoto_FailConMessaggio()
    {
        var validator = new LoginCommandValidator();
        var command = new LoginCommand("", "secret", null);

        var result = validator.Validate(command);
        var messages = result.Errors.Select(e => e.ErrorMessage).ToList();

        Assert.False(result.IsValid);
        Assert.Contains("UserId è obbligatorio.", messages);
    }

    // V-L-03
    [Fact]
    [Trait("Category", "Validator")]
    public void Validate_PasswordVuota_FailConMessaggio()
    {
        var validator = new LoginCommandValidator();
        var command = new LoginCommand("user1", "", null);

        var result = validator.Validate(command);
        var messages = result.Errors.Select(e => e.ErrorMessage).ToList();

        Assert.False(result.IsValid);
        Assert.Contains("La password è obbligatoria.", messages);
    }
}
