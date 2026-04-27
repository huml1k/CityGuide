using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace AdminService.Domain.Entities
{
    [Table("admin_logs")]
    public class AdminLog
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("admin_id")]
        public int AdminId { get; set; }

        [Column("action")]
        [MaxLength(100)]
        public string Action { get; set; } = string.Empty;

        [Column("entity_type")]
        [MaxLength(100)]
        public string? EntityType { get; set; }

        [Column("entity_id")]
        public long? EntityId { get; set; }

        [Column("details")]
        public JsonDocument? Details { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

    }
}
