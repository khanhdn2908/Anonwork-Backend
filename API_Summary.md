# Anonwork Backend Summary

Tài liệu này là bản tóm tắt nhanh toàn bộ repository `Anonwork-Backend`. Mục tiêu là sau này chỉ cần đọc file này để hiểu cấu trúc, chức năng chính, các module quan trọng và trạng thái hiện tại của dự án mà không cần quét lại toàn bộ source.

---

## 1) Tổng quan dự án

`Anonwork-Backend` là backend cho một nền tảng Q&A/diễn đàn ẩn danh xây dựng bằng **.NET 8 / C#** theo **Clean Architecture**.

Mục tiêu chính:
- Xác thực và phân quyền người dùng bằng JWT + role/permission.
- Quản lý bài viết, bình luận, follow, bookmark, vote.
- Hỗ trợ ảnh ẩn danh, thanh toán, gói đăng ký, webhook Sepay.
- Có sẵn hạ tầng cho moderation, messaging, báo cáo, caching, migration.
- Đang chuyển hướng lưu file/ảnh sang **Cloudflare R2** thay cho lưu URL trực tiếp.

---

## 2) Cấu trúc solution

### Các project chính

- `src/Anonwork.API`
  - Tầng presentation: controllers, DTOs, middleware, auth policies, `Program.cs`.
- `src/Anonwork.Application`
  - Tầng business logic: use case, DTO, interfaces, exception, authorization helpers.
- `src/Anonwork.Domain`
  - Tầng domain: entities, enums, base classes.
- `src/Anonwork.Infrastructure`
  - Tầng hạ tầng: EF Core persistence, configurations, services, repositories, migrations.
- `src/Anonwork.Shared`
  - Shared project, hiện rất nhỏ.

### Tài liệu / file hỗ trợ

- `README.md`: giới thiệu tổng quan, setup, API docs, database, hướng dẫn chạy.
- `TASKS.md`: danh sách task theo phase và trạng thái.
- `docs/`: tài liệu chi tiết cho từng phần như controller, cloudinary, payment, repository usage.
- `*.sql`: script database / permission mock.
- `.github/workflows/ci.yml`: pipeline CI.

---

## 3) Kiến trúc tổng thể

Luồng xử lý chuẩn:

`Request -> API Controller -> Use Case -> Repository/UnitOfWork abstraction -> Database -> Response`

### Vai trò từng layer

- **API**: nhận request, validate input cơ bản, gọi use case, trả response HTTP.
- **Application**: chứa logic nghiệp vụ chính, chỉ phụ thuộc vào các interface/abstraction trong Application hoặc Domain, không reference trực tiếp Infrastructure.
- **Domain**: định nghĩa entity và quy tắc lõi của hệ thống.
- **Infrastructure**: triển khai truy cập dữ liệu, dịch vụ ngoài, cấu hình EF, migrations, email, JWT, Cloudinary, Sepay, R2.

---

## 4) Module chức năng chính

## 4.1 Authentication & Authorization

Các use case và thành phần liên quan:
- `LoginUseCase`
- `RegisterUseCase`
- `RefreshTokenUseCase`
- `LogoutUseCase`
- `ForgotPasswordUseCase`
- `ResetPasswordUseCase`
- `VerifyEmailUseCase`
- `GoogleLoginUseCase`
- `PermissionHandler`, `PermissionRequirement`, `PermissionPolicyProvider`
- `PermissionAuthorizationExtensions`

API:
- `AuthController`
- middleware xác thực và/hoặc xử lý exception trong `src/Anonwork.API/Middlewares`

Hệ thống này dùng:
- JWT access token
- refresh token
- permission-based authorization
- role-based access control

---

## 4.2 Users

Hiện có khá nhiều use case và DTO cho user:
- `GetMeUseCase`
- `GetUserUseCase`
- `GetAllUsersUseCase`
- `UpdateUserUseCase`
- `DeleteUserUseCase`
- `DeleteUserUseCasePermanent`
- `AssignRoleToUserUseCase`
- `RemoveRoleFromUserUseCase`
- `GetUserRolesUseCase`
- `AssignAnonImageToUserUseCase`
- `ToggleUserAnonDefaultUseCase`

