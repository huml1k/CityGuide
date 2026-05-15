using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Commands.DeleteRoute
{
    public class DeleteRouteValidator : AbstractValidator<DeleteRouteCommand>
    {
        public DeleteRouteValidator()
        {
            RuleFor(x => x.RouteId)
                .NotEmpty();
        }
    }
}
