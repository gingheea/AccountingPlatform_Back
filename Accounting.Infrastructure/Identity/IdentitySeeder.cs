using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Accounting.Infrastructure.Identity;

public sealed class IdentitySeeder
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<IdentitySeeder> _logger;
    private readonly IConfiguration _configuration;

    public IdentitySeeder(
        RoleManager<IdentityRole<Guid>> roleManager,
        UserManager<AppUser> userManager,
        ILogger<IdentitySeeder> logger,
        IConfiguration configuration)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SeedAsync()
    {
        const string adminRole = Roles.Admin;

        var adminEmail = _configuration["Seed:AdminEmail"];
        var adminPassword = _configuration["Seed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(adminEmail))
        {
            throw new InvalidOperationException("Seed:AdminEmail is not configured.");
        }

        if (string.IsNullOrWhiteSpace(adminPassword))
        {
            throw new InvalidOperationException("Seed:AdminPassword is not configured.");
        }

        if (!await _roleManager.RoleExistsAsync(adminRole))
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole<Guid>(adminRole));

            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Failed to create Admin role: " +
                    string.Join(", ", roleResult.Errors.Select(e => e.Description))
                );
            }

            _logger.LogInformation("Admin role created.");
        }

        var adminUser = await _userManager.FindByEmailAsync(adminEmail);

        if (adminUser is null)
        {
            adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var userResult = await _userManager.CreateAsync(adminUser, adminPassword);

            if (!userResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Failed to create Admin user: " +
                    string.Join(", ", userResult.Errors.Select(e => e.Description))
                );
            }

            _logger.LogInformation("Admin user created.");
        }

        if (!await _userManager.IsInRoleAsync(adminUser, adminRole))
        {
            var addToRoleResult = await _userManager.AddToRoleAsync(adminUser, adminRole);

            if (!addToRoleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Failed to assign Admin role: " +
                    string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))
                );
            }

            _logger.LogInformation("Admin user assigned to Admin role.");
        }
    }
}