using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace ContentService.Application.Features.Files.Commands.GenerateUploadUrl
{
    public class GenerateUploadUrlValidator : AbstractValidator<GenerateUploadUrlCommand>
    {
        public GenerateUploadUrlValidator()
        {
            
        }
    }
}
