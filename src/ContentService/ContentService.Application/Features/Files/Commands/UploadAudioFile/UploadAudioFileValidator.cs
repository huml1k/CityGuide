using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Files.Commands.UploadAudioFile
{
    public class UploadAudioFileCommandValidator : AbstractValidator<UploadAudioFileCommand>
    {
        public UploadAudioFileCommandValidator()
        {
            RuleFor(x => x.RouteId)
                .NotEmpty();

            RuleFor(x => x.File)
                .NotNull();

        }
    }
}
