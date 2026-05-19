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

            RuleFor(x => x.File)
                .NotNull()
                .Must(file => 
                {
                    var extension = Path.GetExtension(file.FileName);
                    return new[] { ".jpg", ".jpeg", ".png" }.Contains(extension.ToLower());
                })
                .WithMessage("Only .jpg, .jpeg, and .png files are allowed.");
            
            RuleFor(x => x.File)
                .NotNull()
                .Must(file => file.Length <= 10 * 1024 * 1024)
                .WithMessage("Must be less than 10MB");
            
            RuleFor(x => x.OrderIndex)
                .GreaterThanOrEqualTo(0);
        }
    }
}