DTO responses:
- `UserResponseDto`
- `UserListResponseDto`
- `UpdateUserResponseDto`
- `GetMeResponseDto`

API:
- `UsersController`

Domain:
- `User` entity
- `UserRole` entity

Ý nghĩa:
- Quản lý hồ sơ người dùng.
- Gán role / xóa role.
- Lấy thông tin cá nhân (`me`).
- Hỗ trợ anonymous alias và ảnh ẩn danh.

### Thay đổi lưu file/ảnh cho User
- `AvatarUrl` đã được đổi thành `AvatarKey`.
- Ý nghĩa: DB chỉ giữ key file trên R2, URL sẽ được build ở tầng service/response.

---

## 4.3 Posts

Đây là một trong các module lớn nhất.

Use case đáng chú ý:
- `CreatePostUseCase`
- `GetPostByIdUseCase`
- `GetPostsUseCase`
- `GetPostsBySubjectUseCase`
- `UpdatePostUseCase`
- `DeletePostUseCase`
- `DeletePostUseCasePermanent`
- `TogglePostVoteUseCase`
- `PostVoteProjectionHelper`

DTOs:
- `CreatePostRequestDto`
- `CreatePostRequest`
- `UpdatePostRequestDto`
- `UpdatePostRequest`
- `UploadPostImagesRequestDto`
- `PostResponseDto`
- `PostListResponseDto`
- `PostVoteResponseDto`

API:
- `PostsController`

Domain:
- `Post`
- `PostMedia`
- `PostTag`
- `Vote`

Lưu ý:
- Có hỗ trợ vote, media, tags, subject association, pagination.
- Có helper riêng cho projection vote để tối ưu query trả về.
- `PostImage` đã được thay bằng `PostMedia` để tránh giới hạn khi sau này hỗ trợ thêm file/video.

### Thay đổi lưu file/ảnh cho Post
- `PostMedia` là entity thay thế cho `PostImage`.
- `PostMedia` có thêm phân loại media bằng `PostMediaType`.
- Các field chính hiện tại:
  - `MediaType`
  - `FileKey`
  - `ContentType`
  - `DisplayOrder`
  - `FileSize`
  - `OriginalFileName`
  - `CreatedAt`
- `Post` đang giữ collection `PostMediaItems`.

---

## 4.4 Comments

Use case:
- `CreateCommentUseCase`
- `GetCommentsByPostUseCase`
- `UpdateCommentUseCase`
- `DeleteCommentUseCase`
- `DeleteCommentUseCasePermanent`
- `ToggleCommentVoteUseCase`

DTOs:
- `CreateCommentRequest`
- `UpdateCommentRequest`
- `CommentResponseDto`
- `CommentListResponseDto`
- `CommentVoteResponseDto`

API:
- `CommentController`

Domain:
- `Comment`

Đây là module đã có nền tảng đầy đủ cho CRUD và vote comment.

---

## 4.5 Follows

Use case:
- `FollowUserUseCase`
- `UnfollowUserUseCase`
- `GetFollowersUseCase`
- `GetFollowingUseCase`
- `GetFollowByIdUseCase`
- `GetFollowStatsUseCase`
- `IsFollowingUseCase`

DTOs:
- `FollowUserRequest`
- `FollowStatsDto`
- `FollowResponseDto`
- `PaginatedFollowResponseDto`

API:
- `FollowController`

Domain:
- `Follow`

Tài liệu liên quan:
- `docs/FollowController.md`

---

## 4.6 Bookmarks

Use case:
- `CreateBookmarkUseCase`
- `DeleteBookmarkUseCase`
- `GetBookmarksUseCase`
- `IsBookmarkedUseCase`

DTOs:
- `CreateBookmarkRequest`
- `BookmarkResponseDto`
- `BookmarkListResponseDto`

API:
- `BookmarkController`

Domain:
- `Bookmark`

---

## 4.7 Subjects

Use case:
- `CreateSubjectUseCase`
- `GetSubjectsUseCase`
- `GetSubjectByIdUseCase`
- `UpdateSubjectUseCase`
- `DeleteSubjectUseCase`
- `DeleteSubjectUseCasePermanent`

