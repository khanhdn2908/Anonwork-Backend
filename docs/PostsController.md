# 📝 Posts Controller Documentation

## Overview

The `PostsController` handles all post-related operations including creating, reading, updating, deleting, and searching posts. It supports image uploads via Cloudinary and provides full-text search capabilities.

**Base Route:** `api/v1/posts`

---

## Endpoints

### 1. Create Post
**POST** `/api/v1/posts`

Creates a new post with optional images and tags.

**Authentication:** Required (Bearer Token)

**Request:**
```http
POST /api/v1/posts
Authorization: Bearer {access_token}
Content-Type: multipart/form-data

Form Data:
- title: string (required, 5-255 characters)
- content: string (required, min 10 characters)
- subjectId: GUID (required)
- tags: string[] (optional, max 5 tags)
- isAnonymous: boolean (optional, default: false)
- images: IFormFile[] (optional, max 5 files, 5MB each)
```

**Response:** `201 Created`
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440001",
  "title": "Cách học C# hiệu quả",
  "content": "Bài viết chi tiết...",
  "authorId": "550e8400-e29b-41d4-a716-446655440002",
  "authorUsername": "john_doe",
  "authorAnonAlias": "SilentWolf",
  "isAnonymous": false,
  "subjectId": "550e8400-e29b-41d4-a716-446655440000",
  "subjectName": "C# Programming",
  "imageUrls": ["https://res.cloudinary.com/..."],
  "tags": ["csharp", "learning"],
  "upvotes": 0,
  "commentsCount": 0,
  "viewCount": 0,
  "status": "active",
  "createdAt": "2026-05-22T10:30:00Z",
  "updatedAt": "2026-05-22T10:30:00Z"
}
```

**Error Responses:**
- `400 Bad Request` - Invalid request data or image upload failed
- `401 Unauthorized` - Missing or invalid token

**Example:**
```bash
curl -X POST https://localhost:5001/api/v1/posts \
  -H "Authorization: Bearer {access_token}" \
  -F "title=Cách học C# hiệu quả" \
  -F "content=Bài viết chi tiết về cách học C#..." \
  -F "subjectId=550e8400-e29b-41d4-a716-446655440000" \
  -F "tags=csharp" \
  -F "tags=learning" \
  -F "isAnonymous=false" \
  -F "images=@file1.jpg" \
  -F "images=@file2.png"
```

---

### 2. Get Post by ID
**GET** `/api/v1/posts/{id}`

Retrieves a specific post by its ID. View count is automatically incremented.

**Authentication:** Not required

**Path Parameters:**
- `id` (GUID, required) - Post ID

**Response:** `200 OK`
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440001",
  "title": "Cách học C# hiệu quả",
  "content": "Bài viết chi tiết...",
  "authorId": "550e8400-e29b-41d4-a716-446655440002",
  "authorUsername": "john_doe",
  "authorAnonAlias": "SilentWolf",
  "isAnonymous": false,
  "subjectId": "550e8400-e29b-41d4-a716-446655440000",
  "subjectName": "C# Programming",
  "imageUrls": ["https://res.cloudinary.com/..."],
  "tags": ["csharp", "learning"],
  "upvotes": 0,
  "commentsCount": 0,
  "viewCount": 1,
  "status": "active",
  "createdAt": "2026-05-22T10:30:00Z",
  "updatedAt": "2026-05-22T10:30:00Z"
}
```

**Error Responses:**
- `404 Not Found` - Post not found

**Example:**
```bash
curl -X GET https://localhost:5001/api/v1/posts/550e8400-e29b-41d4-a716-446655440001
```

---

### 3. Get All Posts (Paginated)
**GET** `/api/v1/posts`

Retrieves a paginated list of all active posts, sorted by creation date (newest first).

**Authentication:** Not required

**Query Parameters:**
- `page` (int, optional, default: 1) - Page number
- `pageSize` (int, optional, default: 10, max: 100) - Items per page

**Response:** `200 OK`
```json
{
  "posts": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440001",
      "title": "Cách học C# hiệu quả",
      ...
    }
  ],
  "total": 50,
  "page": 1,
  "pageSize": 10,
  "totalPages": 5
}
```

**Example:**
```bash
curl -X GET "https://localhost:5001/api/v1/posts?page=1&pageSize=10"
```

---

### 4. Search Posts
**GET** `/api/v1/posts/search`

Searches posts by title and content using PostgreSQL full-text search. Results are ranked by relevance and sorted by creation date.

**Authentication:** Not required

**Query Parameters:**
- `q` (string, required) - Search query (minimum 2 characters)
- `page` (int, optional, default: 1) - Page number
- `pageSize` (int, optional, default: 10, max: 100) - Items per page

