using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ContentService.Domain.Entities
{
    [Table("route_tags")]   
    public class RouteTag
    {
        [Column("route_id")]
        public long RouteId { get; set; }

        [Column("tag_id")]
        public long TagId { get; set; }

    }
}
