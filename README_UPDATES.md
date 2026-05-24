# 📝 README Updates - API Usage Guide

**Ngày**: May 2026  
**Phiên bản**: 1.0.0

---

## 📋 Những Gì Được Thêm Vào README

### **1. Posts Endpoints Section**
✅ Create Post
✅ Get Post by ID
✅ Get All Posts (Paginated)
✅ Get Posts by Subject (Paginated)
✅ Update Post
✅ Delete Post

### **2. Create Admin Account Endpoint**
✅ POST /api/v1/auth/admin
✅ Admin-only endpoint
✅ Tạo admin account mới

### **3. HTTP Status Codes Table**
✅ 200 OK
✅ 201 Created
✅ 204 No Content
✅ 400 Bad Request
✅ 401 Unauthorized
✅ 403 Forbidden
✅ 404 Not Found

### **4. Authorization & Roles Table**
✅ Student, Teacher, Moderator, Admin
✅ Quyền tạo, edit, delete bài
✅ Quyền delete bài của người khác

### **5. Image Upload Guidelines**
✅ Supported formats
✅ Max file size
✅ Max files per post
✅ Storage provider (Cloudinary)

### **6. Validation Rules**
✅ Title validation
✅ Content validation
✅ SubjectId validation
✅ Tags validation
✅ Image validation

### **7. API Testing Examples**
✅ Test 1: Register & Login
✅ Test 2: Create Post with Images
✅ Test 3: Get Post by ID
✅ Test 4: Get All Posts
✅ Test 5: Get Posts by Subject
✅ Test 6: Update Post
✅ Test 7: Delete Post
✅ Test 8: Create Admin Account
✅ Test 9: Admin Delete Any Post

### **8. Swagger Documentation Section**
✅ Swagger UI URL
✅ Cách sử dụng Swagger
✅ Features của Swagger

### **9. Quick Start Guide**
✅ 5 bước setup nhanh
✅ Register tài khoản
✅ Login
✅ Tạo bài viết
✅ Lấy danh sách bài
✅ Xem chi tiết bài

---

## 📊 Content Added

### **Total Lines Added:** ~400 lines

### **Sections:**
1. **Posts Endpoints** - 150 lines
2. **Create Admin Endpoint** - 20 lines
3. **HTTP Status Codes** - 15 lines
4. **Authorization & Roles** - 15 lines
5. **Image Upload Guidelines** - 10 lines
6. **Validation Rules** - 10 lines
7. **API Testing Examples** - 120 lines
8. **Swagger Documentation** - 15 lines
9. **Quick Start Guide** - 45 lines

---

## 🎯 Cấu Trúc README Mới

```
README.md
├── 🔐 Anonwork Backend
├── 📋 Mục Lục
├── ✨ Tính Năng
├── 🏗️ Kiến Trúc
├── 🛠️ Công Nghệ
├── 📦 Cài Đặt
├── 🚀 Chạy Dự Án
│   ├── Development Mode
│   ├── Quick Start (NEW)
│   └── Docker
├── 🖼️ Image Storage (Cloudinary)
├── 📚 API Documentation
│   ├── Authentication Endpoints
│   ├── Posts Endpoints (NEW)
│   ├── HTTP Status Codes (NEW)
│   ├── Authorization & Roles (NEW)
│   ├── Image Upload (NEW)
│   ├── Validation Rules (NEW)
│   └── API Testing Examples (NEW)
├── 📖 Swagger Documentation (NEW)
├── 🗄️ Database
├── 📁 Cấu Trúc Thư Mục
├── 🔧 Development Guidelines
├── 📞 Support & Development
└── 📄 License
```

---

## 📝 Ví Dụ Endpoints

### **Auth Endpoints**
```
POST /api/v1/auth/register
POST /api/v1/auth/login
POST /api/v1/auth/refresh
POST /api/v1/auth/logout
POST /api/v1/auth/admin (NEW)
```

### **Posts Endpoints**
```
POST   /api/v1/posts                    - Create post
GET    /api/v1/posts                    - Get all posts
GET    /api/v1/posts/{id}               - Get post by ID
PUT    /api/v1/posts/{id}               - Update post
DELETE /api/v1/posts/{id}               - Delete post
GET    /api/v1/subjects/{id}/posts      - Get posts by subject
```

---

## 🧪 Testing Examples

README giờ có 9 test examples:
1. Register & Login
2. Create Post with Images
3. Get Post by ID
4. Get All Posts
5. Get Posts by Subject
6. Update Post
7. Delete Post
8. Create Admin Account
9. Admin Delete Any Post

Mỗi example có:
- ✅ Full curl command
- ✅ Request body
- ✅ Response example
- ✅ Explanation

---

## 📖 Swagger Documentation

Thêm section hướng dẫn sử dụng Swagger UI:
- ✅ URL: https://localhost:5001/swagger
- ✅ Cách authorize
- ✅ Cách test endpoints
- ✅ Features

---

## 🎯 Quick Start

Thêm 5-minute quick start guide:
1. Register tài khoản
2. Login
3. Tạo bài viết
4. Lấy danh sách bài
5. Xem chi tiết bài

---

## ✅ Checklist

- ✅ Posts endpoints documentation
- ✅ Create admin endpoint
- ✅ HTTP status codes table
- ✅ Authorization & roles table
- ✅ Image upload guidelines
- ✅ Validation rules
- ✅ 9 API testing examples
- ✅ Swagger documentation
- ✅ Quick start guide
- ✅ All examples with curl commands

---

## 📊 Summary

**README giờ có:**
- ✅ 6 Auth endpoints
- ✅ 6 Posts endpoints
- ✅ 9 Testing examples
- ✅ Complete API documentation
- ✅ Quick start guide
- ✅ Swagger UI guide
- ✅ Authorization & roles info
- ✅ Validation rules
- ✅ Image upload guidelines

**Total:** ~400 lines of API documentation added

---

**Hoàn thành!** 🚀

README giờ có đầy đủ hướng dẫn sử dụng API.

