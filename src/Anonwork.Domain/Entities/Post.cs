using System;
using System.Collections.Generic;
using Anonwork.Domain.Enums;
using NpgsqlTypes;

namespace Anonwork.Domain.Entities;

public partial class Post
{
    public Guid Id { get; set; }

    public Guid AuthorId { get; set; }

    public Guid SubjectId { get; set; }

    public bool IsAnonymous { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public NpgsqlTsVector? SearchVector { get; set; }

    public int Upvotes { get; set; }

    public int CommentsCount { get; set; }

    public int ViewCount { get; set; }

    public decimal AverageRating { get; set; }

    public int RatingsCount { get; set; }

    public double QualityScore { get; set; }

    public PostStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual User Author { get; set; } = null!;

    public virtual ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<PostMedia> PostMediaItems { get; set; } = new List<PostMedia>();

    public virtual ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();

    public virtual ICollection<PostRating> PostRatings { get; set; } = new List<PostRating>();

    public virtual Subject Subject { get; set; } = null!;
}
