using ContentService.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Routes.Commands.CreateRoute
{
    public class CreateRouteCommand : IRequest<CreateRouteResponse> //Хранит входные данные запроса
    {
        public Guid CreatorId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DurationMinutes { get; set; }

        public string? GoogleMapsUrl { get; set; }

        public List<Guid> TagIds { get; set; } = [];
    }
}
