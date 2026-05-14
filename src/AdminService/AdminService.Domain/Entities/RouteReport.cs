using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminService.Domain.Entities
{
    [Table("route_reports")]
    public class RouteReport
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("route_id")]
        public Guid RouteId { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("reason")]
        [MaxLength(255)]
        public string Reason { get; set; } = string.Empty;

        [Column("comment")]
        public string? Comment { get; set; }

        [Column("status")]
        [MaxLength(20)]
        public string Status { get; set; } = "pending"; // pending / reviewed / rejected

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("reviewed_by")]
        public int? ReviewedBy { get; set; }

        [Column("reviewed_at")]
        public DateTime? ReviewedAt { get; set; }
    }
}
