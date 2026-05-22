# 📸 Cloudinary Usage Examples

Các ví dụ thực tế về cách sử dụng Cloudinary Service trong Anonwork Backend.

---

## 📝 Ví Dụ 1: Upload Avatar Người Dùng

### Controller

```csharp
using Anonwork.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UserAvatarController(ICloudinaryService cloudinaryService) : ControllerBase
{
    /// <summary>
    /// Upload user avatar
    /// </summary>
    [HttpPost("avatar")]
    public async Task<IActionResult> UploadAvatar(IFormFile file, CancellationToken ct)
    {
        try
        {
            // Validate file
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "File is empty" });

            // Upload to Cloudinary
            var avatarUrl = await cloudinaryService.UploadImageAsync(file, "avatars", ct);

            // TODO: Update user avatar in database
            // var userId = GetUserIdFromToken();
            // await _userService.UpdateAvatarAsync(userId, avatarUrl);

            return Ok(new { avatarUrl });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to upload avatar" });
        }
    }

    /// <summary>
    /// Delete user avatar
    /// </summary>
    [HttpDelete("avatar/{publicId}")]
    public async Task<IActionResult> DeleteAvatar(string publicId, CancellationToken ct)
    {
        try
        {
            var success = await cloudinaryService.DeleteImageAsync(publicId, ct);
            
            if (!success)
                return NotFound(new { error = "Avatar not found" });

            // TODO: Clear avatar from database
            // var userId = GetUserIdFromToken();
            // await _userService.ClearAvatarAsync(userId);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to delete avatar" });
        }
    }
}
```

---

## 📸 Ví Dụ 2: Upload Hình Ảnh cho Bài Viết

### Controller

```csharp
using Anonwork.Application.Interfaces;
using Anonwork.Application.Features.Posts.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/posts")]
[Authorize]
public class PostImagesController(ICloudinaryService cloudinaryService) : ControllerBase
{
    /// <summary>
    /// Upload images for a post
    /// </summary>
    [HttpPost("{postId}/images")]
    public async Task<IActionResult> UploadPostImages(
        Guid postId,
        IFormFileCollection files,
        CancellationToken ct)
    {
        try
        {
            // Validate files
            if (files == null || files.Count == 0)
                return BadRequest(new { error = "No files provided" });

            if (files.Count > 10)
                return BadRequest(new { error = "Maximum 10 images allowed" });

            // Upload to Cloudinary
            var imageUrls = await cloudinaryService.UploadImagesAsync(files, "posts", ct);

            // TODO: Save image URLs to database
            // var post = await _postRepository.GetByIdAsync(postId);
            // foreach (var url in imageUrls)
            // {
            //     post.Images.Add(new PostImage { ImageUrl = url });
            // }
            // await _postRepository.UpdateAsync(post);

            return Ok(new UploadPostImagesResponseDto
            {
                ImageUrls = imageUrls,
                UploadedCount = imageUrls.Count,
                TotalCount = files.Count
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to upload images" });
        }
    }

    /// <summary>
    /// Delete image from post
    /// </summary>
    [HttpDelete("{postId}/images/{publicId}")]
    public async Task<IActionResult> DeletePostImage(
        Guid postId,
        string publicId,
        CancellationToken ct)
    {
        try
        {
            // TODO: Verify ownership
            // var userId = GetUserIdFromToken();
            // var post = await _postRepository.GetByIdAsync(postId);
            // if (post.AuthorId != userId)
            //     return Forbid();

            var success = await cloudinaryService.DeleteImageAsync(publicId, ct);
            
            if (!success)
                return NotFound(new { error = "Image not found" });

            // TODO: Remove image from database
            // var image = post.Images.FirstOrDefault(i => i.PublicId == publicId);
            // if (image != null)
            //     post.Images.Remove(image);
            // await _postRepository.UpdateAsync(post);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to delete image" });
        }
    }

    /// <summary>
    /// Get post image with thumbnail
    /// </summary>
    [HttpGet("{postId}/images/{publicId}/thumbnail")]
    [AllowAnonymous]
    public IActionResult GetPostImageThumbnail(Guid postId, string publicId)
    {
        try
        {
            var thumbnailUrl = cloudinaryService.GetImageUrl(
                publicId,
                width: 300,
                height: 300,
                quality: "auto"
            );

            return Ok(new { url = thumbnailUrl });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to get thumbnail" });
        }
    }
}
```

