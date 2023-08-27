using Microsoft.Extensions.Options;
using System;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace BotProxy.BotServices;

public class BotExternalService : IBotExternalService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly BotExternalServiceOptions _serviceOptions;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private const string WebClientNames = "BotExternalClient";

    public BotExternalService(IHttpClientFactory httpClientFactory, IOptions<BotExternalServiceOptions> serviceOptions)
    {
        _httpClientFactory = httpClientFactory;
        _serviceOptions = serviceOptions.Value;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }
    public async Task<ConversationsResponse?> StartConversation(CancellationToken cancellationToken)
    {
        var jsonData = new
        {
        };

        using (var httpClient = _httpClientFactory.CreateClient(WebClientNames))
        {
            httpClient.BaseAddress = new Uri(_serviceOptions.BaseUrl);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _serviceOptions.AuthorizationToken);

            try
            {
                // Make a POST request with JSON data
                HttpResponseMessage response = await httpClient.PostAsJsonAsync("v3/directline/conversations", jsonData);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content
                    return await response.Content.ReadFromJsonAsync<ConversationsResponse>(_jsonSerializerOptions, cancellationToken);
                }
            }
            catch (Exception ex)
            {
            }
        }

        return null;

    }

    public async Task StartListener(string streamUrl, CancellationToken cancellationToken)
    {
        var streamUri = new Uri(streamUrl);
        using (var client = new ClientWebSocket())
        {
            try
            {
                await client.ConnectAsync(streamUri, CancellationToken.None);

                Console.WriteLine("WebSocket connection established. Press Enter to exit.");

                var buffer = new byte[1024];
                while (true)
                {
                    var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine("Received: " + message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }

    public async Task<string?> SendActivity(string conversationId, string userId, string message, CancellationToken cancellationToken)
    {

        SendActivityRequest request = new SendActivityRequest
        {
            Locale = "en-EN",
            Type = "message",
            From = new From { Id = userId },
            Text = message
        };

        string jsonData = JsonSerializer.Serialize(request, _jsonSerializerOptions);


        using (var httpClient = _httpClientFactory.CreateClient(WebClientNames))
        {
            httpClient.BaseAddress = new Uri(_serviceOptions.BaseUrl);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _serviceOptions.AuthorizationToken);

            var postEndPoint = $"v3/directline/conversations/{conversationId}/activities";

            try
            {
                // Make a POST request with JSON data
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(postEndPoint, jsonData);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content
                    return await response.Content.ReadFromJsonAsync<string>(_jsonSerializerOptions, cancellationToken);
                }
            }
            catch (Exception ex)
            {
            }
        }

        return null;
    }
}
