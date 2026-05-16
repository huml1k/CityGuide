using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Features.Tags.Queries.GetTags
{
    public class GetTagsResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