**Response:** `200 OK`
```json
{
  "posts": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440001",
      "title": "Cách học C# hiệu quả",
      "content": "Bài viết chi tiết...",
      ...
    }
  ],
  "total": 5,
  "page": 1,
  "pageSize": 10,
  "totalPages": 1
}
```

**Error Responses:**
- `400 Bad Request` - Invalid search query (too short or empty)

**Example:**
```bash
# Search for "C#"
curl -X GET "https://localhost:5001/api/v1/posts/search?q=C%23&page=1&pageSize=10"

# Search for "learning"
curl -X GET "https://localhost:5001/api/v1/posts/search?q=learning&page=1&pageSize=10"
```

**Search Features:**
- Full-text search on title and content
- Results ranked by relevance
- Supports English language stemming
- Minimum query length: 2 characters

---

### 5. Update Post
**PUT** `/api/v1/posts/{id}`

Updates an existing post. Only the post author can update their own posts.

**Authentication:** Required (Bearer Token)

**Path Parameters:**
- `id` (GUID, required) - Post ID

**Request:**
```http
PUT /api/v1/posts/550e8400-e29b-41d4-a716-446655440001
Authorization: Bearer {access_token}
Content-Type: multipart/form-data

Form Data (all optional):
- title: string (5-255 characters)
- content: string (min 10 characters)
- tags: string[] (max 5 tags)
- newImages: IFormFile[] (max 5 files total)
- removeImageUrls: string[] (URLs to remove)
```

**Response:** `200 OK`
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440001",
  "title": "Tiêu đề mới",
  "content": "Nội dung mới...",
  ...
}
```

**Error Responses:**
- `400 Bad Request` - Invalid request data or image upload failed
- `401 Unauthorized` - Missing or invalid token
- `403 Forbidden` - Not the post author
- `404 Not Found` - Post not found

**Example:**
```bash
curl -X PUT https://localhost:5001/api/v1/posts/550e8400-e29b-41d4-a716-446655440001 \
  -H "Authorization: Bearer {access_token}" \
  -F "title=Tiêu đề mới" \
  -F "tags=csharp" \
  -F "tags=advanced" \
  -F "newImages=@file3.jpg" \
  -F "removeImageUrls=https://res.cloudinary.com/..."
```

---

### 6. Delete Post
**DELETE** `/api/v1/posts/{id}`

Deletes a post (soft delete). Only the post author can delete their own posts.

**Authentication:** Required (Bearer Token)

**Path Parameters:**
- `id` (GUID, required) - Post ID

**Response:** `204 No Content`

**Error Responses:**
- `401 Unauthorized` - Missing or invalid token
- `403 Forbidden` - Not the post author
- `404 Not Found` - Post not found

**Example:**
```bash
curl -X DELETE https://localhost:5001/api/v1/posts/550e8400-e29b-41d4-a716-446655440001 \
  -H "Authorization: Bearer {access_token}"
```

---

## Data Models

### PostResponseDto
```json
{
  "id": "GUID",
  "title": "string",
  "content": "string",
  "authorId": "GUID",
  "authorUsername": "string",
  "isAnonymous": "boolean",
  "authorAvatarUrl": "string",
  "subjectId": "GUID",
  "subjectName": "string",
  "media": [
    {
      "id": "GUID",
      "fileKey": "string",
      "publicUrl": "string",
      "contentType": "string",
      "displayOrder": "int",
      "fileSize": "long",
      "originalFileName": "string",
      "mediaType": "string"
    }
  ],
  "tags": ["string"],
  "upvotes": "int",
  "commentsCount": "int",
  "viewCount": "int",
  "averageRating": "decimal",
  "ratingsCount": "int",
  "qualityScore": "double",
  "myStars": "int?",
  "status": "string",
  "createdAt": "DateTime",
  "updatedAt": "DateTime",
  "isUpvotedByMe": "boolean"
}
```

### PostListResponseDto
```json
{
  "posts": [PostResponseDto],
  "total": "int",
  "page": "int",
  "pageSize": "int",
  "totalPages": "int"
}
```

---

## Validation Rules

### Create/Update Post
- **title**: Required, 5-255 characters
- **content**: Required, minimum 10 characters
- **subjectId**: Required, valid GUID
- **tags**: Optional, maximum 5 tags
- **isAnonymous**: Optional, default false
- **images**: Optional, maximum 5 files, each max 5MB

### Search
- **q**: Required, minimum 2 characters

---

## HTTP Status Codes

| Code | Meaning | When |
|------|---------|------|
| `200 OK` | Success | GET, PUT successful |
| `201 Created` | Created | POST successful |
| `204 No Content` | No Content | DELETE successful |
| `400 Bad Request` | Invalid data | Validation error, upload failed |
| `401 Unauthorized` | Not authenticated | Missing/invalid token |
| `403 Forbidden` | Not authorized | Not the resource owner |
| `404 Not Found` | Not found | Resource doesn't exist |

---

## Authorization

- **Create Post**: Requires authentication
- **Get Post**: No authentication required
- **Get All Posts**: No authentication required
- **Search Posts**: No authentication required
- **Update Post**: Requires authentication + must be post author
- **Delete Post**: Requires authentication + must be post author

---

## Image Upload

### Supported Formats
- jpg, jpeg, png, gif, webp, bmp, svg, ico

### Constraints
- Maximum file size: 5MB per file
- Maximum files per post: 5
- Storage provider: Cloudinary

### Upload Process
1. Files are uploaded to Cloudinary
2. Image URLs are stored in database
3. Images are associated with post
4. Display order is maintained

---

## Search Features

### Full-Text Search
- Uses PostgreSQL full-text search with GIN index
- Searches both title and content
- Results ranked by relevance
- Supports English language stemming
- Minimum query length: 2 characters

### Example Searches
```bash
# Search for "C#"
GET /api/v1/posts/search?q=C%23

