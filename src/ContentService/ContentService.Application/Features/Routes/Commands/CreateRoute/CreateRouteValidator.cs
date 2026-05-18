using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Commands.CreateRoute
{
    public class CreateRouteValidator : AbstractValidator<CreateRouteCommand>
    {
        public CreateRouteValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .MaximumLength(2000);

            RuleFor(x => x.DurationMinutes)
                .GreaterThan(0);

            RuleFor(x => x.CreatorId)
                .NotEmpty();
        }
    }
}
