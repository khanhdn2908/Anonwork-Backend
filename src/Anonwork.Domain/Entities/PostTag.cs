using System;
using System.Collections.Generic;

namespace Anonwork.Domain.Entities;

public partial class PostTag
{
    public Guid PostId { get; set; }

    public string Tag { get; set; } = null!;

    public virtual Post Post { get; set; } = null!;
}
