using Accounting.Api.Common;
using Accounting.Api.Middlewares;
using Accounting.Api.Service;
using Accounting.Application;
using Accounting.Application.Abstractions.Identity;
using Accounting.Infrastructure;
using Accounting.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// On Render the app sits behind a proxy, so RemoteIpAddress would equal the
// proxy address for EVERY visitor. Without this, rate limiting would count
// everyone as one client and the first bot would lock the form for the rest.
// The real address arrives in X-Forwarded-For.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

    // By default the header is trusted only from loopback. Render's proxy
    // addresses are not known upfront, so the trusted list is cleared.
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Accounting.Api", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Paste ONLY the JWT token here (without 'Bearer ')."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var jwtSection = builder.Configuration.GetSection("Jwt");
var key = jwtSection["Key"];

if (string.IsNullOrWhiteSpace(key))
{
    throw new InvalidOperationException("JWT key is not configured.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(key)
),
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

// Rate limiting is attached as a NAMED policy, not globally: the admin panel
// and portal make dozens of calls and must not fall under a blanket limit.
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy(RateLimitPolicies.PublicClientRequests, httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            // The partition key is the client address: everyone gets their own window.
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(10),
                // No queue: an excess request is rejected immediately rather than held.
                QueueLimit = 0
            }));

    options.AddPolicy(RateLimitPolicies.PublicNewsletter, httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                // A newsletter signup is cheaper than a request, so the limit is softer.
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(10),
                QueueLimit = 0
            }));

    options.OnRejected = async (context, ct) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter =
                ((int)retryAfter.TotalSeconds).ToString(CultureInfo.InvariantCulture);
        }

        // The body uses the shape the frontend already reads via getApiErrorMessage;
        // otherwise the user would see a generic "could not send".
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            type = "too_many_requests",
            title = "Забагато спроб з вашої адреси. Спробуйте, будь ласка, за 10 хвилин.",
            status = StatusCodes.Status429TooManyRequests
        }, ct);
    };
});

var app = builder.Build();

// First in the pipeline: everything below must see the real client address.
app.UseForwardedHeaders();

app.UseCors("Frontend");

try
{
    using var scope = app.Services.CreateScope();

    var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
    await seeder.SeedAsync();
}
catch (Exception ex)
{
    // Seeding is a startup convenience, not the reason the app exists.
    // If the database is still waking up, better to start and serve requests:
    // roles and the admin are seeded idempotently, the next start finishes the job.
    app.Services
        .GetRequiredService<ILogger<Program>>()
        .LogError(ex, "Identity seeding failed on startup. The application will continue without it.");
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

// After routing: otherwise a policy attached to a specific endpoint by
// an attribute simply would not be found.
app.UseRateLimiter();

try
{
    app.MapControllers();
}
catch (ReflectionTypeLoadException ex)
{
    foreach (var e in ex.LoaderExceptions)
        Console.WriteLine(e?.ToString());
    throw;
}
app.Run();
