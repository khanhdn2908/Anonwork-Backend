using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anonwork.Application.Features.Comments
{
    public class DeleteCommentUseCasePermanent(IUnitOfWork unitOfWork)
    {
        private readonly IGenericRepository<Comment> _commentRepository = unitOfWork.GetRepository<Comment>();

        public async Task ExecuteAsync(Guid currentUserId, Guid commentId, CancellationToken ct = default)
        {
            // ── Validate input ──────────────────────────
            if (currentUserId == Guid.Empty)
                throw new ArgumentException("Current user ID is required.");

            if (commentId == Guid.Empty)
                throw new ArgumentException("Comment ID is required.");

            // ── Find comment ────────────────────────────
            var comment = await _commentRepository.FindSingleAsync(c => c.Id == commentId, ct);
            if (comment is null)
                throw new NotFoundException(nameof(Comment), commentId);

            if (comment.IsActive)
                throw new ArgumentException("Comment need delete first");

            await _commentRepository.DeleteAsync(comment, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }
    }
}
