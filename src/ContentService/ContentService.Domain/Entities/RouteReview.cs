using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ContentService.Domain.Entities
{
    [Table("route_reviews")]
    public class RouteReview
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("route_id")]
        public Guid RouteId { get; set; }

        [Column("rating")]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Column("comment")]
        [MaxLength(1000)]
        public string? Comment { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        public Route Route { get; set; } = null!;

    }
}
