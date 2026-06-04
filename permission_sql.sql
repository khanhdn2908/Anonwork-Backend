

INSERT INTO permissions (id, code, description, is_active, created_at, updated_at)
VALUES
    (gen_random_uuid(), 'users.read', 'Xem thông tin người dùng', true, now(), now()),
    (gen_random_uuid(), 'users.create', 'Tạo người dùng mới', true, now(), now()),
    (gen_random_uuid(), 'users.update', 'Cập nhật thông tin người dùng', true, now(), now()),
    (gen_random_uuid(), 'users.delete', 'Xóa người dùng', true, now(), now()),
    (gen_random_uuid(), 'users.assign-role', 'Gán role cho user', true, now(), now()),
    (gen_random_uuid(), 'users.remove-role', 'Bỏ role khỏi user', true, now(), now()),
    (gen_random_uuid(), 'users.read-roles', 'Xem danh sách role của user', true, now(), now());

INSERT INTO permissions (id, code, description, is_active, created_at, updated_at)
VALUES
    (gen_random_uuid(), 'roles.read', 'Xem danh sách role', true, now(), now()),
    (gen_random_uuid(), 'roles.create', 'Tạo role mới', true, now(), now()),
    (gen_random_uuid(), 'roles.update', 'Cập nhật role', true, now(), now()),
    (gen_random_uuid(), 'roles.delete', 'Xóa role', true, now(), now()),
    (gen_random_uuid(), 'roles.assign-permission', 'Gán permission cho role', true, now(), now()),
    (gen_random_uuid(), 'roles.remove-permission', 'Bỏ permission khỏi role', true, now(), now()),
    (gen_random_uuid(), 'roles.read-permissions', 'Xem danh sách permission của role', true, now(), now());

INSERT INTO permissions (id, code, description, is_active, created_at, updated_at)
VALUES
    (gen_random_uuid(), 'permissions.read', 'Xem danh sách permission', true, now(), now()),
    (gen_random_uuid(), 'permissions.create', 'Tạo permission mới', true, now(), now()),
    (gen_random_uuid(), 'permissions.update', 'Cập nhật permission', true, now(), now()),
    (gen_random_uuid(), 'permissions.delete', 'Xóa permission', true, now(), now());

INSERT INTO permissions (id, code, description, is_active, created_at, updated_at)
VALUES
    (gen_random_uuid(), 'posts.read', 'Xem bài viết', true, now(), now()),
    (gen_random_uuid(), 'posts.create', 'Tạo bài viết', true, now(), now()),
    (gen_random_uuid(), 'posts.update', 'Cập nhật bài viết', true, now(), now()),
    (gen_random_uuid(), 'posts.delete', 'Xóa bài viết', true, now(), now()),
    (gen_random_uuid(), 'posts.moderate', 'Duyệt hoặc kiểm duyệt bài viết', true, now(), now());

INSERT INTO permissions (id, code, description, is_active, created_at, updated_at)
VALUES
    (gen_random_uuid(), 'subjects.read', 'Xem danh sách subject', true, now(), now()),
    (gen_random_uuid(), 'subjects.create', 'Tạo subject mới', true, now(), now()),
    (gen_random_uuid(), 'subjects.update', 'Cập nhật subject', true, now(), now()),
    (gen_random_uuid(), 'subjects.delete', 'Xóa subject', true, now(), now());

INSERT INTO permissions (id, code, description, is_active, created_at, updated_at)
VALUES
    (gen_random_uuid(), 'comments.read', 'Xem bình luận', true, now(), now()),
    (gen_random_uuid(), 'comments.create', 'Tạo bình luận', true, now(), now()),
    (gen_random_uuid(), 'comments.update', 'Cập nhật bình luận', true, now(), now()),
    (gen_random_uuid(), 'comments.delete', 'Xóa bình luận', true, now(), now()),
    (gen_random_uuid(), 'comments.moderate', 'Kiểm duyệt bình luận', true, now(), now());

INSERT INTO permissions (id, code, description, is_active, created_at, updated_at)
VALUES
    (gen_random_uuid(), 'follows.read', 'Xem danh sách theo dõi', true, now(), now()),
    (gen_random_uuid(), 'follows.create', 'Theo dõi người dùng', true, now(), now()),
    (gen_random_uuid(), 'follows.delete', 'Bỏ theo dõi người dùng', true, now(), now());

INSERT INTO permissions (id, code, description, is_active, created_at, updated_at)
VALUES
    (gen_random_uuid(), 'notifications.read', 'Xem thông báo', true, now(), now()),
    (gen_random_uuid(), 'notifications.update', 'Cập nhật trạng thái thông báo', true, now(), now()),
    (gen_random_uuid(), 'notifications.delete', 'Xóa thông báo', true, now(), now());

INSERT INTO permissions (id, code, description, is_active, created_at, updated_at)
VALUES
    (gen_random_uuid(), 'payments.read', 'Xem thông tin thanh toán', true, now(), now()),
    (gen_random_uuid(), 'payments.create', 'Tạo thanh toán', true, now(), now()),
    (gen_random_uuid(), 'payments.manage', 'Quản lý thanh toán', true, now(), now());

INSERT INTO permissions (id, code, description, is_active, created_at, updated_at)
VALUES
    (gen_random_uuid(), 'subscription-plans.read', 'Xem gói đăng ký', true, now(), now()),
    (gen_random_uuid(), 'subscription-plans.create', 'Tạo gói đăng ký', true, now(), now()),
    (gen_random_uuid(), 'subscription-plans.update', 'Cập nhật gói đăng ký', true, now(), now()),
    (gen_random_uuid(), 'subscription-plans.delete', 'Xóa gói đăng ký', true, now(), now());

INSERT INTO permissions (id, code, description, is_active, created_at, updated_at)
VALUES
    (gen_random_uuid(), 'user-subscriptions.read', 'Xem đăng ký của user', true, now(), now()),
    (gen_random_uuid(), 'user-subscriptions.manage', 'Quản lý đăng ký của user', true, now(), now());

