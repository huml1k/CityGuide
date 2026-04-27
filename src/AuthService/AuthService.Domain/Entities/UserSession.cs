using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AuthService.Domain.Entities
{
    [Table("user_sessions")]
    public class UserSession
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("user_id")]
        public Guid UserId { get; set; }

        [Required]
        [Column("refresh_token_hash")]
        [MaxLength(255)]
        public string RefreshTokenHash { get; set; }

        [Required]
        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("revoked_at")]
        public DateTime? RevokedAt { get; set; }

        //опционально 

        [MaxLength(255)]
        [Column("user_agent")]
        public string UserAgent { get; set; }

        [MaxLength(45)]
        [Column("ip_address")]
        public string IpAddress { get; set; }

    }
}
