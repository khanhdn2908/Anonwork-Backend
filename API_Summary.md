# Tóm tắt chức năng và permission của các API

Tài liệu này tóm tắt các API hiện có trong `Anonwork.API`, bao gồm chức năng chính và permission/role yêu cầu cho từng endpoint.

## Quy ước chung

- `Authorize` không gắn policy: chỉ cần đăng nhập.
- `AllowAnonymous`: không cần đăng nhập.
- `Authorize(Policy = "Permission:...")`: cần đúng permission trong token.
- `Authorize(Roles = "admin")`: cần role `admin`.
- Một số API lấy `userId` từ JWT để thao tác theo người dùng hiện tại.

---

## 1) Auth API

Base path: `/api/v1/auth`

| Method | Endpoint | Chức năng | Yêu cầu |
|---|---|---|---|
| POST | `/register` | Đăng ký tài khoản mới, tạo email xác thực | Anonymous |
| POST | `/verify-email` | Xác thực email bằng token | Anonymous |
| POST | `/login` | Đăng nhập bằng email/password, trả JWT | Anonymous |
| POST | `/google` | Đăng nhập bằng Google ID token | Anonymous |
| POST | `/refresh` | Làm mới access token bằng refresh token | Anonymous |
| POST | `/forgot-password` | Gửi hướng dẫn đặt lại mật khẩu | Anonymous |
| POST | `/reset-password` | Đặt lại mật khẩu bằng token | Anonymous |
| POST | `/logout` | Đăng xuất, thu hồi token | Đã đăng nhập |

---

## 2) Users API

Base path: `/api/v1/users`

| Method | Endpoint | Chức năng | Yêu cầu |
|---|---|---|---|
| GET | `/me` | Lấy thông tin tài khoản hiện tại | Đã đăng nhập |
| GET | `/{id}` | Lấy thông tin user theo ID | `Permission:users.read` |
| GET | `/` | Lấy danh sách user có phân trang | `Permission:users.read` |
| PUT | `/me` | Cập nhật thông tin tài khoản hiện tại | Đã đăng nhập |
| PATCH | `/me/anon` | Bật/tắt chế độ anon mặc định của user | Đã đăng nhập |
| PATCH | `/me/anon-image/{anonImageId}` | Gán ảnh anon cho tài khoản hiện tại | Đã đăng nhập |
| PUT | `/{id}` | Cập nhật user theo ID | `Permission:users.update` |
| DELETE | `/me` | Xóa tài khoản hiện tại | Đã đăng nhập |
| DELETE | `/{id}` | Xóa user | `Permission:users.delete` |
| DELETE | `/{id}/permanent` | Xóa user vĩnh viễn | `Permission:users.delete-permanent` |
| GET | `/{userId}/roles` | Lấy danh sách role của user | `Permission:users.read-roles` |
| POST | `/{userId}/roles/{roleId}` | Gán role cho user | `Permission:users.assign-role` |
| DELETE | `/{userId}/roles/{roleId}` | Gỡ role khỏi user | `Permission:users.remove-role` |

---

## 3) Roles API

Base path: `/api/v1/roles`

| Method | Endpoint | Chức năng | Yêu cầu |
|---|---|---|---|
| GET | `/` | Lấy danh sách role | Mặc định công khai trong controller hiện tại |
| GET | `/{id}` | Lấy role theo ID | Mặc định công khai trong controller hiện tại |
| POST | `/` | Tạo role mới | `Permission:roles.create` |
| PUT | `/{id}` | Cập nhật role | `Permission:roles.update` |
| DELETE | `/{id}` | Xóa role mềm | `Permission:roles.delete` |
| DELETE | `/{id}/permanent` | Xóa role vĩnh viễn | `Permission:roles.delete-permanent` |
| GET | `/{roleId}/permissions` | Lấy permissions của role | `Permission:roles.read-permissions` |
| POST | `/{roleId}/permissions/{permissionId}` | Gán 1 permission cho role | `Permission:roles.assign-permission` |
| POST | `/{roleId}/permissions` | Gán nhiều permissions cho role | `Permission:roles.assign-permission` |
| DELETE | `/{roleId}/permissions/{permissionId}` | Gỡ permission khỏi role | `Permission:roles.remove-permission` |

---

## 4) Permissions API

Base path: `/api/v1/permissions`

