using Microsoft.AspNetCore.Http;

namespace Anonwork.Application.Interfaces;

/// <summary>
/// Interface for Cloudinary image upload service
/// </summary>
public interface ICloudinaryService
{
    /// <summary>
    /// Upload a single image to Cloudinary
    /// </summary>
    /// <param name="file">Image file to upload</param>
    /// <param name="folder">Folder path in Cloudinary (e.g., "posts", "avatars")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>URL of uploaded image</returns>
    Task<string> UploadImageAsync(IFormFile file, string folder, CancellationToken cancellationToken = default);

    /// <summary>
    /// Upload multiple images to Cloudinary
    /// </summary>
    /// <param name="files">Image files to upload</param>
    /// <param name="folder">Folder path in Cloudinary</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of uploaded image URLs</returns>
    Task<List<string>> UploadImagesAsync(IEnumerable<IFormFile> files, string folder, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete an image from Cloudinary
    /// </summary>
    /// <param name="publicId">Public ID of the image in Cloudinary</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if deletion was successful</returns>
    Task<bool> DeleteImageAsync(string publicId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete multiple images from Cloudinary
    /// </summary>
    /// <param name="publicIds">Public IDs of images to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of successfully deleted images</returns>
    Task<int> DeleteImagesAsync(IEnumerable<string> publicIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get image URL with transformations
    /// </summary>
    /// <param name="publicId">Public ID of the image</param>
    /// <param name="width">Image width (optional)</param>
    /// <param name="height">Image height (optional)</param>
    /// <param name="quality">Image quality (optional, default: "auto")</param>
    /// <returns>Transformed image URL</returns>
    string GetImageUrl(string publicId, int? width = null, int? height = null, string quality = "auto");
}
