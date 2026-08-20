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
            // Найпростіший спосіб залишитися без доступу до адмінки — стерти
            // самого себе. Тому це заборонено окремо.
            if (_currentUserService.UserId == request.Id)
                throw new BadRequestException("Не можна видалити власний акаунт.");

            // Рядки документів приберуться каскадом разом із користувачем, а от
            // самі файли в сховищі — ні: сховище про базу нічого не знає.
            // Тому спершу прибираємо файли, і лише потім акаунт.
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
                    // Файл міг бути вже видалений або сховище тимчасово недоступне.
                    // Через це не блокуємо видалення акаунта — інакше зайвий
                    // користувач лишиться назавжди. Пишемо в лог і йдемо далі.
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
