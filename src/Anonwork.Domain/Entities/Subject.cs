using Anonwork.Domain.Common;

namespace Anonwork.Domain.Entities;

public class Subject : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? IconEmoji { get; set; }
    public int PostCount { get; set; } = 0;

    public ICollection<Post> Posts { get; set; } = new List<Post>();
}
