﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Mvc.Filters;

namespace RouteManager.WebAPI.Core.Configuration;

public class SecurityHeadersAttribute : ActionFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options
        if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Type-Options"))
        {
            context.HttpContext.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        }

        // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options
        if (!context.HttpContext.Response.Headers.ContainsKey("X-Frame-Options"))
        {
            context.HttpContext.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
        }

        // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy
        var csp = "default-src 'self'; object-src 'none'; frame-ancestors 'none'; sandbox allow-forms allow-same-origin allow-scripts; base-uri 'self';";

        // once for standards compliant browsers
        if (!context.HttpContext.Response.Headers.ContainsKey("Content-Security-Policy"))
        {
            context.HttpContext.Response.Headers.Add("Content-Security-Policy", csp);
        }
        // and once again for IE
        if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Security-Policy"))
        {
            context.HttpContext.Response.Headers.Add("X-Content-Security-Policy", csp);
        }

        // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Referrer-Policy
        var referrer_policy = "no-referrer";
        if (!context.HttpContext.Response.Headers.ContainsKey("Referrer-Policy"))
        {
            context.HttpContext.Response.Headers.Add("Referrer-Policy", referrer_policy);
        }

    }
}