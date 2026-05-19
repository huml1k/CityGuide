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

            RuleFor(x => x.File)
                .NotNull()
                .Must(file => 
                {
                    var extension = Path.GetExtension(file.FileName);
                    return new[] { ".mp3", ".ogg", }.Contains(extension.ToLower());
                })
                .WithMessage("Only .mp3 and .ogg files are allowed.");
            RuleFor(x => x.File)
                .NotNull()
                .Must(file => file.Length <= 5 * 1024 * 1024)
                .WithMessage("Must be less than 5MB");
        }
    }
}
