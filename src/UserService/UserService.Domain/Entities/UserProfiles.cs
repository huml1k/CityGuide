using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace UserService.Domain.Entities
{
    [Table("user_profiles")]
    public class UserProfile
    {
        [Key]
        [Column("user_id")]
        public long UserId { get; set; }

        [Column("full_name")]
        [MaxLength(150)]
        public string? FullName { get; set; }

        [Column("avatar_url")]
        [MaxLength(500)]
        public string? AvatarUrl { get; set; }

        [Column("bio")]
        [MaxLength(1000)]
        public string? Bio { get; set; }

        [Column("phone")]
        [MaxLength(30)]
        [Phone]
        public string? Phone { get; set; }

    }
}
