using ContentService.Application.Features.Files.Commands.DeleteAudioFile;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Files.Commands.DeleteRouteImage
{
    public class DeleteRouteImageValidator : AbstractValidator<DeleteRouteImageCommand>
    {
        public DeleteRouteImageValidator()
        {
            RuleFor(x => x.ImageId)
                .NotEmpty();
        }
    }
}
