-- Mock permissions for Anonwork
-- Database: PostgreSQL
-- Tables: permissions, roles

INSERT INTO permissions (id, code, description, is_active, created_at, updated_at)
VALUES
    (gen_random_uuid(), 'posts.create', 'Create posts', true, now(), now()),
    (gen_random_uuid(), 'posts.update', 'Update posts', true, now(), now()),
    (gen_random_uuid(), 'posts.delete', 'Soft delete posts', true, now(), now()),
    (gen_random_uuid(), 'posts.delete-permanent', 'Permanently delete posts', true, now(), now()),
    (gen_random_uuid(), 'posts.vote', 'Vote on posts', true, now(), now()),

    (gen_random_uuid(), 'subjects.read', 'Read subjects', true, now(), now()),
    (gen_random_uuid(), 'subjects.create', 'Create subjects', true, now(), now()),
    (gen_random_uuid(), 'subjects.update', 'Update subjects', true, now(), now()),
    (gen_random_uuid(), 'subjects.delete', 'Soft delete subjects', true, now(), now()),
    (gen_random_uuid(), 'subjects.delete-permanent', 'Permanently delete subjects', true, now(), now()),

    (gen_random_uuid(), 'permissions.read', 'Read permissions', true, now(), now()),
    (gen_random_uuid(), 'permissions.create', 'Create permissions', true, now(), now()),
    (gen_random_uuid(), 'permissions.update', 'Update permissions', true, now(), now()),
    (gen_random_uuid(), 'permissions.delete', 'Soft delete permissions', true, now(), now()),
    (gen_random_uuid(), 'permissions.delete-permanent', 'Permanently delete permissions', true, now(), now()),

    (gen_random_uuid(), 'roles.create', 'Create roles', true, now(), now()),
    (gen_random_uuid(), 'roles.update', 'Update roles', true, now(), now()),
    (gen_random_uuid(), 'roles.delete', 'Soft delete roles', true, now(), now()),
    (gen_random_uuid(), 'roles.delete-permanent', 'Permanently delete roles', true, now(), now()),
    (gen_random_uuid(), 'roles.read-permissions', 'Read role permissions', true, now(), now()),
    (gen_random_uuid(), 'roles.assign-permission', 'Assign permissions to roles', true, now(), now()),
    (gen_random_uuid(), 'roles.remove-permission', 'Remove permissions from roles', true, now(), now()),

    (gen_random_uuid(), 'users.read', 'Read users', true, now(), now()),
    (gen_random_uuid(), 'users.update', 'Update users', true, now(), now()),
    (gen_random_uuid(), 'users.delete', 'Soft delete users', true, now(), now()),
    (gen_random_uuid(), 'users.delete-permanent', 'Permanently delete users', true, now(), now()),
    (gen_random_uuid(), 'users.read-roles', 'Read user roles', true, now(), now()),
    (gen_random_uuid(), 'users.assign-role', 'Assign roles to users', true, now(), now()),
    (gen_random_uuid(), 'users.remove-role', 'Remove roles from users', true, now(), now()),

    (gen_random_uuid(), 'comments.create', 'Create comments', true, now(), now()),
    (gen_random_uuid(), 'comments.update', 'Update comments', true, now(), now()),
    (gen_random_uuid(), 'comments.delete', 'Soft delete comments', true, now(), now()),
    (gen_random_uuid(), 'comments.delete-permanent', 'Permanently delete comments', true, now(), now()),
    (gen_random_uuid(), 'comments.vote', 'Vote on comments', true, now(), now()),

    (gen_random_uuid(), 'bookmarks.create', 'Create bookmarks', true, now(), now()),
    (gen_random_uuid(), 'bookmarks.delete', 'Delete bookmarks', true, now(), now()),

    (gen_random_uuid(), 'follows.create', 'Follow users', true, now(), now()),
    (gen_random_uuid(), 'follows.delete', 'Unfollow users', true, now(), now()),
    (gen_random_uuid(), 'follows.read', 'Read follow status', true, now(), now()),

    (gen_random_uuid(), 'anon-images.read', 'Read anonymous images', true, now(), now()),
    (gen_random_uuid(), 'anon-images.create', 'Create anonymous images', true, now(), now()),
    (gen_random_uuid(), 'anon-images.update', 'Update anonymous images', true, now(), now()),
    (gen_random_uuid(), 'anon-images.delete', 'Soft delete anonymous images', true, now(), now()),
    (gen_random_uuid(), 'anon-images.delete-permanent', 'Permanently delete anonymous images', true, now(), now()),

    (gen_random_uuid(), 'subscription-plans.read', 'Read subscription plans', true, now(), now()),

    (gen_random_uuid(), 'user-subscriptions.read', 'Read user subscriptions', true, now(), now()),
    (gen_random_uuid(), 'user-subscriptions.manage', 'Manage user subscriptions', true, now(), now()),

    (gen_random_uuid(), 'payments.create', 'Create payment orders and renew subscriptions', true, now(), now()),
    (gen_random_uuid(), 'payments.read', 'Read payment orders', true, now(), now())
ON CONFLICT (code) DO UPDATE
SET
    description = EXCLUDED.description,
    is_active = EXCLUDED.is_active,
    updated_at = now();

INSERT INTO roles (id, name, description, is_active, created_at, updated_at)
VALUES
    (gen_random_uuid(), 'admin', 'System administrator', true, now(), now()),
    (gen_random_uuid(), 'menber', 'Regular member', true, now(), now()),
    (gen_random_uuid(), 'coordinator', 'Coordinator role', true, now(), now())
ON CONFLICT (name) DO UPDATE
SET
    description = EXCLUDED.description,
    is_active = EXCLUDED.is_active,
    updated_at = now();
