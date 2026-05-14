using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ContentService.Domain.Entities
{
    [Table("tags")]
    public class Tag
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("name")]
        [MaxLength(100)]
        [Required]
        public string Name { get; set; } = string.Empty;

        public ICollection<RouteTag> RouteTags { get; set; }
            = new List<RouteTag>();
    }
}
