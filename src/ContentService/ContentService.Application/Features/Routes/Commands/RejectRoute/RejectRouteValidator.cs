using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Commands.RejectRoute
{
    public class RejectRouteValidator : AbstractValidator<RejectRouteCommand>
    {
        public RejectRouteValidator()
        {
            RuleFor(x => x.RouteId)
                .NotEmpty();
        }
    }
}