| Method | Endpoint | Chức năng | Yêu cầu |
|---|---|---|---|
| GET | `/` | Lấy danh sách permission | `Permission:permissions.read` |
| GET | `/{id}` | Lấy permission theo ID | `Permission:permissions.read` |
| POST | `/` | Tạo permission mới | `Permission:permissions.create` |
| PUT | `/{id}` | Cập nhật permission | `Permission:permissions.update` |
| DELETE | `/{id}` | Xóa permission mềm | `Permission:permissions.delete` |
| DELETE | `/{id}/permanent` | Xóa permission vĩnh viễn | `Permission:permissions.delete-permanent` |

---

## 5) Subjects API

Base path: `/api/v1/subjects`

| Method | Endpoint | Chức năng | Yêu cầu |
|---|---|---|---|
| GET | `/` | Lấy danh sách subject | `Permission:subjects.read` |
| GET | `/{id}` | Lấy subject theo ID | `Permission:subjects.read` |
| POST | `/` | Tạo subject mới | `Permission:subjects.create` |
| PUT | `/{id}` | Cập nhật subject | `Permission:subjects.update` |
| DELETE | `/{id}` | Xóa subject mềm | `Permission:subjects.delete` |
| DELETE | `/{id}/permanent` | Xóa subject vĩnh viễn | `Permission:subjects.delete-permanent` |
| GET | `/{subjectId}/posts` | Lấy bài viết theo subject | Anonymous |

---

## 6) Posts API

Base path: `/api/v1/posts`

| Method | Endpoint | Chức năng | Yêu cầu |
|---|---|---|---|
| POST | `/` | Tạo bài viết, hỗ trợ upload ảnh | `Permission:posts.create` |
| GET | `/{id}` | Lấy chi tiết bài viết theo ID | Anonymous |
| GET | `/` | Lấy danh sách bài viết, hỗ trợ search/pagination | Anonymous |
| PUT | `/{id}` | Cập nhật bài viết, hỗ trợ thêm/xóa ảnh | `Permission:posts.update` |
| DELETE | `/{id}` | Xóa bài viết mềm | `Permission:posts.delete` |
| DELETE | `/{id}/permanent` | Xóa bài viết vĩnh viễn | `Permission:posts.delete-permanent` |
| POST | `/{id}/upvote` | Upvote / bỏ upvote bài viết | `Permission:posts.vote` |

Ghi chú:
- Một số endpoint public nhưng vẫn đọc `permission` từ JWT nếu có để tùy biến dữ liệu trả về.

---

## 7) Comments API

Base path: `/api/v1/comments`

| Method | Endpoint | Chức năng | Yêu cầu |
|---|---|---|---|
| POST | `/` | Tạo bình luận cho bài viết | `Permission:comments.create` |
| GET | `/post/{postId}` | Lấy danh sách bình luận theo bài viết | Anonymous |
| PUT | `/{commentId}` | Cập nhật nội dung bình luận | `Permission:comments.update` |
| DELETE | `/{commentId}` | Xóa bình luận mềm | `Permission:comments.delete` |
| DELETE | `/{commentId}/permanent` | Xóa bình luận vĩnh viễn | `Permission:comments.delete-permanent` |
| POST | `/{commentId}/upvote` | Upvote / bỏ upvote bình luận | `Permission:comments.vote` |

---

## 8) Bookmarks API

Base path: `/api/v1/bookmarks`

| Method | Endpoint | Chức năng | Yêu cầu |
|---|---|---|---|
| POST | `/` | Tạo bookmark cho bài viết | `Permission:bookmarks.create` |
| DELETE | `/{postId}` | Xóa bookmark khỏi bài viết | `Permission:bookmarks.delete` |
| GET | `/` | Lấy danh sách bookmark của user hiện tại | Đã đăng nhập |
| GET | `/{postId}/exists` | Kiểm tra post đã được bookmark chưa | Đã đăng nhập |

---

## 9) Follows API

Base path: `/api/v1/follows`

| Method | Endpoint | Chức năng | Yêu cầu |
|---|---|---|---|
| POST | `/` | Follow một user khác | `Permission:follows.create` |
| DELETE | `/{followingId}` | Unfollow user | `Permission:follows.delete` |
| GET | `/{id}` | Lấy chi tiết follow relationship | Anonymous |
| GET | `/followers/{userId}` | Lấy danh sách followers của user | Anonymous |
| GET | `/following/{userId}` | Lấy danh sách user đang follow | Anonymous |
| GET | `/stats/{userId}` | Lấy thống kê follow/follower | Anonymous |
| GET | `/is-following/{followingId}` | Kiểm tra user hiện tại có đang follow không | `Permission:follows.read` |

