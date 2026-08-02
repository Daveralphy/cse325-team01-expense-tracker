using ExpenseTracker.Components;
using ExpenseTracker.Components.Account;
using ExpenseTracker.Data;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

/*
 * STORAGE CONFIGURATION
 *
 * Local development:
 *   Uses the project's Data folder.
 *
 * Azure production:
 *   Uses Azure App Service's persistent HOME directory.
 */
var storageRoot = builder.Environment.IsDevelopment()
    ? Path.Combine(builder.Environment.ContentRootPath, "Data")
    : Path.Combine(
        Environment.GetEnvironmentVariable("HOME")
            ?? builder.Environment.ContentRootPath,
        "data",
        "ExpenseTracker");

// Ensure the storage folder exists before SQLite or Data Protection uses it.
Directory.CreateDirectory(storageRoot);

var dataProtectionKeysPath = Path.Combine(
    storageRoot,
    "DataProtectionKeys");

Directory.CreateDirectory(dataProtectionKeysPath);

var databasePath = Path.Combine(
    storageRoot,
    "app.db");

var connectionString =
    $"Data Source={databasePath};Cache=Shared";

/*
 * FORWARDED HEADERS
 *
 * Azure App Service sits behind a proxy.
 * These settings allow ASP.NET Core to recognise the original
 * HTTPS request and host information.
 */
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto |
        ForwardedHeaders.XForwardedHost;

    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Add Razor Components and interactive server-side rendering.
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// Authentication state.
builder.Services.AddCascadingAuthenticationState();

/*
 * DATA PROTECTION
 *
 * Stores authentication and security keys in a persistent folder.
 * This helps authentication cookies remain valid after an Azure restart.
 */
builder.Services
    .AddDataProtection()
    .PersistKeysToFileSystem(
        new DirectoryInfo(dataProtectionKeysPath))
    .SetApplicationName("ExpenseTracker");

// Identity support services.
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();

builder.Services.AddScoped<
    AuthenticationStateProvider,
    IdentityRevalidatingAuthenticationStateProvider>();

// Application services.
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddScoped<ExpenseService>();
builder.Services.AddScoped<IncomeService>();

// Configure Identity authentication cookies.
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme =
            IdentityConstants.ApplicationScheme;

        options.DefaultSignInScheme =
            IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

/*
 * DATABASE
 *
 * SQLite remains the current database provider.
 * The database file is created in the persistent storage folder.
 */
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure ASP.NET Core Identity.
builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        // Email confirmation is not configured for this classroom project.
        options.SignIn.RequireConfirmedAccount = false;

        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// No real email provider is configured for this project.
builder.Services.AddSingleton<
    IEmailSender<ApplicationUser>,
    IdentityNoOpEmailSender>();

var app = builder.Build();

/*
 * DATABASE MIGRATION
 *
 * Creates or updates the SQLite database when the application starts.
 */
using (var scope = app.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider
        .GetRequiredService<
            IDbContextFactory<ApplicationDbContext>>();

    await using var dbContext =
        await dbFactory.CreateDbContextAsync();

    await dbContext.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler(
        "/Error",
        createScopeForErrors: true);

    app.UseHsts();
}

// Must be used before HTTPS redirection.
app.UseForwardedHeaders();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Identity account endpoints.
app.MapAdditionalIdentityEndpoints();

try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
    throw;
}