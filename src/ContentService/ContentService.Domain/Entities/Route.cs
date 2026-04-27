using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ContentService.Domain.Entities
{
    [Table("routes")]
    public class Route
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("creator_id")]
        public long CreatorId { get; set; }

        [Column("title")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Column("description")]
        [MaxLength(2000)]
        public string? Description { get; set; }

        [Column("duration_minutes")]
        public int DurationMinutes { get; set; }

        [Column("price", TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column("status")]
        [MaxLength(50)]
        public string Status { get; set; } = "draft";   // draft, approved, rejected,

        [Column("google_maps_url")]
        [MaxLength(500)]
        public string? GoogleMapsUrl { get; set; }

        [Column("version")]
        public int Version { get; set; } = 1;

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

    }
}
