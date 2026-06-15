using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anonwork.Application.Features.AnonImages
{
    public class DeleteAnonImageUseCasePermanent(IUnitOfWork unitOfWork)
    {
        private readonly IGenericRepository<AnonImage> _anonImageRepo = unitOfWork.GetRepository<AnonImage>();

        public async Task ExecuteAsync(Guid anonImageId, CancellationToken ct = default)
        {
            if (anonImageId == Guid.Empty)
                throw new ArgumentException("Anon image id is required.");

            var anonImage = await _anonImageRepo.GetByIdAsync(anonImageId, ct)
                ?? throw new NotFoundException(nameof(AnonImage), anonImageId);

            if (anonImage.IsActive == true) throw new ArgumentException("Anon image need deleted");

            await _anonImageRepo.DeleteAsync(anonImageId,ct);
            await unitOfWork.SaveChangesAsync(ct);
        }
    }
}
