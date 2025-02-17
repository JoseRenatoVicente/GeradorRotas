﻿using System;
using System.Security.Claims;

namespace RouteManager.Domain.Core.Identity.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentException(nameof(principal));

        var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
        return claim?.Value;
    }

    public static string GetName(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentException(nameof(principal));

        var claim = principal.FindFirst("Name");
        return claim?.Value;
    }

    public static string GetUserEmail(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentException(nameof(principal));

        var claim = principal.FindFirst("email");
        return claim?.Value;
    }

    public static string GetToken(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentException(nameof(principal));

        var claim = principal.FindFirst("JWT");
        return claim?.Value;
    }

    public static string GetUserRefreshToken(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentException(nameof(principal));

        var claim = principal.FindFirst("RefreshToken");
        return claim?.Value;
    }
}