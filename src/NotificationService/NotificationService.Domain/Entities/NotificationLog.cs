using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NotificationService.Domain.Entities
{
    [Table("notification_logs")]
    public class NotificationLog
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("notification_id")]
        public Guid NotificationId { get; set; }

        [Required]
        [Column("status")]
        [MaxLength(20)]
        public string Status { get; set; }

        [Required]
        [Column("provider")]
        [MaxLength(50)]
        public string Provider { get; set; }

        [Column("error_message")]
        public string ErrorMessage { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

    }
}
