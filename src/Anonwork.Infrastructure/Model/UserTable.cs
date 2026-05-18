using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anonwork.Infrastructure.Model
{
    [Table("users")]
    public class UserTable : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("username")]
        public string Username { get; set; } = string.Empty;

        [Column("avatar_url")]
        public string? AvatarUrl { get; set; }

        [Column("bio")]
        public string? Bio { get; set; }

        [Column("anon_alias")]
        public string AnonAlias { get; set; } = string.Empty;

        [Column("is_anon_default")]
        public bool IsAnonDefault { get; set; } = false;

        [Column("role")]
        public string Role { get; set; } = "student";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