DTOs:
- `CreateSubjectRequestDto`
- `UpdateSubjectRequestDto`
- `SubjectResponseDto`
- `SubjectListResponseDto`

API:
- `SubjectsController`

Domain:
- `Subject`

---

## 4.8 Anonymous images

Use case:
- `CreateAnonImageUseCase`
- `GetAllAnonImagesUseCase`
- `GetAnonImageByIdUseCase`
- `UpdateAnonImageUseCase`
- `DeleteAnonImageUseCase`
- `DeleteAnonImageUseCasePermanent`

DTOs:
- `CreateAnonImageRequestDto`
- `UpdateAnonImageRequestDto`
- `AnonImageResponseDto`

API:
- `AnonImagesController`

Domain:
- `AnonImage`

### Thay đổi lưu file/ảnh cho AnonImage
- `ImageUrl` đã được đổi thành `FileKey`.
- Entity này giờ đóng vai trò metadata cho file ảnh trên R2.

---

## 4.9 Subscription plans & user subscriptions

Subscription plans use case:
- `CreateSubscriptionPlanUseCase`
- `GetAllSubscriptionPlansUseCase`
- `GetSubscriptionPlanByIdUseCase`
- `GetSubscriptionPlanBySlugUseCase`
- `UpdateSubscriptionPlanUseCase`
- `DeleteSubscriptionPlanUseCase`

DTOs:
- `CreateSubscriptionPlanRequestDto`
- `UpdateSubscriptionPlanRequestDto`
- `GetAllSubscriptionPlansRequestDto`
- `SubscriptionPlanResponseDto`

User subscriptions use case:
- `CreateUserSubscriptionUseCase`
- `GetUserSubscriptionByIdUseCase`
- `GetUserSubscriptionsByUserIdUseCase`
- `UpdateUserSubscriptionUseCase`
- `DeleteUserSubscriptionUseCase`

DTOs:
- `UserSubscriptionRequestDto`
- `UserSubscriptionResponseDto`

API:
- `SubscriptionPlansController`
- `UserSubscriptionsController`

Domain:
- `SubscriptionPlan`
- `UserSubscription`

---

## 4.10 Payments / Sepay

Use case:
- `CreateOrderUseCase`
- `GetOrderStatusUseCase`
- `HandleSepayWebhookUseCase`
- `RenewSubscriptionUseCase`

DTOs:
- `CreateOrderRequest`
- `OrderResponse`
- `SepayQrResponse`
- `SepayWebhookRequest`
- `WebhookResult`

API:
- `PaymentController`

Infrastructure services / config:
- `SepayService`
- `ISepayService`
- `SepayOptions`

Domain:
- `Order`

Tài liệu liên quan:
- `docs/PAYMENT_SEPAY_INTEGRATION.md`
- `docs/PAYMENT_QUICK_START.md`

---

## 4.11 Roles & Permissions

Use case:
- `CreateRoleUseCase`
- `GetAllRolesUseCase`
- `GetRoleByIdUseCase`
- `UpdateRoleUseCase`
- `DeleteRoleUseCase`
- `DeleteRoleUseCasePermanent`
- `AssignPermissionToRoleUseCase`
- `AssignPermissionsToRoleUseCase`
- `RemovePermissionFromRoleUseCase`
- `GetRolePermissionsUseCase`

Permissions use case:
- `CreatePermissionUseCase`
- `GetAllPermissionsUseCase`
- `GetPermissionByIdUseCase`
- `UpdatePermissionUseCase`
- `DeletePermissionUseCase`
- `DeletePermissionUseCasePermanent`

DTOs:
- `RoleRequestDto`
- `RoleDto`
- `AssignPermissionsRequestDto`
- `PermissionDto`
- `PermissionRequestDto`

API:
- `RolesController`
- `PermissionsController`

Domain:
- `Role`
- `Permission`
- `RolePermission`

Infrastructure:
- `RolePermissionService`
- configurations for role/permission mappings

---

## 4.12 Maintenance / housekeeping

