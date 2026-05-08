using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminService.Domain.Entities
{
    [Table("user_moderation")]
    public class UserModeration
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("action")]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty; // ban / unban / role_change

        [Column("reason")]
        [MaxLength(255)]
        public string? Reason { get; set; }

        [Column("created_by")]
        public int CreatedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
