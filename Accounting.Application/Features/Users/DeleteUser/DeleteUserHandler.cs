using Accounting.Application.Abstractions.Identity;
using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Abstractions.Storage;
using Accounting.Application.Common.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Users.DeleteUser
{
    public sealed class DeleteUserHandler : IRequestHandler<DeleteUserRequest>
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IClientDocumentRepository _documents;
        private readonly IFileStorage _fileStorage;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<DeleteUserHandler> _logger;

        public DeleteUserHandler(
            IUserManagementService userManagementService,
            IClientDocumentRepository documents,
            IFileStorage fileStorage,
            ICurrentUserService currentUserService,
            ILogger<DeleteUserHandler> logger)
        {
            _userManagementService = userManagementService;
            _documents = documents;
            _fileStorage = fileStorage;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task Handle(DeleteUserRequest request, CancellationToken ct)
        {
            // The easiest way to lose access to the admin panel is to delete yourself,
            // so that is forbidden explicitly.
            if (_currentUserService.UserId == request.Id)
                throw new BadRequestException("Не можна видалити власний акаунт.");

            // Document rows are removed by cascade along with the user, but the files
            // in storage are not: storage knows nothing about the database.
            // So the files go first and the account only afterwards.
            var storageKeys = await _documents.Query()
                .AsNoTracking()
                .Where(x => x.UserId == request.Id)
                .Select(x => x.StorageKey)
                .ToListAsync(ct);

            foreach (var key in storageKeys)
            {
                try
                {
                    await _fileStorage.DeleteAsync(key, ct);
                }
                catch (Exception ex)
                {
                    // The file may already be gone, or storage may be briefly unavailable.
                    // That must not block deleting the account, otherwise a junk user would
                    // stay forever. Log it and move on.
                    _logger.LogWarning(ex,
                        "Could not delete file {StorageKey} while removing user {UserId}.",
                        key, request.Id);
                }
            }

            await _userManagementService.DeleteAsync(request.Id, ct);

            _logger.LogInformation(
                "User {UserId} was deleted along with {FileCount} stored files.",
                request.Id, storageKeys.Count);
        }
    }
}