Use case:
- `CleanupEmailVerificationTokensUseCase`
- `CleanupUnpaidExpiredOrdersUseCase`

API:
- `MaintenanceController`

Ý nghĩa:
- Dọn token xác thực email hết hạn.
- Dọn order chưa thanh toán quá hạn.

---

## 4.13 Folders / features hiện có khác

Các feature khác xuất hiện trong source và đã có nền tảng:
- `Messages`
- `Conversations`
- `Notifications`
- `Reports`
- `Votes`
- `Maintenance`
- `Permissions`
- `Roles`
- `Payments`
- `UserSubscriptions`

Một số module này đã có entity/config nhưng controller hoặc use case vẫn có thể chưa đầy đủ so với task list.

---

## 5) Domain entities chính

Các entity đã thấy trong `src/Anonwork.Domain/Entities`:

- `User`
- `UserRole`
- `Role`
- `Permission`
- `RolePermission`
- `Post`
- `PostMedia`
- `PostTag`
- `Comment`
- `Vote`
- `Bookmark`
- `Follow`
- `Subject`
- `AnonImage`
- `SubscriptionPlan`
- `UserSubscription`
- `Order`
- `Conversation`
- `ConversationMember`
- `Message`
- `Notification`
- `Report`
- `OneTimeToken`
- `EmailVerificationToken`

Base / common:
- `BaseEntity`
- enums trong `src/Anonwork.Domain/Enums/Enums.cs`

### Ghi chú về media/file
- `User` hiện lưu `AvatarKey`.
- `PostMedia` là entity lưu media của bài viết.
- `AnonImage` hiện lưu `FileKey`.
- Database không nên lưu URL public là nguồn sự thật chính; key file trên R2 là nguồn sự thật.

---

## 6) Infrastructure quan trọng

> Lưu ý kiến trúc: `Anonwork.Application` không reference trực tiếp `Anonwork.Infrastructure`. Application chỉ làm việc qua các interface/abstraction như `IUnitOfWork`, `IGenericRepository<T>`, `IR2Service`, `ISepayService`, ... còn Infrastructure là project implement các interface đó và được đăng ký qua DI ở tầng startup/API.

### DbContext / persistence
- `AppDBContext.cs`
- `UnitOfWork.cs`
- `GenericRepository.cs`

### EF Core configurations
Có rất nhiều `IEntityTypeConfiguration<>` cho từng entity, ví dụ:
- `UserConfiguration`
- `PostConfiguration`
- `CommentConfiguration`
- `VoteConfiguration`
- `FollowConfiguration`
- `BookmarkConfiguration`
- `SubscriptionPlanConfiguration`
- `UserSubscriptionConfiguration`
- `RoleConfiguration`
- `PermissionConfiguration`
- `RolePermissionConfiguration`
- `OrderConfiguration`
- `MessageConfiguration`
- `ConversationConfiguration`
- `ConversationMemberConfiguration`
- `NotificationConfiguration`
- `ReportConfiguration`
- `AnonImageConfiguration`
- `OneTimeTokenConfiguration`
- `EmailVerificationTokenConfiguration`
- `PostMediaConfiguration`
- `PostTagConfiguration`

### Services
- `JwtService`
- `PasswordHasher`
- `EmailSender`
- `CloudinaryService`
- `RolePermissionService`
- `SepayService`
- `R2Service` (đã bổ sung cho Cloudflare R2)

### Options / config classes
- `JwtOptions`
- `EmailOptions`
- `CloudinaryOptions`
- `SepayOptions`
- `MaintenanceOptions`
- `R2Options`

### Migrations
- `20260619141755_AddPostReadScopes`
- `AppDbContextModelSnapshot`

---

## 7) API layer

### Controllers hiện có
- `AuthController`
- `UsersController`
- `PostsController`
- `CommentController`
- `FollowController`
- `BookmarkController`
- `SubjectsController`
- `SubscriptionPlansController`
- `UserSubscriptionsController`
- `RolesController`
- `PermissionsController`
- `PaymentController`
- `AnonImagesController`
- `MaintenanceController`
- `BaseApiController`