---

## 💬 Ví Dụ 3: Upload Hình Ảnh cho Bình Luận

### Controller

```csharp
using Anonwork.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/comments")]
[Authorize]
public class CommentImagesController(ICloudinaryService cloudinaryService) : ControllerBase
{
    /// <summary>
    /// Upload image for comment
    /// </summary>
    [HttpPost("{commentId}/image")]
    public async Task<IActionResult> UploadCommentImage(
        Guid commentId,
        IFormFile file,
        CancellationToken ct)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "File is empty" });

            var imageUrl = await cloudinaryService.UploadImageAsync(file, "comments", ct);

            // TODO: Update comment with image
            // var comment = await _commentRepository.GetByIdAsync(commentId);
            // comment.ImageUrl = imageUrl;
            // await _commentRepository.UpdateAsync(comment);

            return Ok(new { imageUrl });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to upload image" });
        }
    }
}
```

---

## 🎨 Ví Dụ 4: Image Transformations

### Service Usage

```csharp
using Anonwork.Application.Interfaces;

public class ImageTransformationService
{
    private readonly ICloudinaryService _cloudinaryService;

    public ImageTransformationService(ICloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    /// <summary>
    /// Get thumbnail (300x300)
    /// </summary>
    public string GetThumbnail(string publicId)
    {
        return _cloudinaryService.GetImageUrl(publicId, width: 300, height: 300);
    }

    /// <summary>
    /// Get medium image (600x600)
    /// </summary>
    public string GetMedium(string publicId)
    {
        return _cloudinaryService.GetImageUrl(publicId, width: 600, height: 600);
    }

    /// <summary>
    /// Get large image (1200x1200)
    /// </summary>
    public string GetLarge(string publicId)
    {
        return _cloudinaryService.GetImageUrl(publicId, width: 1200, height: 1200);
    }

    /// <summary>
    /// Get responsive image (width only)
    /// </summary>
    public string GetResponsive(string publicId, int width)
    {
        return _cloudinaryService.GetImageUrl(publicId, width: width);
    }

    /// <summary>
    /// Get avatar (circular, 150x150)
    /// </summary>
    public string GetAvatar(string publicId)
    {
        return _cloudinaryService.GetImageUrl(publicId, width: 150, height: 150);
    }
}
```

### Controller Usage

```csharp
[ApiController]
[Route("api/v1/images")]
public class ImageTransformationController(
    ImageTransformationService transformationService) : ControllerBase
{
    [HttpGet("{publicId}/thumbnail")]
    public IActionResult GetThumbnail(string publicId)
    {
        var url = transformationService.GetThumbnail(publicId);
        return Ok(new { url });
    }

    [HttpGet("{publicId}/medium")]
    public IActionResult GetMedium(string publicId)
    {
        var url = transformationService.GetMedium(publicId);
        return Ok(new { url });
    }

    [HttpGet("{publicId}/large")]
    public IActionResult GetLarge(string publicId)
    {
        var url = transformationService.GetLarge(publicId);
        return Ok(new { url });
    }

    [HttpGet("{publicId}/responsive")]
    public IActionResult GetResponsive(string publicId, [FromQuery] int width = 400)
    {
        var url = transformationService.GetResponsive(publicId, width);
        return Ok(new { url });
    }

    [HttpGet("{publicId}/avatar")]
    public IActionResult GetAvatar(string publicId)
    {
        var url = transformationService.GetAvatar(publicId);
        return Ok(new { url });
    }
}
```

---

## 🔄 Ví Dụ 5: Batch Upload & Delete

### Service

