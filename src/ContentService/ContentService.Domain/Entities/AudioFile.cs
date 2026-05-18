using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ContentService.Domain.Entities
{
    [Table("audio_files")]
    public class AudioFile
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("route_id")]
        public Guid RouteId { get; set; }

        [Column("path")]
        [Required]
        public string Path { get; set; }
        
        [Column("file_extension")]
        [MaxLength(5)]
        [Required]
        public required string FileExtension { get; set; }  //Разрешение файла ogg, mp3 и тп
        
        [Column("duration_seconds")]
        public int? DurationSeconds { get; set; }

        [Column("order_index")]
        public int OrderIndex { get; set; }

        [Column("original_filename")]
        [MaxLength(255)]
        public string? OriginalFilename { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("deleted_at")]
        public DateTime DeletedAt { get; set; }

        public Route Route { get; set; } = null!;

    }
}
