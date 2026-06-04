namespace Anonwork.Domain.Entities;

public partial class Permission
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    private Permission() { }

    public static Permission Create(string code, string? description = null)
    {
        return new Permission
        {
            Id = Guid.NewGuid(),
            Code = code.Trim().ToLowerInvariant(),
            Description = description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
