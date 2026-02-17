using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Accounting.Infrastructure.Identity;

public sealed class IdentitySeeder
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly ILogger<IdentitySeeder> _logger;

    public IdentitySeeder(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        ILogger<IdentitySeeder> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        const string adminRole = "Admin";
        const string adminEmail = "admin@accounting.local";
        const string adminPassword = "Admin1234!";

        if (!await _roleManager.RoleExistsAsync(adminRole))
        {
            await _roleManager.CreateAsync(
                new IdentityRole<Guid>(adminRole)
            );

            _logger.LogInformation("Admin role created");
        }

        var user = await _userManager.FindByEmailAsync(adminEmail);

        if (user is null)
        {
            user = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, adminPassword);

            if (!result.Succeeded)
            {
                throw new Exception(
                    "Failed to create admin user: " +
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );
            }

            _logger.LogInformation("Admin user created");
        }

        if (!await _userManager.IsInRoleAsync(user, adminRole))
        {
            await _userManager.AddToRoleAsync(user, adminRole);
            _logger.LogInformation("Admin user assigned to Admin role");
        }
    }
}
