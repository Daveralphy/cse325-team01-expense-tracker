using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace ExpenseTracker.Services;

/// <summary>
/// Provides the stable Identity user ID used to scope application data to the
/// currently authenticated user.
/// </summary>
public sealed class CurrentUserService(AuthenticationStateProvider authenticationStateProvider)
{
    public async Task<string?> GetUserIdAsync()
    {
        var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
        return authenticationState.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public async Task<string> GetRequiredUserIdAsync()
    {
        return await GetUserIdAsync()
            ?? throw new InvalidOperationException("An authenticated user is required.");
    }
}
