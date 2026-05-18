using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Commands.ApproveRoute
{
    public class ApproveRouteValidator : AbstractValidator<ApproveRouteCommand>
    {
        public ApproveRouteValidator()
        {
            RuleFor(x => x.RouteId)
                .NotEmpty();
        }
    }
}
