using Accounting.Application.Abstractions.Persistence;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientRequests.ChangeAdminNote
{
    public sealed class ChangeAdminNoteHandler : IRequestHandler<ChangeAdminNoteCommand>
    {
        private readonly IClientRequestRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ChangeAdminNoteHandler(IClientRequestRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Handle(ChangeAdminNoteCommand request, CancellationToken ct)
        {
            var clientRequest = await _repository.GetByIdAsync(request.Id, ct);

            if (clientRequest == null)
            {
                throw new Application.Common.Errors.NotFoundException($"Client Request with id {request.Id} not found.");
            }

            clientRequest.SetAdminNote(request.adminNote);

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
