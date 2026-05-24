# 🔐 Anonwork Backend

**Anonwork** là một nền tảng Q&A ẩn danh hiện đại, cho phép người dùng đặt câu hỏi và chia sẻ kiến thức một cách ẩn danh hoặc công khai.

## 📋 Mục Lục

- [Tính Năng](#tính-năng)
- [Kiến Trúc](#kiến-trúc)
- [Công Nghệ](#công-nghệ)
- [Cài Đặt](#cài-đặt)
- [Chạy Dự Án](#chạy-dự-án)
- [API Documentation](#api-documentation)
- [Database](#database)
- [Cấu Trúc Thư Mục](#cấu-trúc-thư-mục)

## ✨ Tính Năng

### Hiện Tại
- ✅ **Authentication & Authorization**
  - Đăng ký người dùng
  - Đăng nhập với JWT
  - Refresh token
  - Đăng xuất
  - Role-based access control (Student, Teacher, Moderator, Admin)

- ✅ **User Management**
  - Tạo tài khoản với username, email, password
  - Tạo anonymous alias (tên ẩn danh)
  - Profile management (avatar, bio)

- ✅ **Database & Caching**
  - PostgreSQL database
  - Redis caching
  - Full-text search support

### Sắp Tới
- 📝 Posts Management (Create, Read, Update, Delete)
- 💬 Comments & Nested Replies
- 👍 Voting System (Upvote/Downvote)
- 🔖 Bookmarks
- 👥 Follow System
- 🔔 Notifications
- 💌 Direct Messaging
- 📊 Reporting & Moderation
- 🏷️ Tags & Categories

## 🏗️ Kiến Trúc

Project sử dụng **Clean Architecture** với 4 layers chính:

```
Anonwork-Backend/
├── src/
│   ├── Anonwork.API              # Presentation Layer
│   │   ├── Controllers/          # API endpoints
│   │   ├── DTOs/                 # Data Transfer Objects
│   │   ├── Middlewares/          # Custom middlewares
│   │   └── appsettings.json      # Configuration
│   │
│   ├── Anonwork.Application      # Business Logic Layer
│   │   ├── Features/             # Use cases
│   │   ├── Interfaces/           # Contracts
│   │   └── Common/               # Exceptions, Models
│   │
│   ├── Anonwork.Domain           # Core Business Rules
│   │   ├── Entities/             # Domain models
│   │   ├── Enums/                # Enumerations
│   │   └── Common/               # Base classes
│   │
│   └── Anonwork.Infrastructure   # Data Access & Services
│       ├── Persistence/          # DbContext
│       ├── Repositories/         # Data access
│       └── Services/             # External services
```

### Luồng Dữ Liệu

```
Request → API Controller → Use Case → Repository → Database
                ↓
            Response ← Mapper ← Entity
```

## 🛠️ Công Nghệ

| Công Nghệ | Phiên Bản | Mục Đích |
|-----------|----------|---------|
| .NET | 8.0 | Framework chính |
| C# | Latest | Ngôn ngữ lập trình |
| PostgreSQL | Latest | Database chính |
| Redis | Latest | Caching & Session |
| Entity Framework Core | 8.0 | ORM |
| JWT | 8.0.27 | Authentication |
| SignalR | 1.2.10 | Real-time communication |
| Swagger | 8.1.4 | API Documentation |
| Docker | Latest | Containerization |

## 📦 Cài Đặt

### Yêu Cầu
- .NET 8 SDK
- PostgreSQL 12+
- Redis 6+
- Docker (optional)

### Bước 1: Clone Repository
```bash
git clone <repository-url>
cd Anonwork-Backend
```

### Bước 2: Cài Đặt Dependencies
```bash
dotnet restore
```

### Bước 3: Cấu Hình Database
Tạo file `appsettings.Development.json` trong `src/Anonwork.API/`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=anonwork;Username=postgres;Password=your_password"
  },
  "Jwt": {
    "SecretKey": "your-secret-key-min-32-characters-long",
    "Issuer": "anonwork",
    "Audience": "anonwork-users",
    "ExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "REDIS_URL": "redis://localhost:6379"
}
```

### Bước 4: Tạo Database
```bash
# Chạy migration
dotnet ef database update --project src/Anonwork.Infrastructure --startup-project src/Anonwork.API

# Hoặc chạy SQL script
psql -U postgres -d anonwork -f Anonwork_pg.sql
```

## 🚀 Chạy Dự Án

### Development Mode
```bash
cd src/Anonwork.API
dotnet run
```

API sẽ chạy tại: `https://localhost:5001`

### Quick Start - 5 Phút

**1. Register tài khoản:**
```bash
POST /api/v1/auth/register
{
  "username": "testuser",
  "email": "test@example.com",
  "password": "Password123!",
  "anonAlias": "TestAlias"
}
```

**2. Login:**
```bash
POST /api/v1/auth/login
{
  "email": "test@example.com",
  "password": "Password123!"
}
# Lưu accessToken
```

**3. Tạo bài viết:**
```bash
POST /api/v1/posts
Authorization: Bearer {accessToken}
Content-Type: multipart/form-data

{
  "title": "My First Post",
  "content": "This is my first post...",
  "subjectId": "550e8400-e29b-41d4-a716-446655440000",
  "isAnonymous": false
}
```

**4. Lấy danh sách bài:**
```bash
GET /api/v1/posts?page=1&pageSize=10
```

**5. Xem chi tiết bài:**
```bash
GET /api/v1/posts/{postId}
```

### Docker
```bash
docker-compose up -d
```

## 🖼️ Image Storage (Cloudinary)

Anonwork sử dụng **Cloudinary** để lưu trữ hình ảnh. Xem hướng dẫn setup:

- **Quick Start**: [CLOUDINARY_QUICKSTART.md](./CLOUDINARY_QUICKSTART.md) (5 phút)
- **Full Setup**: [docs/CLOUDINARY_SETUP.md](./docs/CLOUDINARY_SETUP.md)
- **Examples**: [docs/CLOUDINARY_EXAMPLE.md](./docs/CLOUDINARY_EXAMPLE.md)
- **Implementation**: [CLOUDINARY_IMPLEMENTATION.md](./CLOUDINARY_IMPLEMENTATION.md)

## 📚 API Documentation

Swagger UI: `https://localhost:5001/swagger`

### Authentication Endpoints

#### Register
```http
POST /api/v1/auth/register
Content-Type: application/json

{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "SecurePassword123!",
  "anonAlias": "SilentWolf"
}
```

#### Login
```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "SecurePassword123!"
}
```

#### Refresh Token
```http
POST /api/v1/auth/refresh
Content-Type: application/json

{
  "refreshToken": "your-refresh-token"
}
```

#### Logout
```http
POST /api/v1/auth/logout
Authorization: Bearer <access-token>
Content-Type: application/json

{
  "refreshToken": "your-refresh-token",
  "accessToken": "your-access-token"
}
```

#### Create Admin Account (Admin Only)
```http
POST /api/v1/auth/admin
Authorization: Bearer <admin-token>
Content-Type: application/json

{
  "username": "admin2",
  "email": "admin2@anonwork.com",
  "password": "SecurePassword123!",
  "anonAlias": "AdminAlias2"
}
```

### Posts Endpoints

#### Create Post
```http
POST /api/v1/posts
Authorization: Bearer <access-token>
Content-Type: multipart/form-data

Form Data:
- title: "Cách học C# hiệu quả" (required, 5-255 chars)
- content: "Bài viết chi tiết..." (required, min 10 chars)
- subjectId: "550e8400-e29b-41d4-a716-446655440000" (required, GUID)
- tags: ["csharp", "learning"] (optional, max 5)
- isAnonymous: false (optional, default: false)
- images: [file1.jpg, file2.png] (optional, max 5 files, 5MB each)

Response: 201 Created
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

#### Get Post by ID
```http
GET /api/v1/posts/{postId}

Response: 200 OK
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

#### Get All Posts (Paginated)
```http
GET /api/v1/posts?page=1&pageSize=10

Query Parameters:
- page: int (default: 1)
- pageSize: int (default: 10, max: 100)

Response: 200 OK
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

#### Get Posts by Subject (Paginated)
```http
GET /api/v1/subjects/{subjectId}/posts?page=1&pageSize=10

Path Parameters:
- subjectId: GUID (required)

Query Parameters:
- page: int (default: 1)
- pageSize: int (default: 10, max: 100)

Response: 200 OK
{
  "posts": [...],
  "total": 25,
  "page": 1,
  "pageSize": 10,
  "totalPages": 3
}
```

#### Update Post
```http
PUT /api/v1/posts/{postId}
Authorization: Bearer <access-token>
Content-Type: multipart/form-data

Form Data (all optional):
- title: "Tiêu đề mới" (5-255 chars)
- content: "Nội dung mới..." (min 10 chars)
- tags: ["tag1", "tag2"] (max 5)
- newImages: [file.jpg] (max 5 files total)
- removeImageUrls: ["https://..."] (URLs to remove)

Response: 200 OK
{
  "id": "550e8400-e29b-41d4-a716-446655440001",
  "title": "Tiêu đề mới",
  ...
}
```

#### Delete Post
```http
DELETE /api/v1/posts/{postId}
Authorization: Bearer <access-token>

Response: 204 No Content
```

**Note:** Only post author, admin, or moderator can delete posts.

### HTTP Status Codes

| Code | Meaning | Khi Nào |
|------|---------|---------|
| `200 OK` | Success | GET, PUT thành công |
| `201 Created` | Created | POST thành công |
| `204 No Content` | No Content | DELETE thành công |
| `400 Bad Request` | Invalid data | Validation error, upload failed |
| `401 Unauthorized` | Not authenticated | Missing/invalid token |
| `403 Forbidden` | Not authorized | Not the resource owner |
| `404 Not Found` | Not found | Resource không tồn tại |

### Authorization & Roles

| Role | Tạo Bài | Edit Bài | Delete Bài | Delete Bài Khác |
|------|---------|----------|-----------|-----------------|
| **Student** | ✅ | ✅ Own | ✅ Own | ❌ |
| **Teacher** | ✅ | ✅ Own | ✅ Own | ❌ |
| **Moderator** | ✅ | ✅ Own | ✅ Own | ✅ |
| **Admin** | ✅ | ✅ Own | ✅ Own | ✅ |

### Image Upload

- **Supported formats:** jpg, jpeg, png, gif, webp, bmp, svg, ico
- **Max file size:** 5MB per file
- **Max files per post:** 5
- **Storage:** Cloudinary

### Validation Rules

**Posts:**
- `title`: Required, 5-255 characters
- `content`: Required, min 10 characters
- `subjectId`: Required, valid GUID
- `tags`: Optional, max 5 tags
- `isAnonymous`: Optional, default false
- `images`: Optional, max 5 files, each max 5MB

## 🧪 API Testing Examples

### Test 1: Register & Login
```bash
# 1. Register
curl -X POST https://localhost:5001/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "john_doe",
    "email": "john@example.com",
    "password": "SecurePassword123!",
    "anonAlias": "SilentWolf"
  }'

# Response: 201 Created
# Save accessToken and refreshToken

# 2. Login
curl -X POST https://localhost:5001/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "password": "SecurePassword123!"
  }'
```

### Test 2: Create Post with Images
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

# Response: 201 Created
# Save postId from response
```

### Test 3: Get Post by ID
```bash
curl -X GET https://localhost:5001/api/v1/posts/{postId}

# Response: 200 OK
# View count auto-incremented
```

### Test 4: Get All Posts
```bash
curl -X GET "https://localhost:5001/api/v1/posts?page=1&pageSize=10"

# Response: 200 OK
# Returns paginated list
```

### Test 5: Get Posts by Subject
```bash
curl -X GET "https://localhost:5001/api/v1/subjects/{subjectId}/posts?page=1&pageSize=10"

# Response: 200 OK
# Returns posts for specific subject
```

### Test 6: Update Post
```bash
curl -X PUT https://localhost:5001/api/v1/posts/{postId} \
  -H "Authorization: Bearer {access_token}" \
  -F "title=Tiêu đề mới" \
  -F "tags=csharp" \
  -F "tags=advanced" \
  -F "newImages=@file3.jpg" \
  -F "removeImageUrls=https://res.cloudinary.com/..."

# Response: 200 OK
```

### Test 7: Delete Post
```bash
curl -X DELETE https://localhost:5001/api/v1/posts/{postId} \
  -H "Authorization: Bearer {access_token}"

# Response: 204 No Content
```

### Test 8: Create Admin Account
```bash
# 1. First, create a regular user and update role to admin in database
UPDATE users SET role = 'admin' WHERE email = 'admin@anonwork.com';

# 2. Login as admin
curl -X POST https://localhost:5001/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@anonwork.com",
    "password": "SecurePassword123!"
  }'

# 3. Create new admin
curl -X POST https://localhost:5001/api/v1/auth/admin \
  -H "Authorization: Bearer {admin_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin2",
    "email": "admin2@anonwork.com",
    "password": "SecurePassword123!",
    "anonAlias": "AdminAlias2"
  }'

# Response: 201 Created
```

### Test 9: Admin Delete Any Post
```bash
# Admin can delete any post
curl -X DELETE https://localhost:5001/api/v1/posts/{any_post_id} \
  -H "Authorization: Bearer {admin_token}"

# Response: 204 No Content
```

## � Swagger Documentation

Swagger UI: `https://localhost:5001/swagger`

Tất cả API endpoints được document đầy đủ trên Swagger UI với:
- ✅ Endpoint descriptions
- ✅ Request/Response examples
- ✅ Parameter descriptions
- ✅ Authorization requirements
- ✅ HTTP status codes

**Cách sử dụng:**
1. Chạy project: `dotnet run`
2. Mở browser: `https://localhost:5001/swagger`
3. Authorize với JWT token (nếu cần)
4. Test endpoints trực tiếp trên UI

## �🗄️ Database

### Schema Chính

**Users** - Người dùng
- id (UUID)
- username (VARCHAR 50, UNIQUE)
- email (VARCHAR 255, UNIQUE)
- password_hash (TEXT)
- anon_alias (VARCHAR 80, UNIQUE)
- role (VARCHAR 20: student, teacher, moderator, admin)
- avatar_url, bio, created_at, updated_at

**Posts** - Bài viết
- id (UUID)
- author_id (FK → users)
- subject_id (FK → subjects)
- title, content
- is_anonymous (BOOLEAN)
- upvotes, comments_count, view_count
- status (active, pending, removed)
- created_at, updated_at

**Comments** - Bình luận
- id (UUID)
- post_id (FK → posts)
- author_id (FK → users)
- parent_id (FK → comments, for nested replies)
- content
- depth (0-3)
- upvotes, is_deleted
- created_at, updated_at

**Subjects** - Chủ đề
- id (UUID)
- name, slug
- icon_emoji
- post_count

**Votes** - Bình chọn
- id (UUID)
- user_id (FK → users)
- target_id (UUID)
- target_type (post, comment)
- vote_type (up, down)

**Follows** - Theo dõi
- id (UUID)
- follower_id (FK → users)
- following_id (FK → users)

**Bookmarks** - Lưu bài
- id (UUID)
- user_id (FK → users)
- post_id (FK → posts)

**Notifications** - Thông báo
- id (UUID)
- user_id (FK → users)
- actor_id (FK → users)
- type (new_comment, upvote, new_follower, mention, system, ranking)
- is_read (BOOLEAN)

**Conversations** - Cuộc trò chuyện
- id (UUID)
- is_group (BOOLEAN)
- title, avatar_url

**Messages** - Tin nhắn
- id (UUID)
- conversation_id (FK → conversations)
- sender_id (FK → users)
- content
- is_deleted (BOOLEAN)

**Reports** - Báo cáo
- id (UUID)
- reporter_id (FK → users)
- target_id (UUID)
- target_type (post, comment)
- reason
- status (pending, resolved, dismissed)

## 📁 Cấu Trúc Thư Mục

```
Anonwork-Backend/
├── .github/
│   └── workflows/              # CI/CD pipelines
├── src/
│   ├── Anonwork.API/
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs
│   │   │   └── UsersController.cs
│   │   ├── DTOs/
│   │   │   └── Auth.cs
│   │   ├── Middlewares/
│   │   │   └── Middlewares.cs
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   ├── Dockerfile
│   │   └── Anonwork.API.csproj
│   │
│   ├── Anonwork.Application/
│   │   ├── Features/
│   │   │   └── Auth/
│   │   │       ├── DTOs/
│   │   │       └── UseCases/
│   │   ├── Interfaces/
│   │   ├── Common/
│   │   └── DependencyInjection.cs
│   │
│   ├── Anonwork.Domain/
│   │   ├── Entities/
│   │   ├── Enums/
│   │   ├── Common/
│   │   └── Anonwork.Domain.csproj
│   │
│   └── Anonwork.Infrastructure/
│       ├── Persistence/
│       │   └── AppDbContext.cs
│       ├── Repositories/
│       ├── Services/
│       ├── Common/
│       └── DependencyInjection.cs
│
├── Anonwork-Backend.sln
├── Anonwork_pg.sql            # Database schema
├── README.md                   # This file
├── TASKS.md                    # Development tasks
├── Dockerfile
├── docker-compose.yml
└── .gitignore
```

## 🔧 Development Guidelines

### Code Style
- Sử dụng PascalCase cho class names, method names
- Sử dụng camelCase cho variables, parameters
- Thêm XML comments cho public methods
- Tuân theo SOLID principles

### Naming Conventions
- Controllers: `{Feature}Controller`
- Use Cases: `{Action}{Feature}UseCase`
- Repositories: `{Entity}Repository`
- Services: `{Feature}Service`
- DTOs: `{Feature}{Request|Response}Dto`

### Commit Messages
```
feat: Add new feature
fix: Fix bug
docs: Update documentation
refactor: Refactor code
test: Add tests
chore: Update dependencies
```

## 📞 Support & Development

### Development Tasks
Xem [TASKS.md](./TASKS.md) để xem danh sách chi tiết tất cả các task cần hoàn thành, được chia thành 5 phase:
- **Phase 1**: Core Features (Posts, Comments, Voting)
- **Phase 2**: Social Features (Bookmarks, Follows, Notifications)
- **Phase 3**: Messaging & Moderation
- **Phase 4**: Infrastructure & DevOps
- **Phase 5**: Performance & Security

### Báo Cáo Vấn Đề
Nếu gặp vấn đề:
1. Kiểm tra [TASKS.md](./TASKS.md) để xem các task liên quan
2. Tạo issue trên GitHub
3. Liên hệ team development

## 📄 License

MIT License - Xem file LICENSE để chi tiết

---

**Last Updated**: May 2026
**Version**: 1.0.0
