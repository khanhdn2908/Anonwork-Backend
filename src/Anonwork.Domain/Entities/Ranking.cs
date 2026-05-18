using Anonwork.Domain.Common;

namespace Anonwork.Domain.Entities;

public class TrendingTag : BaseEntity
{
    public string Tag { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public int PostCount { get; set; } = 0;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class MonthlyRanking : BaseEntity
{
    public Guid PostId { get; set; }
    public string Period { get; set; } = string.Empty;
    public int RankPosition { get; set; }
    public int LikesCount { get; set; } = 0;
    public int CommentsCount { get; set; } = 0;
    public bool IsFinalized { get; set; } = false;
    public DateTime? FinalizedAt { get; set; }
    public Post Post { get; set; } = null!;
}