```csharp
public class BatchImageService
{
    private readonly ICloudinaryService _cloudinaryService;

    public BatchImageService(ICloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    /// <summary>
    /// Upload multiple images and return with metadata
    /// </summary>
    public async Task<List<ImageMetadata>> UploadWithMetadataAsync(
        IEnumerable<IFormFile> files,
        string folder,
        CancellationToken ct = default)
    {
        var results = new List<ImageMetadata>();
        var fileList = files.ToList();

        foreach (var file in fileList)
        {
            try
            {
                var url = await _cloudinaryService.UploadImageAsync(file, folder, ct);
                results.Add(new ImageMetadata
                {
                    FileName = file.FileName,
                    Url = url,
                    Size = file.Length,
                    UploadedAt = DateTime.UtcNow,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                results.Add(new ImageMetadata
                {
                    FileName = file.FileName,
                    Size = file.Length,
                    UploadedAt = DateTime.UtcNow,
                    Success = false,
                    Error = ex.Message
                });
            }
        }

        return results;
    }

    /// <summary>
    /// Delete multiple images and return results
    /// </summary>
    public async Task<BatchDeleteResult> DeleteBatchAsync(
        IEnumerable<string> publicIds,
        CancellationToken ct = default)
    {
        var result = new BatchDeleteResult();
        var publicIdList = publicIds.ToList();

        foreach (var publicId in publicIdList)
        {
            try
            {
                var deleted = await _cloudinaryService.DeleteImageAsync(publicId, ct);
                if (deleted)
                    result.SuccessCount++;
                else
                    result.FailedCount++;
            }
            catch (Exception ex)
            {
                result.FailedCount++;
                result.Errors.Add(new { publicId, error = ex.Message });
            }
        }

        result.TotalCount = publicIdList.Count;
        return result;
    }
}

public class ImageMetadata
{
    public string FileName { get; set; } = string.Empty;
    public string? Url { get; set; }
    public long Size { get; set; }
    public DateTime UploadedAt { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
}

public class BatchDeleteResult
{
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public List<object> Errors { get; set; } = new();
}
```

---

## 🧪 Ví Dụ 6: Unit Testing

### Test Class

```csharp
using Anonwork.Application.Interfaces;
using Moq;
using Xunit;

public class CloudinaryServiceTests
{
    private readonly Mock<ICloudinaryService> _mockCloudinaryService;

    public CloudinaryServiceTests()
    {
        _mockCloudinaryService = new Mock<ICloudinaryService>();
    }

    [Fact]
    public async Task UploadImageAsync_WithValidFile_ReturnsUrl()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("test.jpg");
        mockFile.Setup(f => f.Length).Returns(1024);

        var expectedUrl = "https://res.cloudinary.com/test/image/upload/v123/test.jpg";
        _mockCloudinaryService
            .Setup(s => s.UploadImageAsync(mockFile.Object, "posts", default))
            .ReturnsAsync(expectedUrl);

        // Act
        var result = await _mockCloudinaryService.Object.UploadImageAsync(mockFile.Object, "posts");

        // Assert
        Assert.Equal(expectedUrl, result);
        _mockCloudinaryService.Verify(
            s => s.UploadImageAsync(mockFile.Object, "posts", default),
            Times.Once);
    }

    [Fact]
    public async Task DeleteImageAsync_WithValidPublicId_ReturnsTrue()
    {
        // Arrange
        var publicId = "anonwork/posts/test123";
        _mockCloudinaryService
            .Setup(s => s.DeleteImageAsync(publicId, default))
            .ReturnsAsync(true);

        // Act
        var result = await _mockCloudinaryService.Object.DeleteImageAsync(publicId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void GetImageUrl_WithDimensions_ReturnsTransformedUrl()
    {
        // Arrange
        var publicId = "anonwork/posts/test123";
        var expectedUrl = "https://res.cloudinary.com/test/image/upload/w_300,h_300/test123.jpg";
        _mockCloudinaryService
            .Setup(s => s.GetImageUrl(publicId, 300, 300, "auto"))
            .Returns(expectedUrl);

        // Act
        var result = _mockCloudinaryService.Object.GetImageUrl(publicId, 300, 300);

        // Assert
        Assert.Equal(expectedUrl, result);
    }
}
```

---

## 📋 Checklist Implementasi

Khi implement Cloudinary trong feature mới:

- [ ] Thêm `ICloudinaryService` vào constructor
- [ ] Validate file trước upload
- [ ] Handle exceptions với try-catch
- [ ] Lưu image URL vào database
- [ ] Implement delete functionality
- [ ] Thêm unit tests
- [ ] Update API documentation
- [ ] Test với Swagger UI

---

**Last Updated**: May 2026
**Version**: 1.0.0
