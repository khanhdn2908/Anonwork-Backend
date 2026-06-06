namespace Anonwork.Application.Features.Posts.DTOs.Response;

public record PostVoteResponseDto(
    Guid PostId,
    int Upvotes,
    bool IsUpvoted,
    string Message
);
