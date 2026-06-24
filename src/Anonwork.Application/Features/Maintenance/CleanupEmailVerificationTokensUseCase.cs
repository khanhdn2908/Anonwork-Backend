//using Anonwork.Application.Common;
//using Anonwork.Application.Interfaces;
//using Anonwork.Domain.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Options;

//namespace Anonwork.Application.Features.Maintenance;

//public class CleanupEmailVerificationTokensUseCase(
//    IAppDbContext dbContext,
//    IOptions<MaintenanceOptions> options)
//{
//    public async Task<int> ExecuteAsync(CancellationToken ct = default)
//    {
//        var cutoff = DateTime.UtcNow.AddDays(-options.Value.EmailVerificationTokenRetentionDays);

//        var expiredOrUsedTokens = await dbContext.EmailVerificationTokens
//            .Where(t => t.IsUsed || t.ExpiresAt < cutoff || t.CreatedAt < cutoff)
//            .ToListAsync(ct);

//        if (expiredOrUsedTokens.Count == 0)
//            return 0;

//        dbContext.EmailVerificationTokens.RemoveRange(expiredOrUsedTokens);
//        await dbContext.SaveChangesAsync(ct);

//        return expiredOrUsedTokens.Count;
//    }
//}
