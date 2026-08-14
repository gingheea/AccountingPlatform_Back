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

// На Render застосунок стоїть за проксі, тому RemoteIpAddress для ВСІХ
// відвідувачів дорівнював би адресі проксі. Без цього обмеження частоти
// рахувало б усіх як одного клієнта й після першого ж бота закрило форму
// для решти. Справжня адреса приходить у X-Forwarded-For.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

    // За замовчуванням заголовку довіряють лише від loopback. Адреси проксі
    // Render наперед невідомі, тому список довірених очищаємо.
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

// Обмеження частоти вішається ІМЕННОЮ політикою, а не глобально: адмінка
// й портал роблять десятки запитів і під загальний ліміт потрапляти не мають.
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy(RateLimitPolicies.PublicClientRequests, httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            // Ключ розділу — адреса клієнта: у кожного своє власне вікно.
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(10),
                // Черги немає: зайвий запит одразу відхиляємо, а не тримаємо.
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

        // Тіло у форматі, який фронт уже вміє читати через getApiErrorMessage,
        // інакше користувач побачив би загальне «не вдалося надіслати».
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            type = "too_many_requests",
            title = "Забагато заявок з вашої адреси. Спробуйте, будь ласка, за 10 хвилин.",
            status = StatusCodes.Status429TooManyRequests
        }, ct);
    };
});

var app = builder.Build();

// Найперше в конвеєрі: усе, що нижче, має бачити справжню адресу клієнта.
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
    // Сідер — допоміжна дія на старті, а не сенс існування застосунку.
    // Якщо база саме прокидається, краще піднятись і обслуговувати запити:
    // ролі й адмін створюються ідемпотентно, наступний старт досіє решту.
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

// Після маршрутизації — інакше політика, привʼязана до конкретного
// ендпоінта атрибутом, просто не буде знайдена.
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