---

## 10) Anon Images API

Base path: `/api/v1/anon-images`

| Method | Endpoint | Chức năng | Yêu cầu |
|---|---|---|---|
| GET | `/` | Lấy danh sách ảnh anon | `Permission:anon-images.read` |
| GET | `/{id}` | Lấy ảnh anon theo ID | `Permission:anon-images.read` |
| POST | `/` | Tạo ảnh anon mới | `Permission:anon-images.create` |
| PUT | `/{id}` | Cập nhật ảnh anon | `Permission:anon-images.update` |
| DELETE | `/{id}` | Xóa ảnh anon mềm | `Permission:anon-images.delete` |
| DELETE | `/{id}/permanent` | Xóa ảnh anon vĩnh viễn | `Permission:anon-images.delete-permanent` |

---

## 11) Subscription Plans API

Base path: `/api/v1/subscription-plans`

| Method | Endpoint | Chức năng | Yêu cầu |
|---|---|---|---|
| GET | `/` | Lấy danh sách gói subscription | `Permission:subscription-plans.read` |
| GET | `/{id}` | Lấy gói subscription theo ID | `Permission:subscription-plans.read` |
| GET | `/slug/{slug}` | Lấy gói subscription theo slug | `Permission:subscription-plans.read` |
| POST | `/` | Tạo gói mới | Role `admin` |
| PUT | `/{id}` | Cập nhật gói | Role `admin` |
| DELETE | `/{id}` | Xóa gói | Role `admin` |

---

## 12) Payments API

Base path: `/api/v1/payments`

| Method | Endpoint | Chức năng | Yêu cầu |
|---|---|---|---|
| POST | `/create-order` | Tạo order thanh toán | `Permission:payments.create` |
| GET | `/orders/{orderId}` | Lấy trạng thái order | `Permission:payments.read` |
| POST | `/webhook` | Nhận webhook từ Sepay | Anonymous |
| POST | `/subscriptions/{subscriptionId}/renew` | Gia hạn subscription | `Permission:payments.create` |

---

## 13) Maintenance API

Base path: `/api/v1/maintenance`

| Method | Endpoint | Chức năng | Yêu cầu |
|---|---|---|---|
| POST | `/cleanup-email-verification-tokens` | Xóa token xác thực email hết hạn | Header `X-Maintenance-Secret` đúng secret cấu hình |
| POST | `/cleanup-unpaid-expired-orders` | Xóa order chưa thanh toán đã hết hạn | Header `X-Maintenance-Secret` đúng secret cấu hình |

---

## 14) Danh sách permission xuất hiện trong controller

- `posts.create`
- `posts.update`
- `posts.delete`
- `posts.delete-permanent`
- `posts.vote`
- `subjects.read`
- `subjects.create`
- `subjects.update`
- `subjects.delete`
- `subjects.delete-permanent`
- `permissions.read`
- `permissions.create`
- `permissions.update`
- `permissions.delete`
- `permissions.delete-permanent`
- `roles.create`
- `roles.update`
- `roles.delete`
- `roles.delete-permanent`
- `roles.read-permissions`
- `roles.assign-permission`
- `roles.remove-permission`
- `users.read`
- `users.update`
- `users.delete`
- `users.delete-permanent`
- `users.read-roles`
- `users.assign-role`
- `users.remove-role`
- `comments.create`
- `comments.update`
- `comments.delete`
- `comments.delete-permanent`
- `comments.vote`
- `bookmarks.create`
- `bookmarks.delete`
- `follows.create`
- `follows.delete`
- `follows.read`
- `anon-images.read`
- `anon-images.create`
- `anon-images.update`
- `anon-images.delete`
- `anon-images.delete-permanent`
- `subscription-plans.read`
- `payments.create`
- `payments.read`

---

## Ghi chú nhanh

- Một số controller đang cho phép `AllowAnonymous` ở các endpoint đọc công khai như post, comment, follow stats, subject posts, payment webhook.
- `RolesController` hiện không gắn `[Authorize]` ở cấp controller, nhưng các endpoint tạo/sửa/xóa vẫn có permission riêng.
- `SubscriptionPlansController` dùng role `admin` cho các thao tác ghi.
- `MaintenanceController` không dùng JWT permission mà dùng secret header để bảo vệ.
