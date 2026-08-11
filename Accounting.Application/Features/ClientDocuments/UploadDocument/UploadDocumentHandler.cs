using Accounting.Application.Abstractions.Identity;
using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Abstractions.Storage;
using Accounting.Application.Common.Errors;
using Accounting.Domain.Entities;
using MediatR;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientDocuments.UploadDocument
{
    public sealed class UploadDocumentHandler : IRequestHandler<UploadDocumentCommand, Guid>
    {
        private readonly IClientDocumentRepository _repository;
        private readonly IUserManagementService _userManagementService;
        private readonly IFileStorage _fileStorage;
        private readonly IUnitOfWork _unitOfWork;

        public UploadDocumentHandler(
            IClientDocumentRepository repository,
            IUserManagementService userManagementService,
            IFileStorage fileStorage,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _userManagementService = userManagementService;
            _fileStorage = fileStorage;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(UploadDocumentCommand request, CancellationToken ct)
        {
            var userExists = await _userManagementService.ExistsAsync(request.UserId, ct);

            if (!userExists)
                throw new NotFoundException($"User with id {request.UserId} not found.");

            var storageKey = BuildStorageKey(request.UserId, request.FileName);

            await _fileStorage.UploadAsync(storageKey, request.Content, request.ContentType, ct);

            try
            {
                var document = ClientDocument.Create(
                    request.UserId,
                    request.Title,
                    request.FileName,
                    storageKey,
                    request.ContentType,
                    request.SizeBytes,
                    request.Category,
                    request.Direction,
                    request.Note);

                await _repository.AddAsync(document, ct);
                await _unitOfWork.SaveChangesAsync(ct);

                return document.Id;
            }
            catch
            {
                // Do not leave an orphaned blob behind if the row could not be written.
                await _fileStorage.DeleteAsync(storageKey, CancellationToken.None);
                throw;
            }
        }

        private static string BuildStorageKey(Guid userId, string fileName)
        {
            var extension = Path.GetExtension(fileName);

            if (extension.Length > 10 || extension.Any(c => !char.IsLetterOrDigit(c) && c != '.'))
                extension = string.Empty;

            return $"clients/{userId:N}/{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        }
    }
}
