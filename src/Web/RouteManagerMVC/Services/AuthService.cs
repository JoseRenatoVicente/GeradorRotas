﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using RouteManager.Domain.Core.Identity.Extensions;
using RouteManager.Domain.Core.Services;
using RouteManager.Domain.Core.Services.Base;
using RouteManager.WebAPI.Core.Notifications;
using RouteManagerMVC.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RouteManagerMVC.Services;

public interface IAuthService
{
    Task<UserResponseLogin> LoginAsync(UserLogin userLogin);
    Task LogoutAsync();
    Task<UserResponseLogin> SaveTokenAsync(UserResponseLogin responseLogin);
}

public class AuthService : BaseService, IAuthService
{
    private readonly GatewayService _gatewayService;
    private readonly IAuthenticationService _authenticationService;
    private readonly IAspNetUser _aspNetUser;

    public AuthService(GatewayService gatewayService, IAuthenticationService authenticationService, IAspNetUser aspNetUser, INotifier notifier) : base(notifier)
    {
        _gatewayService = gatewayService;
        _authenticationService = authenticationService;
        _aspNetUser = aspNetUser;
    }

    public async Task<UserResponseLogin> LoginAsync(UserLogin userLogin)
    {
        var userResponseLogin = await _gatewayService.PostAsync<UserResponseLogin>("Identity/api/v1/Auth/Login", userLogin);
        return userResponseLogin == null ? null : await SaveTokenAsync(userResponseLogin);
    }

    public async Task<UserResponseLogin> SaveTokenAsync(UserResponseLogin responseLogin)
    {
        var claims = new List<Claim>
        {
            new("JWT", responseLogin.AccessToken),
            new("RefreshToken", responseLogin.RefreshToken.ToString()),
            new("Name", responseLogin.UserToken.Name)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
            IsPersistent = true
        };

        await _authenticationService.SignInAsync(
            _aspNetUser.GetHttpContext(),
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
        return responseLogin;
    }


    public async Task LogoutAsync()
    {
        await _authenticationService.SignOutAsync(
            _aspNetUser.GetHttpContext(),
            CookieAuthenticationDefaults.AuthenticationScheme,
            null);
    }

}