# Search for "learning"
GET /api/v1/posts/search?q=learning

# Search for "database design"
GET /api/v1/posts/search?q=database%20design

# Paginated search
GET /api/v1/posts/search?q=C%23&page=2&pageSize=20
```

---

## Implementation Details

### CreatePostUseCase
- Validates input (title, content, subject)
- Creates post entity
- Handles tags (max 5)
- Handles images (max 5)
- Returns created post

### GetPostByIdUseCase
- Fetches post with related data
- Increments view count
- Returns post details

### GetPostsUseCase
- Fetches all active posts
- Supports pagination
- Sorted by creation date (newest first)
- Includes author and subject info

### SearchPostsUseCase
- Validates search query (min 2 chars)
- Uses PostgreSQL full-text search
- Ranks results by relevance
- Supports pagination
- Returns paginated results

### UpdatePostUseCase
- Validates ownership
- Updates post fields
- Handles image additions/removals
- Updates tags

### DeletePostUseCase
- Validates ownership
- Soft deletes post (sets status to "removed")
- Records deletion timestamp

---

## Error Handling

### Common Errors

**400 Bad Request**
```json
{
  "message": "Search query must be at least 2 characters."
}
```

**401 Unauthorized**
```json
{
  "message": "User not authenticated"
}
```

**403 Forbidden**
```json
{
  "message": "You are not authorized to perform this action"
}
```

**404 Not Found**
```json
{
  "message": "Post not found"
}
```

### 6. Rate Post
**POST** `/api/v1/posts/{id}/rate`

Rates or updates rating for a post (1 to 5 stars).

**Authentication:** Required (Bearer Token)

**Request Body:**
```json
{
  "stars": 5,
  "review": "Bài viết rất chi tiết và bổ ích!"
}
```

---

### 7. Get Post Ratings Summary
**GET** `/api/v1/posts/{id}/ratings`

Gets rating summary, star breakdown, user's rating, and recent reviews for a post.

**Authentication:** Public (Optional Bearer Token)

---

### 8. Delete Post Rating
**DELETE** `/api/v1/posts/{id}/rate`

Deletes current user's rating for a post.

**Authentication:** Required (Bearer Token)

---

## Testing Examples

### Test 1: Create Post
```bash
curl -X POST https://localhost:5001/api/v1/posts \
  -H "Authorization: Bearer {access_token}" \
  -F "title=My First Post" \
  -F "content=This is my first post content..." \
  -F "subjectId=550e8400-e29b-41d4-a716-446655440000" \
  -F "tags=test" \
  -F "isAnonymous=false"
```

### Test 2: Get Post
```bash
curl -X GET https://localhost:5001/api/v1/posts/550e8400-e29b-41d4-a716-446655440001
```

### Test 3: Rate Post
```bash
curl -X POST https://localhost:5001/api/v1/posts/550e8400-e29b-41d4-a716-446655440001/rate \
  -H "Authorization: Bearer {access_token}" \
  -H "Content-Type: application/json" \
  -d '{"stars": 5, "review": "Rất hay!"}'
```

---

## Related Documentation

- [README.md](../README.md) - Main project documentation
- [CLOUDINARY_SETUP.md](./CLOUDINARY_SETUP.md) - Image upload setup
- [TASKS.md](../TASKS.md) - Development tasks

---

**Last Updated:** July 2026  
**Version:** 1.1.0
