# Follow Controller API Documentation

## Overview
The Follow Controller manages user follow relationships. Users can follow other users, unfollow them, and view follow statistics.

## Recent Updates

### Phase 1: FollowUserUseCase Fixes
1. **Missing User Data Loading**: After creating a follow relationship, the use case now loads the complete follow data with user information using `GetByIdAsync()` to ensure `Follower` and `Following` properties are populated
2. **Improved Method Usage**: Changed from `IsFollowingAsync()` to `ExistsByFollowerAndFollowingAsync()` for consistency with repository interface
3. **Better Error Handling**: Added validation to ensure the created follow relationship is successfully retrieved before mapping to DTO

### Phase 2: Complete Use Cases & Controller Implementation
Created comprehensive use cases and FollowController with full API implementation:

**Use Cases Created:**
- `FollowUserUseCase` - Follow a user
- `UnfollowUserUseCase` - Unfollow a user
- `GetFollowByIdUseCase` - Get follow relationship by ID
- `GetFollowersUseCase` - Get followers with pagination
- `GetFollowingUseCase` - Get following list with pagination
- `GetFollowStatsUseCase` - Get follow statistics
- `IsFollowingUseCase` - Check if following

**DTOs Created:**
- `FollowResponseDto` - Single follow relationship response
- `PaginatedFollowResponseDto` - Paginated follow relationships
- `FollowStatsDto` - Follow statistics
- `UserBasicDto` - Basic user information
- `FollowUserRequest` - Follow request

**FollowController Endpoints:**
- All 7 endpoints fully implemented with proper error handling and authorization

## Endpoints

### 1. Follow a User
**POST** `/api/follow/follow`

Follow another user.

**Request Body:**
```json
{
  "followingId": "550e8400-e29b-41d4-a716-446655440000"
}
```

**Response (201 Created):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440001",
  "followerId": "550e8400-e29b-41d4-a716-446655440002",
  "followingId": "550e8400-e29b-41d4-a716-446655440000",
  "createdAt": "2024-01-15T10:30:00Z",
  "follower": {
    "id": "550e8400-e29b-41d4-a716-446655440002",
    "username": "john_doe",
    "email": "john@example.com",
    "avatar": "https://example.com/avatar.jpg"
  },
  "following": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "username": "jane_smith",
    "email": "jane@example.com",
    "avatar": "https://example.com/avatar2.jpg"
  }
}
```

**Error Responses:**
- `400 Bad Request`: Invalid input or already following
- `404 Not Found`: User to follow not found
- `401 Unauthorized`: User not authenticated

---

### 2. Unfollow a User
**DELETE** `/api/follow/unfollow/{followingId}`

Unfollow a user.

**Parameters:**
- `followingId` (path, required): ID of the user to unfollow

**Response (204 No Content):**
No response body

**Error Responses:**
- `404 Not Found`: Follow relationship not found
- `401 Unauthorized`: User not authenticated

---

### 3. Get Follow Relationship by ID
**GET** `/api/follow/{id}`

Get details of a specific follow relationship.

**Parameters:**
- `id` (path, required): Follow relationship ID

**Response (200 OK):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440001",
  "followerId": "550e8400-e29b-41d4-a716-446655440002",
  "followingId": "550e8400-e29b-41d4-a716-446655440000",
  "createdAt": "2024-01-15T10:30:00Z",
  "follower": {
    "id": "550e8400-e29b-41d4-a716-446655440002",
    "username": "john_doe",
    "email": "john@example.com",
    "avatar": "https://example.com/avatar.jpg"
  },
  "following": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "username": "jane_smith",
    "email": "jane@example.com",
    "avatar": "https://example.com/avatar2.jpg"
  }
}
```

**Error Responses:**
- `404 Not Found`: Follow relationship not found

---

### 4. Get Followers of a User
**GET** `/api/follow/followers/{userId}`

Get all followers of a user with pagination.

**Parameters:**
- `userId` (path, required): User ID
- `page` (query, optional): Page number (default: 1)
- `pageSize` (query, optional): Page size (default: 10, max: 100)

