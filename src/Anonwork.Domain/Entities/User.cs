using System;
using System.Collections.Generic;

namespace Anonwork.Domain.Entities;

public partial class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    public string? Bio { get; set; }

    public string AnonAlias { get; set; } = null!;

    public bool IsAnonDefault { get; set; }

    public string Role { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<ConversationMember> ConversationMembers { get; set; } = new List<ConversationMember>();

    public virtual ICollection<Follow> FollowFollowers { get; set; } = new List<Follow>();

    public virtual ICollection<Follow> FollowFollowings { get; set; } = new List<Follow>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Notification> NotificationActors { get; set; } = new List<Notification>();

    public virtual ICollection<Notification> NotificationUsers { get; set; } = new List<Notification>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();


    private User() { }

    public static User Create(
        string username,
        string email,
        string passwordHash,
        string anonAlias)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Username = username.ToLower().Trim(),
            Email = email.ToLower().Trim(),
            PasswordHash = passwordHash,
            AnonAlias = anonAlias,
            IsAnonDefault = false,
            Role = "student",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // Dùng cho login / update profile sau này
    public void UpdateProfile(string? avatarUrl, string? bio)
    {
        AvatarUrl = avatarUrl;
        Bio = bio;
        UpdatedAt = DateTime.UtcNow;
    }
}
