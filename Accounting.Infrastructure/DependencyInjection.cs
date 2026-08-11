using Accounting.Application.Abstractions.Auth;
using Accounting.Application.Abstractions.Identity;
using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Abstractions.Storage;
using Accounting.Infrastructure.Identity;
using Accounting.Infrastructure.Persistence;
using Accounting.Infrastructure.Repositories;
using Accounting.Infrastructure.Storage;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Accounting.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("Default");

        services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(cs));

        services.AddIdentityCore<AppUser>(opt =>
        {
            opt.User.RequireUniqueEmail = true;

            opt.Password.RequiredLength = 8;
            opt.Password.RequireDigit = true;
            opt.Password.RequireLowercase = true;
            opt.Password.RequireUppercase = true;
            opt.Password.RequireNonAlphanumeric = false;
        }).AddRoles<IdentityRole<Guid>>()
          .AddEntityFrameworkStores<AppDbContext>()
          .AddSignInManager<SignInManager<AppUser>>()
          .AddDefaultTokenProviders();

        services.Configure<JwtOptions>(config.GetSection("Jwt"));
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IdentitySeeder>();
        services.AddScoped<IIdentityAuthService, IdentityAuthService>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IClientRequestRepository, ClientRequestRepository>();
        services.AddScoped<IPricingPackageRepository, PricingPackageRepository>();
        services.AddScoped<IClientDocumentRepository, ClientDocumentRepository>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddStorage(config);

        return services;
    }

    private static IServiceCollection AddStorage(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<S3StorageOptions>(config.GetSection("Storage"));

        services.AddSingleton<IAmazonS3>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<S3StorageOptions>>().Value;

            if (string.IsNullOrWhiteSpace(options.ServiceUrl))
                throw new InvalidOperationException("Storage:ServiceUrl is not configured.");

            if (string.IsNullOrWhiteSpace(options.BucketName))
                throw new InvalidOperationException("Storage:BucketName is not configured.");

            var s3Config = new AmazonS3Config
            {
                ServiceURL = options.ServiceUrl,
                ForcePathStyle = options.ForcePathStyle,
                AuthenticationRegion = options.Region,

                // Cloudflare R2 rejects the flexible checksums the v4 SDK adds by default.
                RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
                ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED
            };

            var credentials = new BasicAWSCredentials(options.AccessKeyId, options.SecretAccessKey);

            return new AmazonS3Client(credentials, s3Config);
        });

        services.AddScoped<IFileStorage, S3FileStorage>();

        return services;
    }
}
