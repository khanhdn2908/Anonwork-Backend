using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anonwork.Domain.Entities
{
    public class PostImage
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Post Post { get; set; } = null!;
    }
}
