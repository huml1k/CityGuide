using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ContentService.Application.Features.Tags.DTOs
{
    public class TagDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
