using FluentValidation;
using smERP.Application.Features.Auth.Commands.Models;
using smERP.Application.Features.Auth.Commands.Results;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Auth.Commands.Validators;

public class RegisterCommandValidator : AbstractValidator<RegisterCommandModel<IResult<RegisterResult>>>
{
    public RegisterCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress()
            .WithMessage(SharedResourcesKeys.___FieldDoesNotMeetCriteria.Localize(SharedResourcesKeys.Email.Localize()));

        RuleFor(c => c.Password)
            .NotNull()
            .NotEmpty()
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$")
            .WithMessage(SharedResourcesKeys.___FieldDoesNotMeetCriteria.Localize(SharedResourcesKeys.Password.Localize()));

        RuleFor(c => c.BranchId)
            .NotNull()
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage(SharedResourcesKeys.___FieldDoesNotMeetCriteria.Localize(SharedResourcesKeys.Branch.Localize()));

        RuleFor(c => c.FirstName)
            .NotNull()
            .NotEmpty()
            .WithMessage(SharedResourcesKeys.___FieldDoesNotMeetCriteria.Localize(SharedResourcesKeys.Name.Localize()));

        RuleFor(c => c.LastName)
            .NotNull()
            .NotEmpty()
            .WithMessage(SharedResourcesKeys.___FieldDoesNotMeetCriteria.Localize(SharedResourcesKeys.Name.Localize()));
    }
}