### Middleware / auth helpers
- `Middlewares.cs`
- `Exception handling / request logging / authorization` có nền tảng trong project và task list.
- `PermissionAuthorizationExtensions`

### API DTOs ở tầng API
- `Auth.cs`
- `Follow.cs`
- `RolePermissionDtos.cs`
- `RegisterRequestDto.cs`
- `ResetPasswordRequestDto.cs`
- `ForgotPasswordRequestDto.cs`

---

## 8) Documentation hiện có

Trong repo có khá nhiều file tài liệu. Các file đáng nhớ:
- `README.md` — mô tả tổng quan lớn nhất.
- `TASKS.md` — theo dõi tiến độ phát triển.
- `API_Summary.md` — file tổng hợp này.
- `docs/GENERIC_REPOSITORY_USAGE.md`
- `docs/FollowController.md`
- `docs/PostsController.md`
- `docs/CLOUDINARY_SETUP.md`
- `docs/CLOUDINARY_EXAMPLE.md`
- `docs/PAYMENT_SEPAY_INTEGRATION.md`
- `docs/PAYMENT_QUICK_START.md`

---

## 9) Trạng thái phát triển theo `TASKS.md`

### Đã hoàn thành nhiều ở Phase 1
- Auth / JWT / refresh / logout đã xong.
- Posts CRUD và upload images đã xong.
- Một phần comments / voting / social features đã có nền tảng.
- Nhiều controller và use case cho user, role, permission, payment, subject, bookmark, follow đã có sẵn.

### Còn lại đáng chú ý
- Một số task comment, notifications, messaging, reports, tests, logging, rate limiting, encryption, docs, CI/CD vẫn còn trong kế hoạch.
- Đang trong quá trình chuyển hệ thống lưu ảnh/file từ URL trực tiếp sang R2-backed keys và metadata.

---

## 10) Những điểm cần chú ý khi làm việc tiếp

1. **Có file trùng / gần trùng tên ở vài nơi**
   - Ví dụ `GetPostsUseCase`, `GetPostByIdUseCase`, `PostVoteProjectionHelper`, `GetMeUseCase`, `UsersController`, `PostsController`, `BookmarkController` có cả đường dẫn dùng dấu `/` và `\` trong snapshot git status.
   - Nên kiểm tra xem đây là file thật sự khác nhau hay chỉ là vấn đề path trên Windows / Git status.

2. **Có nhiều file build output trong `bin/` và `obj/`**
   - Không nên đọc/commit các file này.

3. **Có file trong `.vs/`**
   - Đây là cache của Visual Studio, không nên đưa vào git.

4. **Media/file storage đã thay đổi mô hình**
   - `AvatarUrl` -> `AvatarKey`
   - `PostImage` -> `PostMedia`
   - `ImageUrl` -> `FileKey`
   - `PostMedia` có `MediaType` để phân biệt ảnh/file/video.

5. **Module đã khá lớn**
   - Nếu cần hiểu sâu một chức năng, nên đọc thêm file controller + use case + DTO + entity tương ứng.

---

## 11) Tóm tắt cực ngắn theo module

- **Auth**: đăng ký, đăng nhập, refresh, logout, Google login, quên mật khẩu, verify email.
- **Users**: hồ sơ, role, anon image, quản trị user.
- **Posts**: CRUD, list, filter by subject, media, vote.
- **Comments**: CRUD, nested replies, vote.
- **Follow / Bookmark**: follow người dùng, lưu bài viết.
- **Subjects**: quản lý chủ đề.
- **Subscription / Payment**: gói đăng ký, order, webhook Sepay.
- **Roles / Permissions**: hệ thống phân quyền chi tiết.
- **Storage**: R2 đang được thêm vào để lưu ảnh/file.
- **Infrastructure**: EF Core, repository, unit of work, external services, migrations.

---

## 12) Ghi chú cuối

Tài liệu này được tạo để làm “bản đồ nhanh” của repository. Khi codebase thay đổi đáng kể, nên cập nhật lại file này để giữ nó luôn là nguồn tham chiếu nhanh nhất.

**Last updated**: 2026-06-22
