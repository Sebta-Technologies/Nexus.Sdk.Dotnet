﻿using Microsoft.Extensions.Logging;
using Nexus.SDK.Shared.ErrorHandling;
using Nexus.SDK.Shared.Responses;
using System.Net;

namespace Nexus.SDK.Shared.Http;

public class NexusResponseHandler : IResponseHandler
{
    private readonly ILogger? _logger;

    public NexusResponseHandler(ILogger? logger = null)
    {
        _logger = logger;
    }

    public async Task<T> HandleResponse<T>(HttpResponseMessage response) where T : class
    {
        var statusCode = response.StatusCode;
        var content = await response.Content.ReadAsStringAsync();

        _logger?.LogDebug("{statusCode} Response: {content}", statusCode, content);

        var responseObj = JsonSingleton.GetInstance<NexusResponse<T>>(content);

        if (responseObj == null)
        {
            throw new NexusApiException((int)statusCode, "Unable to parse Nexus response to JSON", null);
        }
;
        if ((int)statusCode >= 300)
        {
            _logger?.LogError("{statusCode} Response: {content}", statusCode, content);

            if (statusCode == HttpStatusCode.Unauthorized)
            {
                _logger?.LogWarning("Did you configure your authentication provider using ConnectTo");
            }

            throw new NexusApiException((int)statusCode, responseObj.Message, responseObj.Errors);
        }

        return responseObj.Values;
    }

    public async Task HandleResponse(HttpResponseMessage response)
    {
        var statusCode = response.StatusCode;

        if ((int)statusCode >= 300)
        {
            if (statusCode == HttpStatusCode.Unauthorized)
            {
                _logger?.LogWarning("Did you configure your authentication provider using ConnectTo?");
            }

            var content = await response.Content.ReadAsStringAsync();
            _logger?.LogError("Response: {content}", content);

            var responseObj = JsonSingleton.GetInstance<NexusResponse>(content);

            if (responseObj == null)
            {
                throw new NexusApiException((int)statusCode, "Unable to parse Nexus response to JSON", null);
            }
;
            throw new NexusApiException((int)statusCode, responseObj.Message, responseObj.Errors);
        }

        _logger?.LogDebug("{statusCode} Response", statusCode);
    }
}
