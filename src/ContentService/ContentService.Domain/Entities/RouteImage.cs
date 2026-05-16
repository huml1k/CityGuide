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
        public Guid Id { get; set; }

        [Column("route_id")]
        public Guid RouteId { get; set; }

        
        [Column("file_extension")]
        [MaxLength(5)]
        [Required]
        public required string FileExtension { get; set; }  //Разрешение файла png, jpg и тп

        [Column("is_cover")]
        public bool IsCover { get; set; } = false;

        [Column("order_index")]
        public int OrderIndex { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("deleted_at")]
        public DateTime DeletedAt { get; set; }

        public Route Route { get; set; } = null!;
    }
}
