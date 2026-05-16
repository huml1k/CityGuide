using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Files.Queries.GetRouteImage
{
    public class GetRouteImageHandler : IRequestHandler< GetRouteImageQuery, GetRouteImageResponse>
    {
        private readonly IRouteImageRepository _imageRepository;

        public GetRouteImageHandler(IRouteImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        public async Task<GetRouteImageResponse> Handle(GetRouteImageQuery request, CancellationToken cancellationToken)
        {
            var image = await _imageRepository.GetByIdAsync(request.ImageId, cancellationToken);

            if (image is null)
            {
                throw new Exception("Image not found");
            }

            return new GetRouteImageResponse
            {
                Stream = new MemoryStream(),

                ContentType = $"image/{image.FileExtension}",

                FileName =
                    $"{image.Id}.{image.FileExtension}"
            };
        }
    }
}
