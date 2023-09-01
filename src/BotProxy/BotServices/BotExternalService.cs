using BotProxy.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
    private readonly ClientWebSocket _clientWebSocket;

    public BotExternalService(IHttpClientFactory httpClientFactory, IOptions<BotExternalServiceOptions> serviceOptions)
    {
        _httpClientFactory = httpClientFactory;
        _serviceOptions = serviceOptions.Value;
        _clientWebSocket = new ClientWebSocket();
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
        string socketUrl = "wss://directline.botframework.com/v3/directline/conversations/88LWOHkST1KF3WjX5IXFK3-in/stream?watermark=-&t=eyJhbGciOiJSUzI1NiIsImtpZCI6IjhoNW5lWE53VkhvUTdSMDdiYUhNWVZEY3ltcyIsIng1dCI6IjhoNW5lWE53VkhvUTdSMDdiYUhNWVZEY3ltcyIsInR5cCI6IkpXVCJ9.eyJib3QiOiJhenVyZS1uZXctdGVzdC1ib3QiLCJzaXRlIjoieWl0S1E2WEJENmciLCJjb252IjoiODhMV09Ia1NUMUtGM1dqWDVJWEZLMy1pbiIsIm5iZiI6MTY5MzMwNjE3NCwiZXhwIjoxNjkzMzA2MjM0LCJpc3MiOiJodHRwczovL2RpcmVjdGxpbmUuYm90ZnJhbWV3b3JrLmNvbS8iLCJhdWQiOiJodHRwczovL2RpcmVjdGxpbmUuYm90ZnJhbWV3b3JrLmNvbS8ifQ.gLruAF3jVyRsyRklYZD6dVqJlXMSNlKJOdgQClKz32ZTPUcs7HW2TQdUsYRk8oDmveUk3LHstE0pc3p_NGo63Z4aDtcyLGbRhm2KV6JqAGrXqX-fPjqI81rw97XENWkTTlIgXPhac6D_NIpAfA086iIJ7xYpYpxqsex5Z3dkTO2cMFmISOh2DeL6npqEVkZB-if3GhwObQTfed3re4jXey0ra9WJUQ8O3fh-DKxg7mdvwl0OXCWPsKzZuFHYTQWy-6ouV4a5CLei4IoW9cVgJdHSppVpjoJDISLOO4Kua8L0ZN9ht-lZDubIL5zzUpNQJcGRzuz0ky51SD3W4wHbzw"; // Replace with your actual WebSocket URL

        if (_clientWebSocket.State != WebSocketState.Closed)
        {
            var streamUri = new Uri(socketUrl);
            await _clientWebSocket.ConnectAsync(streamUri, CancellationToken.None);

            _ = ReceiveMessages();
        }

        return;
       
        
        //using (var client = new ClientWebSocket())
        //{
        //    try
        //    {
        //        await client.ConnectAsync(streamUri, CancellationToken.None);

        //        _ = ReceiveMessages();

        //        Console.WriteLine("WebSocket connection established. Press Enter to exit.");

        //        var buffer = new byte[1024];
        //        while (true)
        //        {
        //            var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        //            if (result.MessageType == WebSocketMessageType.Text)
        //            {
        //                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
        //                Console.WriteLine("Received: " + message);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error: " + ex.Message);
        //    }
        //}
    }

    private async Task ReceiveMessages()
    {
        byte[] buffer = new byte[1024];

        try
        {
            while (_clientWebSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received message: {receivedMessage}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket receive error: {ex.Message}");
        }
    }

    private async Task DisconnectWebSocket()
    {
        try
        {
            await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "User requested closure", CancellationToken.None);
            Console.WriteLine("WebSocket disconnected.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket disconnection error: {ex.Message}");
        }
    }

    public async Task<string?> SendActivity(string conversationId, string userId, string message, CancellationToken cancellationToken)
    {

        var requestData = new
        {
            locale = "en-EN",
            type = "message",
            from = new { id = userId },
            text = message
        };

        using (var httpClient = _httpClientFactory.CreateClient(WebClientNames))
        {
            // httpClient.BaseAddress = new Uri(_serviceOptions.BaseUrl);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _serviceOptions.AuthorizationToken);

            var postEndPoint = $"{_serviceOptions.BaseUrl}v3/directline/conversations/{conversationId}/activities";

            try
            {

                // Serialize the requestData object to JSON
                string jsonPayload = JsonConvert.SerializeObject(requestData);

                // Create a StringContent object with the JSON payload
                var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

                // Send the POST request with JSON data
                HttpResponseMessage response = await httpClient.PostAsync(postEndPoint, content);

                // Send the POST request with JSON data
                // HttpResponseMessage response = await httpClient.PostAsJsonAsync(postEndPoint, requestData, _jsonSerializerOptions);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content
                    return await response.Content.ReadAsStringAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
            }
        }

        return null;
    }

    public async Task<RootObject?> RetrieveActivities(string conversationId, CancellationToken cancellationToken)
    {

        using (var httpClient = _httpClientFactory.CreateClient(WebClientNames))
        {
            // httpClient.BaseAddress = new Uri(_serviceOptions.BaseUrl);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _serviceOptions.AuthorizationToken);

            var postEndPoint = $"{_serviceOptions.BaseUrl}v3/directline/conversations/{conversationId}/activities";

            try
            {

                // Send the POST request with JSON data
                HttpResponseMessage response = await httpClient.GetAsync(postEndPoint);

                // Send the POST request with JSON data
                // HttpResponseMessage response = await httpClient.PostAsJsonAsync(postEndPoint, requestData, _jsonSerializerOptions);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content
                    return await response.Content.ReadFromJsonAsync<RootObject>(_jsonSerializerOptions, cancellationToken);
                }
            }
            catch (Exception ex)
            {
            }
        }

        return null;
    }
}
