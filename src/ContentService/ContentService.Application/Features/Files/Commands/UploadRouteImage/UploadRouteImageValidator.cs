using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Files.Commands.UploadRouteImage
{
    public class UploadRouteImageValidator : AbstractValidator<UploadRouteImageCommand>
    {
        public UploadRouteImageValidator()
        {
            RuleFor(x => x.RouteId)
                .NotEmpty();

            RuleFor(x => x.File)
                .NotNull();

            RuleFor(x => x.OrderIndex)
                .GreaterThanOrEqualTo(0);
        }
    }
}
