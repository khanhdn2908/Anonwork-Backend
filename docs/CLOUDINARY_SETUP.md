# 🖼️ Cloudinary Setup Guide

Hướng dẫn cấu hình Cloudinary để lưu trữ hình ảnh cho Anonwork Backend.

## 📋 Mục Lục

- [Tạo Tài Khoản Cloudinary](#tạo-tài-khoản-cloudinary)
- [Lấy API Credentials](#lấy-api-credentials)
- [Cấu Hình Ứng Dụng](#cấu-hình-ứng-dụng)
- [Cài Đặt NuGet Package](#cài-đặt-nuget-package)
- [Sử Dụng Cloudinary Service](#sử-dụng-cloudinary-service)
- [Ví Dụ Thực Tế](#ví-dụ-thực-tế)
- [Troubleshooting](#troubleshooting)

---

## 🔐 Tạo Tài Khoản Cloudinary

### Bước 1: Truy Cập Cloudinary
1. Mở trình duyệt và truy cập [https://cloudinary.com](https://cloudinary.com)
2. Nhấp vào **Sign Up** (Đăng ký)

### Bước 2: Chọn Gói Miễn Phí
- Chọn **Free** plan (miễn phí)
- Điền thông tin đăng ký:
  - Email
  - Password
  - Full Name
  - Company (optional)

### Bước 3: Xác Nhận Email
- Kiểm tra email và nhấp vào link xác nhận
- Hoàn thành setup wizard

---

## 🔑 Lấy API Credentials

### Bước 1: Truy Cập Dashboard
1. Đăng nhập vào Cloudinary
2. Bạn sẽ thấy **Dashboard** với thông tin tài khoản

### Bước 2: Tìm Credentials
Trên Dashboard, bạn sẽ thấy:

```
Cloud Name:  your-cloud-name
API Key:     your-api-key
API Secret:  your-api-secret
```

### Bước 3: Sao Chép Thông Tin
- Sao chép **Cloud Name**
- Sao chép **API Key**
- Sao chép **API Secret** (giữ bí mật!)

---

## ⚙️ Cấu Hình Ứng Dụng

### Bước 1: Cập Nhật appsettings.Development.json

Mở file `src/Anonwork.API/appsettings.Development.json` và thêm:

```json
{
  "Cloudinary": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  }
}
```

**Ví dụ:**
```json
{
  "Cloudinary": {
    "CloudName": "dxyz1234",
    "ApiKey": "123456789012345",
    "ApiSecret": "abcdefghijklmnopqrstuvwxyz"
  }
}
```

### Bước 2: Cập Nhật appsettings.json (Production)

Mở file `src/Anonwork.API/appsettings.json`:

```json
{
  "Cloudinary": {
    "CloudName": "",
    "ApiKey": "",
    "ApiSecret": ""
  }
}
```

**Lưu ý**: Trong production, sử dụng environment variables thay vì hardcode credentials.

### Bước 3: Sử Dụng Environment Variables (Recommended)

Thay vì lưu credentials trong file, sử dụng environment variables:

#### Windows (PowerShell)
```powershell
$env:Cloudinary__CloudName = "your-cloud-name"
$env:Cloudinary__ApiKey = "your-api-key"
$env:Cloudinary__ApiSecret = "your-api-secret"
```

#### Windows (CMD)
```cmd
set Cloudinary__CloudName=your-cloud-name
set Cloudinary__ApiKey=your-api-key
set Cloudinary__ApiSecret=your-api-secret
```

#### Linux/Mac
```bash
export Cloudinary__CloudName="your-cloud-name"
export Cloudinary__ApiKey="your-api-key"
export Cloudinary__ApiSecret="your-api-secret"
```

#### Docker (.env file)
```env
Cloudinary__CloudName=your-cloud-name
Cloudinary__ApiKey=your-api-key
Cloudinary__ApiSecret=your-api-secret
```

---

## 📦 Cài Đặt NuGet Package

### Bước 1: Cài Đặt Cloudinary Package

Chạy lệnh sau trong Package Manager Console:

```bash
dotnet add package CloudinaryDotNet --version 1.26.0
```

Hoặc sử dụng Package Manager Console:

```powershell
Install-Package CloudinaryDotNet -Version 1.26.0
```

### Bước 2: Xác Nhận Cài Đặt

Kiểm tra file `Anonwork.Infrastructure.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="CloudinaryDotNet" Version="1.26.0" />
</ItemGroup>
```

### Bước 3: Restore Dependencies

```bash
dotnet restore
```

---

## 🚀 Sử Dụng Cloudinary Service

### Inject Service vào Controller

```csharp
using Anonwork.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/images")]
public class ImagesController(ICloudinaryService cloudinaryService) : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file, CancellationToken ct)
    {
        var imageUrl = await cloudinaryService.UploadImageAsync(file, "posts", ct);
        return Ok(new { url = imageUrl });
    }

    [HttpPost("upload-multiple")]
    public async Task<IActionResult> UploadImages(IFormFileCollection files, CancellationToken ct)
    {
        var imageUrls = await cloudinaryService.UploadImagesAsync(files, "posts", ct);
        return Ok(new { urls = imageUrls });
    }

    [HttpDelete("{publicId}")]
    public async Task<IActionResult> DeleteImage(string publicId, CancellationToken ct)
    {
        var success = await cloudinaryService.DeleteImageAsync(publicId, ct);
        return success ? Ok() : NotFound();
    }
}
```

### Các Phương Thức Có Sẵn

#### 1. Upload Một Hình Ảnh
```csharp
var imageUrl = await cloudinaryService.UploadImageAsync(file, "posts");
// Returns: https://res.cloudinary.com/...
```

#### 2. Upload Nhiều Hình Ảnh
```csharp
var imageUrls = await cloudinaryService.UploadImagesAsync(files, "posts");
// Returns: List<string> of URLs
```

#### 3. Xóa Một Hình Ảnh
```csharp
var success = await cloudinaryService.DeleteImageAsync(publicId);
// Returns: bool
```

#### 4. Xóa Nhiều Hình Ảnh
```csharp
var deletedCount = await cloudinaryService.DeleteImagesAsync(publicIds);
// Returns: int (number of deleted images)
```

#### 5. Lấy URL với Transformations
```csharp
var url = cloudinaryService.GetImageUrl(publicId, width: 300, height: 300);
// Returns: https://res.cloudinary.com/.../w_300,h_300/...
```

---

## 💡 Ví Dụ Thực Tế

### Ví Dụ 1: Upload Avatar Người Dùng

```csharp
[HttpPost("avatar")]
public async Task<IActionResult> UploadAvatar(IFormFile file, CancellationToken ct)
{
    try
    {
        var avatarUrl = await _cloudinaryService.UploadImageAsync(file, "avatars", ct);
        
        // Cập nhật user avatar trong database
        var user = await _userRepository.GetByIdAsync(userId);
        user.AvatarUrl = avatarUrl;
        await _userRepository.UpdateAsync(user);
        
        return Ok(new { avatarUrl });
    }
    catch (ArgumentException ex)
    {
        return BadRequest(new { error = ex.Message });
    }
}
```

### Ví Dụ 2: Upload Hình Ảnh cho Bài Viết

```csharp
[HttpPost("{postId}/images")]
public async Task<IActionResult> UploadPostImages(
    Guid postId,
    IFormFileCollection files,
    CancellationToken ct)
{
    try
    {
        var imageUrls = await _cloudinaryService.UploadImagesAsync(files, "posts", ct);
        
        // Lưu URLs vào database
        var post = await _postRepository.GetByIdAsync(postId);
        foreach (var url in imageUrls)
        {
            post.Images.Add(new PostImage { ImageUrl = url });
        }
        await _postRepository.UpdateAsync(post);
        
        return Ok(new { imageUrls });
    }
    catch (ArgumentException ex)
    {
        return BadRequest(new { error = ex.Message });
    }
}
```

### Ví Dụ 3: Lấy Hình Ảnh với Kích Thước Tùy Chỉnh

```csharp
[HttpGet("{publicId}/thumbnail")]
public IActionResult GetThumbnail(string publicId)
{
    var thumbnailUrl = _cloudinaryService.GetImageUrl(
        publicId,
        width: 200,
        height: 200,
        quality: "auto"
    );
    
    return Ok(new { url = thumbnailUrl });
}
```

---

## 🔍 Cloudinary Folder Structure

Hình ảnh sẽ được tổ chức theo cấu trúc:

```
anonwork/
├── posts/
│   ├── post-1.jpg
│   ├── post-2.png
│   └── ...
├── avatars/
│   ├── user-1.jpg
│   ├── user-2.jpg
│   └── ...
└── comments/
    ├── comment-1.jpg
    └── ...
```

---

## ⚙️ Cloudinary Settings (Optional)

### Tối Ưu Hóa Hình Ảnh

Trên Cloudinary Dashboard, bạn có thể cấu hình:

1. **Auto Format & Quality**
   - Tự động chọn định dạng tốt nhất (WebP, AVIF, etc.)
   - Tự động tối ưu chất lượng

2. **Responsive Images**
   - Tự động tạo các phiên bản khác nhau của hình ảnh
   - Phục vụ hình ảnh phù hợp với thiết bị

3. **CDN Caching**
   - Lưu cache hình ảnh trên CDN toàn cầu
   - Tăng tốc độ tải

---

## 🐛 Troubleshooting

### Lỗi: "Cloudinary CloudName is not configured"

**Nguyên nhân**: Chưa cấu hình CloudName trong appsettings

**Giải pháp**:
1. Kiểm tra file `appsettings.Development.json`
2. Đảm bảo `Cloudinary.CloudName` có giá trị
3. Restart ứng dụng

### Lỗi: "File type is not allowed"

**Nguyên nhân**: Định dạng file không được hỗ trợ

**Giải pháp**:
- Chỉ upload các định dạng: jpg, jpeg, png, gif, webp, bmp, svg, ico
- Kiểm tra extension của file

### Lỗi: "File size exceeds maximum allowed size"

**Nguyên nhân**: File quá lớn (> 5MB)

**Giải pháp**:
- Nén hình ảnh trước khi upload
- Sử dụng công cụ như TinyPNG, ImageOptim

### Lỗi: "Invalid API credentials"

**Nguyên nhân**: API Key hoặc API Secret sai

**Giải pháp**:
1. Kiểm tra lại credentials trên Cloudinary Dashboard
2. Đảm bảo không có khoảng trắng thừa
3. Regenerate API Key nếu cần

### Hình Ảnh Upload Thành Công Nhưng URL Không Hoạt Động

**Nguyên nhân**: Có thể là vấn đề CORS hoặc CDN

**Giải pháp**:
1. Kiểm tra CORS settings trên Cloudinary
2. Đợi vài phút để CDN cache
3. Thử truy cập URL trực tiếp trên trình duyệt

---

## 📚 Tài Liệu Tham Khảo

- [Cloudinary Documentation](https://cloudinary.com/documentation)
- [CloudinaryDotNet GitHub](https://github.com/cloudinary/CloudinaryDotNet)
- [Cloudinary API Reference](https://cloudinary.com/documentation/image_upload_api_reference)
- [Image Transformations](https://cloudinary.com/documentation/image_transformation_reference)

---

## 🔒 Bảo Mật

### Best Practices

1. **Không commit credentials**
   - Thêm `appsettings.Development.json` vào `.gitignore`
   - Sử dụng environment variables

2. **Regenerate API Secret**
   - Nếu bị leak, regenerate ngay lập tức
   - Trên Cloudinary Dashboard → Settings → API Keys

3. **Giới Hạn Quyền Truy Cập**
   - Sử dụng Restricted API Keys nếu có thể
   - Chỉ cấp quyền cần thiết

4. **Monitoring**
   - Kiểm tra usage trên Cloudinary Dashboard
   - Đặt alert cho quota

---

**Last Updated**: May 2026
**Version**: 1.0.0
