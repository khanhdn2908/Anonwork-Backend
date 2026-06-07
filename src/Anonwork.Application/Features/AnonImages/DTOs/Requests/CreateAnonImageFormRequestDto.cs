using Microsoft.AspNetCore.Http;

namespace Anonwork.Application.Features.AnonImages.DTOs.Requests;

public class CreateAnonImageFormRequestDto
{
    public string Name { get; set; } = string.Empty;

    public IFormFile Image { get; set; } = null!;

    public bool IsActive { get; set; } = true;
}
