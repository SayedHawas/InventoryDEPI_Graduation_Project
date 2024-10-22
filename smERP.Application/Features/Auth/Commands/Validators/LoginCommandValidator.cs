using FluentValidation;
using smERP.Application.Features.Auth.Commands.Models;
using smERP.Application.Features.Auth.Commands.Results;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Auth.Commands.Validators;

public class LoginCommandValidator : AbstractValidator<LoginCommandModel<IResult<LoginResult>>>
{
    public LoginCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotNull()
            .NotEmpty()
            .WithMessage(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Email.Localize()));

        RuleFor(c => c.Password)
            .NotNull()
            .NotEmpty()
            .WithMessage(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Password.Localize()));
    }
}