**Response (200 OK):**
```json
{
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440001",
      "followerId": "550e8400-e29b-41d4-a716-446655440002",
      "followingId": "550e8400-e29b-41d4-a716-446655440000",
      "createdAt": "2024-01-15T10:30:00Z",
      "follower": {
        "id": "550e8400-e29b-41d4-a716-446655440002",
        "username": "john_doe",
        "email": "john@example.com",
        "avatar": "https://example.com/avatar.jpg"
      },
      "following": {
        "id": "550e8400-e29b-41d4-a716-446655440000",
        "username": "jane_smith",
        "email": "jane@example.com",
        "avatar": "https://example.com/avatar2.jpg"
      }
    }
  ],
  "page": 1,
  "pageSize": 10,
  "total": 25,
  "totalPages": 3
}
```

---

### 5. Get Following List
**GET** `/api/follow/following/{userId}`

Get all users that a user is following with pagination.

**Parameters:**
- `userId` (path, required): User ID
- `page` (query, optional): Page number (default: 1)
- `pageSize` (query, optional): Page size (default: 10, max: 100)

**Response (200 OK):**
Same structure as Get Followers endpoint

---

### 6. Get Follow Statistics
**GET** `/api/follow/stats/{userId}`

Get follow statistics for a user.

**Parameters:**
- `userId` (path, required): User ID

**Response (200 OK):**
```json
{
  "followerCount": 150,
  "followingCount": 75,
  "isFollowing": true
}
```

---

### 7. Check if Following
**GET** `/api/follow/is-following/{followingId}`

Check if the current user is following a specific user.

**Parameters:**
- `followingId` (path, required): User ID to check

**Response (200 OK):**
```json
true
```

**Error Responses:**
- `401 Unauthorized`: User not authenticated

---

## Authentication
- Most endpoints require authentication (Bearer token in Authorization header)
- Endpoints marked as `[AllowAnonymous]` can be accessed without authentication:
  - GET `/api/follow/{id}`
  - GET `/api/follow/followers/{userId}`
  - GET `/api/follow/following/{userId}`
  - GET `/api/follow/stats/{userId}`

## Error Handling

### Common Error Responses

**400 Bad Request:**
```json
{
  "error": "You cannot follow yourself"
}
```

**401 Unauthorized:**
```json
{
  "error": "User not authenticated"
}
```

**404 Not Found:**
```json
{
  "error": "User to follow not found"
}
```

## Business Rules

1. **Self-Follow Prevention**: Users cannot follow themselves
2. **Duplicate Prevention**: Users cannot follow the same user twice
3. **Soft Delete**: Unfollowing removes the follow relationship
4. **Pagination**: Maximum page size is 100 items
5. **Timestamps**: All timestamps are in UTC format

## Usage Examples

### Example 1: Follow a User
```bash
curl -X POST https://api.example.com/api/follow/follow \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "followingId": "550e8400-e29b-41d4-a716-446655440000"
  }'
```

### Example 2: Get User's Followers
```bash
curl -X GET "https://api.example.com/api/follow/followers/550e8400-e29b-41d4-a716-446655440000?page=1&pageSize=20"
```

### Example 3: Get Follow Statistics
```bash
curl -X GET https://api.example.com/api/follow/stats/550e8400-e29b-41d4-a716-446655440000
```

### Example 4: Unfollow a User
```bash
curl -X DELETE https://api.example.com/api/follow/unfollow/550e8400-e29b-41d4-a716-446655440000 \
  -H "Authorization: Bearer YOUR_TOKEN"
```

## Data Models

### Follow Entity
```csharp
public class Follow
{
    public Guid Id { get; set; }
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual User Follower { get; set; }
    public virtual User Following { get; set; }
}
```

### FollowDto
```csharp
public class FollowDto
{
    public Guid Id { get; set; }
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserBasicDto? Follower { get; set; }
    public UserBasicDto? Following { get; set; }
}
```

### FollowStatsDto
```csharp
public class FollowStatsDto
{
    public int FollowerCount { get; set; }
    public int FollowingCount { get; set; }
    public bool IsFollowing { get; set; }
}
```

## Notes

- Follow relationships are directional (A follows B ≠ B follows A)
- Follower counts are real-time and updated immediately
- All timestamps are stored in UTC
- The API supports pagination for large follower/following lists
