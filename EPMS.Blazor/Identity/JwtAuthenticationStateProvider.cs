﻿using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EPMS.Client.Identity;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationState _anonymous;

    public JwtAuthenticationStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrWhiteSpace(token))
            return _anonymous;

        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt", "name", "role");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void NotifyUserAuthentication(string token)
    {
        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt", "name", "role");

        var authenticatedUser = new ClaimsPrincipal(identity);
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        var authState = Task.FromResult(_anonymous);
        NotifyAuthenticationStateChanged(authState);
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);

       
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonBytes);

        if (keyValuePairs != null)
        {
            foreach (var kvp in keyValuePairs)
            {
                if (kvp.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var element in kvp.Value.EnumerateArray())
                    {
                        claims.Add(new Claim(kvp.Key, element.ToString()));
                    }
                }
                else
                {
                    claims.Add(new Claim(kvp.Key, kvp.Value.ToString()));
                }
            }
        }
        return claims;
    }

    private byte[] ParseBase64WithoutPadding(string base64)
    {
     
        base64 = base64.Replace('-', '+').Replace('_', '/');

        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}