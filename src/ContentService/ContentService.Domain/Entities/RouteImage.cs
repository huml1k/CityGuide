using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ContentService.Domain.Entities
{
    [Table("route_images")]
    public class RouteImage
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("route_id")]
        public long RouteId { get; set; }

        [Column("file_key")]
        [MaxLength(500)]
        [Required]
        public string FileKey { get; set; } = string.Empty;

        [Column("is_cover")]
        public bool IsCover { get; set; } = false;

        [Column("order_index")]
        public int OrderIndex { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
