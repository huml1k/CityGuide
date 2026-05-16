using ContentService.Application.Features.Routes.Commands.DeleteRoute;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Files.Commands.DeleteAudioFile
{
    public class DeleteAudioFileValidator : AbstractValidator<DeleteAudioFileCommand>
    {
        public DeleteAudioFileValidator()
        {
            RuleFor(x => x.AudioFileId)
                .NotEmpty();
        }
    }
}
