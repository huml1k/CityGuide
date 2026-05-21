using ContentService.Application.Common.Exceptions;
using ContentService.Domain.Entities;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;

namespace ContentService.Application.Features.Routes.Commands.CreateRoute
{
    public class CreateRouteHandler : IRequestHandler<CreateRouteCommand, CreateRouteResponse>
    {
        private readonly IRouteRepository _routeRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateRouteHandler(IRouteRepository routeRepository, ITagRepository tagRepository, IUnitOfWork unitOfWork)
        {
            _routeRepository = routeRepository;
            _tagRepository = tagRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateRouteResponse> Handle(CreateRouteCommand request, CancellationToken cancellationToken)
        {
            if (request.CreatorId == Guid.Empty)
            {
                throw new ValidationException(new Dictionary<string, string[]>
            {
                {
                    nameof(request.CreatorId),
                    new[] { "CreatorId is required." }
                }
            });
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ValidationException(new Dictionary<string, string[]>
            {
                {
                    nameof(request.Title),
                    new[] { "Title is required." }
                }
            });
            }

            if (request.DurationMinutes <= 0)
            {
                throw new ValidationException(new Dictionary<string, string[]>
            {
                {
                    nameof(request.DurationMinutes),
                    new[] { "DurationMinutes must be greater than zero." }
                }
            });
            }

            var tags = await _tagRepository.GetByIdsAsync(request.TagIds, cancellationToken);

            if (tags.Count != request.TagIds.Count)
            {
                throw new ValidationException(new Dictionary<string, string[]>
            {
                {
                    nameof(request.TagIds),
                    new[] { "One or more tags do not exist." }
                }
            });
            }

            var routeId = Guid.NewGuid();

            var route = new Route
            {
                Id = routeId,
                CreatorId = request.CreatorId,
                Title = request.Title,
                Description = request.Description,
                DurationMinutes = request.DurationMinutes,
                GoogleMapsUrl = request.GoogleMapsUrl,
                CreatedAt = DateTime.UtcNow,
                RouteStats = new Domain.Entities.RouteStats
                {
                    RouteId = routeId,
                    FavoritesCount = 0
                },

                RouteTags = request.TagIds
                .Select(tagId => new RouteTag
                {
                    RouteId = routeId,
                    TagId = tagId
                })
                .ToList()
            };

            try
            {
                await _routeRepository.AddAsync(
                    route,
                    cancellationToken);

                var saved = await _unitOfWork.SaveChangesAsync(
                    cancellationToken);

                if (saved <= 0)
                {
                    throw new BusinessRuleException(
                        "Failed to create route.");
                }
            }
            catch (BusinessRuleException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException(
                    "An error occurred while creating route.",
                    ex);
            }

            return new CreateRouteResponse
            {
                Id = route.Id,
                Title = route.Title,
                CreatedAt = route.CreatedAt
            };
        }
    }
}
