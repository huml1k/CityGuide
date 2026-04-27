using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ContentService.Domain.Entities
{
    [Table("route_stats")]
    public class RouteStats
    {
        [Key]
        [Column("route_id")]
        public int RouteId { get; set; }

        [Column("favorites_count")]
        public int FavoritesCount { get; set; }

        [Column("average_rating")]
        public double AverageRating { get; set; }

        [Column("reviews_count")]
        public int ReviewsCount { get; set; }

    }
}
