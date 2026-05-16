using ContentService.Application.Features.Routes.Queries.GetRoutes;
using ContentService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Tags.Queries.GetTags
{
    public class GetTagsHandler : IRequestHandler<GetTagsQuery, IReadOnlyCollection<GetTagsResponse>>
    {
        private readonly ITagRepository _tagRepository;
        public GetTagsHandler(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }
        public async Task<IReadOnlyCollection<GetTagsResponse>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
        {
            var tags = await _tagRepository.GetAllAsync(cancellationToken);

            return tags
                .Select(tag => new GetTagsResponse
                {
                    Id = tag.Id,
                    Name = tag.Name
                })
                .ToList();
        }
    }
}
