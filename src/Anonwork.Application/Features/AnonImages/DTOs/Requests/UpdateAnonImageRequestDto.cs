using Microsoft.AspNetCore.Http;

namespace Anonwork.Application.Features.AnonImages.DTOs.Requests;

public class UpdateAnonImageRequestDto
{
    public string Name { get; set; } = string.Empty;

    public IFormFile? Image { get; set; }

    public bool IsActive { get; set; }

    public bool IsExclusive { get; set; }
};
