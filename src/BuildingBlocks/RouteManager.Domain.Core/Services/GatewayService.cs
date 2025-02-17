﻿using RouteManager.Domain.Core.Extensions;
using RouteManager.Domain.Core.Identity.Extensions;
using RouteManager.Domain.Core.Services.Base;
using RouteManager.WebAPI.Core.Notifications;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace RouteManager.Domain.Core.Services;

public class GatewayService : BaseService
{
    private readonly IAspNetUser _aspNetUser;
    private readonly HttpClient _httpClient;

    public GatewayService(HttpClient httpClient, IAspNetUser aspNetUser, INotifier notifier) : base(notifier)
    {
        _httpClient = httpClient;
        _aspNetUser = aspNetUser;
    }

    public async Task<T> GetFromJsonAsync<T>(string path)
    {
        try
        {
            using var response = await GetAsync(path);

            if (response.IsSuccessStatusCode)
                return await DeserializeObjectResponse<T>(response);

            await ErrorsResponse(response);

            return default;
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            Notification("API Unavailable try again later");
            return default;
        }
    }

    private async Task<HttpResponseMessage> GetAsync(string path)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _aspNetUser.GetToken());
        return await _httpClient.GetAsync(path);
    }

    public async Task<T> PostAsync<T>(string path, object content)
    {
        return await DeserializeObjectResponse<T>(await PostAsync(path, content));
    }

    public async Task<T> PostAsync<T>(string path, MultipartFormDataContent content)
    {
        return await DeserializeObjectResponse<T>(await PostAsync(path, content));
    }

    private async Task<HttpResponseMessage> PostAsync(string path, MultipartFormDataContent content)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _aspNetUser.GetToken());

        var response = await _httpClient.PostAsync(path, content);
        await ErrorsResponse(response);

        return response;
    }

    public async Task<HttpResponseMessage> PostAsync(string path, object content)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _aspNetUser.GetToken());

        var response = await _httpClient.PostAsJsonAsync(path, content);
        await ErrorsResponse(response);

        return response;
    }

    public async Task<T> PutAsync<T>(string path, object content)
    {
        return await DeserializeObjectResponse<T>(await PutAsync(path, content));
    }

    public async Task<HttpResponseMessage> PutAsync(string path, object content)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _aspNetUser.GetToken());

        var response = await _httpClient.PutAsJsonAsync(path, content);
        await ErrorsResponse(response);

        return response;
    }

    public async Task<T> DeleteAsync<T>(string path)
    {
        return await DeserializeObjectResponse<T>(await DeleteAsync(path));
    }

    public async Task<HttpResponseMessage> DeleteAsync(string path)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _aspNetUser.GetToken());

        var response = await _httpClient.DeleteAsync(path);
        await ErrorsResponse(response);

        return response;
    }


    private async Task ErrorsResponse(HttpResponseMessage response)
    {
        switch ((int)response.StatusCode)
        {
            case 400:
                foreach (var item in (await DeserializeObjectResponse<ErrorResult>(response)).Errors)
                    Notification(item);
                return;

            case 401:
            case 403:
            case 404:
            case 500:
                throw new CustomHttpRequestException(response.StatusCode);
        }

        response.EnsureSuccessStatusCode();
    }

    public async Task<T> DeserializeObjectResponse<T>(HttpResponseMessage responseMessage)
    {
        var jsonStream = await responseMessage.Content.ReadAsStreamAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        try
        {
            return await JsonSerializer.DeserializeAsync<T>(jsonStream, options);
        }
        catch
        {
            return default;
        }
    }

    private sealed record ErrorResult
    {
        public ErrorResult(IEnumerable<string> errors, bool success)
        {
            Errors = errors;
            Success = success;
        }

        public bool Success { get; }
        public IEnumerable<string> Errors { get; }
    }
}

