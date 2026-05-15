using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text;

namespace UserService.Domain.Entities
{
    [Table("favorites")]
    public class Favorite
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("route_id")]
        public Guid RouteId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

    }
}
