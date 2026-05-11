using eTeller.Auth.Features.ChangePassword;
using System.Linq;

namespace eTeller.Application.UnitTests.Feature.Auth.Validators;

public class ChangePasswordCommandValidatorTests
{
    // V-CP-01
    [Fact]
    [Trait("Category", "Validator")]
    public void Validate_CommandValido_PassaValidazione()
    {
        var validator = new ChangePasswordCommandValidator();
        var command = new ChangePasswordCommand("user1", "OldPass1!", "NewPass2@", "ip");

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }

    // V-CP-02
    [Fact]
    [Trait("Category", "Validator")]
    public void Validate_NuovaPasswordTroppoCorta_FailConMessaggio()
    {
        var validator = new ChangePasswordCommandValidator();
        var command = new ChangePasswordCommand("user1", "OldPass1!", "Ab1!", "ip");

        var result = validator.Validate(command);
        var messages = result.Errors.Select(e => e.ErrorMessage).ToList();

        Assert.False(result.IsValid);
        Assert.Contains(messages, m => m.Contains("almeno 8 caratteri"));
    }

    // V-CP-03
    [Fact]
    [Trait("Category", "Validator")]
    public void Validate_NuovaPasswordSenzaMaiuscola_FailConMessaggio()
    {
        var validator = new ChangePasswordCommandValidator();
        var command = new ChangePasswordCommand("user1", "OldPass1!", "newpass1!", "ip");

        var result = validator.Validate(command);
        var messages = result.Errors.Select(e => e.ErrorMessage).ToList();

        Assert.False(result.IsValid);
        Assert.Contains(messages, m => m.Contains("lettera maiuscola"));
    }

    // V-CP-04
    [Fact]
    [Trait("Category", "Validator")]
    public void Validate_NuovaPasswordSenzaCifra_FailConMessaggio()
    {
        var validator = new ChangePasswordCommandValidator();
        var command = new ChangePasswordCommand("user1", "OldPass1!", "NewPass!!", "ip");

        var result = validator.Validate(command);
        var messages = result.Errors.Select(e => e.ErrorMessage).ToList();

        Assert.False(result.IsValid);
        Assert.Contains(messages, m => m.Contains("almeno una cifra"));
    }

    // V-CP-05
    [Fact]
    [Trait("Category", "Validator")]
    public void Validate_NuovaPasswordUgualeCorrente_FailConMessaggio()
    {
        var validator = new ChangePasswordCommandValidator();
        var command = new ChangePasswordCommand("user1", "SamePass1!", "SamePass1!", "ip");

        var result = validator.Validate(command);
        var messages = result.Errors.Select(e => e.ErrorMessage).ToList();

        Assert.False(result.IsValid);
        Assert.Contains(messages, m => m.Contains("diversa da quella corrente"));
    }
}
