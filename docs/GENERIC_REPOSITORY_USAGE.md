# GenericRepository Usage Guide

## Overview

The GenericRepository provides a comprehensive set of CRUD operations for any entity in your application. It follows the Repository pattern and integrates seamlessly with Entity Framework Core.

## Features

- **Complete CRUD Operations**: Create, Read, Update, Delete
- **Async/Await Support**: All operations are asynchronous
- **Flexible Querying**: Support for LINQ expressions, includes, pagination
- **Transaction Support**: Begin, commit, rollback transactions
- **Performance Optimized**: Uses AsNoTracking for read operations by default
- **Cancellation Token Support**: All operations support cancellation

## Basic Usage

### 1. Inject the Repository

```csharp
public class SomeService
{
    private readonly IGenericRepository<User> _userRepository;
    
    public SomeService(IGenericRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }
}
```

### 2. Basic CRUD Operations

```csharp
// CREATE
var newUser = new User 
{ 
    Username = "john_doe", 
    Email = "john@example.com" 
};
var createdUser = await _userRepository.AddAsync(newUser);
await _userRepository.SaveChangesAsync();

// READ
var user = await _userRepository.GetByIdAsync(userId);
var allUsers = await _userRepository.GetAllAsync();

// UPDATE
user.Username = "john_updated";
await _userRepository.UpdateAsync(user);
await _userRepository.SaveChangesAsync();

// DELETE
await _userRepository.DeleteAsync(userId);
await _userRepository.SaveChangesAsync();
```

### 3. Advanced Querying

```csharp
// Find users by condition
var activeUsers = await _userRepository.FindAsync(u => u.IsActive);

// Find single user
var user = await _userRepository.FindSingleAsync(u => u.Email == "john@example.com");

// Get with includes (related data)
var usersWithPosts = await _userRepository.GetWithIncludesAsync(u => u.Posts);

// Pagination
var pagedUsers = await _userRepository.GetPagedAsync(page: 1, pageSize: 10);

// Count operations
var totalUsers = await _userRepository.CountAsync();
var activeUsersCount = await _userRepository.CountAsync(u => u.IsActive);

// Existence checks
var userExists = await _userRepository.ExistsAsync(userId);
var emailExists = await _userRepository.ExistsAsync(u => u.Email == "test@example.com");
```

### 4. Complex Queries with IQueryable

```csharp
// Get queryable for complex operations
var query = _userRepository.GetQueryableNoTracking()
    .Where(u => u.CreatedAt > DateTime.UtcNow.AddDays(-30))
    .Include(u => u.Posts)
    .OrderBy(u => u.Username);

var recentUsers = await query.ToListAsync();
```

### 5. Transaction Management

```csharp
try
{
    await _userRepository.BeginTransactionAsync();
    
    // Multiple operations
    await _userRepository.AddAsync(user1);
    await _userRepository.AddAsync(user2);
    await _userRepository.SaveChangesAsync();
    
    await _userRepository.CommitTransactionAsync();
}
catch
{
    await _userRepository.RollbackTransactionAsync();
    throw;
}
```

### 6. Bulk Operations

```csharp
// Add multiple entities
var users = new List<User> { user1, user2, user3 };
await _userRepository.AddRangeAsync(users);
await _userRepository.SaveChangesAsync();

// Update multiple entities
await _userRepository.UpdateRangeAsync(users);
await _userRepository.SaveChangesAsync();

// Delete multiple entities
await _userRepository.DeleteRangeAsync(users);
await _userRepository.SaveChangesAsync();

// Delete by condition
await _userRepository.DeleteWhereAsync(u => u.IsActive == false);
await _userRepository.SaveChangesAsync();
```

## Extending for Specific Entities

### Option 1: Use BaseRepository

```csharp
public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
}

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await FindSingleAsync(u => u.Email == email.ToLower().Trim(), ct);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
    {
        return await ExistsAsync(u => u.Email == email.ToLower().Trim(), ct);
    }
}
```

### Option 2: Composition over Inheritance

```csharp
public class UserService
{
    private readonly IGenericRepository<User> _repository;
    
    public UserService(IGenericRepository<User> repository)
    {
        _repository = repository;
    }
    
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _repository.FindSingleAsync(u => u.Email == email.ToLower().Trim());
    }
}
```

## Best Practices

1. **Always use SaveChangesAsync()** after write operations
2. **Use AsNoTracking** for read-only operations (default in GenericRepository)
3. **Use WithTracking methods** only when you need to update entities
4. **Implement specific repositories** for complex domain logic
5. **Use transactions** for multiple related operations
6. **Handle cancellation tokens** for better performance
7. **Use pagination** for large datasets

## Performance Tips

- The repository uses `AsNoTracking()` by default for better read performance
- Use `GetQueryableNoTracking()` for complex queries
- Use `CountAsync()` instead of loading all entities just to count
- Use `ExistsAsync()` instead of loading entities just to check existence
- Use bulk operations (`AddRangeAsync`, `UpdateRangeAsync`) for multiple entities

## Integration with Existing Code

The GenericRepository is already registered in DI container and can be used alongside your existing specific repositories. You can gradually migrate to use the generic repository or use it for new entities.