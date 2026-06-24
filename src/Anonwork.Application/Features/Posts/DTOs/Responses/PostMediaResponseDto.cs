namespace Anonwork.Application.Features.Posts.DTOs.Response;

public record PostMediaResponseDto(
    Guid Id,
    string FileKey,
    string FileUrl,
    string? ContentType,
    int DisplayOrder,
    long FileSize,
    string? OriginalFileName,
    string MediaType
);
