using Anek._365.Application.Abstractions.Queries;
using Anek._365.Application.Abstractions.Repositories;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Anek._365.Infrastructure.Authentication;

public class AuthenticationRepository : IAuthenticationRepository
{
    private const string SignInWithPassword = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword";

    private const string SignInWithCustomToken =
        "https://identitytoolkit.googleapis.com/v1/accounts:signInWithCustomToken";

    private readonly IOptions<AuthenticationConfiguration> _options;

    public AuthenticationRepository(IOptions<AuthenticationConfiguration> options)
    {
        _options = options;
    }

    public async Task<Register.Result> Register(string email, string password, CancellationToken cancellationToken)
    {
        var userArgs = new UserRecordArgs()
        {
            Email = email,
            Password = password,
        };

        try
        {
            await FirebaseAuth.DefaultInstance.CreateUserAsync(userArgs, cancellationToken);
        }
        catch (FirebaseException)
        {
            return new Register.Result.Failure("Email is used");
        }

        return new Register.Result.Success();
    }

    public async Task<string?> Login(string email, string password, CancellationToken cancellationToken)
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri($"{SignInWithPassword}?key={_options.Value.Key}"),
        };

        var request = new
        {
            email,
            password,
            returnSecureToken = true,
        };

        HttpResponseMessage response = await client
            .PostAsJsonAsync(
                string.Empty,
                request,
                cancellationToken);

        if (response.StatusCode is HttpStatusCode.BadRequest)
            return null;

        AuthToken? uid = await response.Content
            .ReadFromJsonAsync<AuthToken>(cancellationToken);

        if (uid is null)
            return null;

        string token = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(
            uid.LocalId,
            new Dictionary<string, object>() { { "Email", request.email } },
            cancellationToken);

        var client1 = new HttpClient
        {
            BaseAddress = new Uri($"{SignInWithCustomToken}?key={_options.Value.Key}"),
        };

        var request1 = new
        {
            token,
            returnSecureToken = true,
        };

        HttpResponseMessage result1 =
            await client1.PostAsJsonAsync(string.Empty, request1, cancellationToken: cancellationToken);
        AuthToken? authToken = await result1.Content.ReadFromJsonAsync<AuthToken>(cancellationToken: cancellationToken);

        return authToken?.IdToken;
    }

#pragma warning disable SA1134
    private class AuthToken
    {
        [JsonPropertyName("kind")] public string Kind { get; set; } = null!;

        [JsonPropertyName("localId")] public string LocalId { get; set; } = null!;

        [JsonPropertyName("email")] public string Email { get; set; } = null!;

        [JsonPropertyName("displayName")] public string DisplayName { get; set; } = null!;

        [JsonPropertyName("idToken")] public string IdToken { get; set; } = null!;

        [JsonPropertyName("registered")] public bool Registered { get; set; }

        [JsonPropertyName("refreshToken")] public string RefreshToken { get; set; } = null!;

        [JsonPropertyName("expiresIn")] public long ExpiresIn { get; set; }
    }
}