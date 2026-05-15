using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ContentService.Domain.Entities
{
    [Table("user_audio_progress")]
    public class UserAudioProgress
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("audio_file_id")]
        public Guid AudioFileId { get; set; }

        [Column("progress_seconds")]
        public int ProgressSeconds { get; set; } = 0;

        [Column("is_completed")]
        public bool IsCompleted { get; set; } = false;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        public AudioFile AudioFile { get; set; } = null!;
    }
}
