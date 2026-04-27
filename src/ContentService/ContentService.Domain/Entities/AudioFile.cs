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
        public long Id { get; set; }

        [Column("route_id")]
        public long RouteId { get; set; }

        [Column("file_key")]
        [MaxLength(500)]
        [Required]
        public string FileKey { get; set; } = string.Empty;  //путь в MinIO

        [Column("duration_seconds")]
        public int? DurationSeconds { get; set; }

        [Column("order_index")]
        public int OrderIndex { get; set; }

        [Column("original_filename")]
        [MaxLength(255)]
        public string? OriginalFilename { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
