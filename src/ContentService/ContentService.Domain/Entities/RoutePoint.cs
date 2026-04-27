using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ContentService.Domain.Entities
{
    [Table("route_points")]
    public class RoutePoint
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("route_id")]
        public long RouteId { get; set; }

        [Column("latitude")]
        public double Latitude { get; set; }

        [Column("longitude")]
        public double Longitude { get; set; }

        [Column("order_index")]
        public int OrderIndex { get; set; }

        [Column("title")]
        [MaxLength(150)]
        public string? Title { get; set; }

        [Column("description")]
        [MaxLength(500)]
        public string? Description { get; set; }

    }
}
