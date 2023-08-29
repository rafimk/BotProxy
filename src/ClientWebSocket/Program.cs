using System.Net.WebSockets;
using System.Text;

ClientWebSocket clientWebSocket = new ClientWebSocket();

while (true)
{
    Console.WriteLine("Press Enter to connect or disconnect (Q to quit):");
    var key = Console.ReadKey(intercept: true).Key;

    if (key == ConsoleKey.Q)
    {
        break;
    }

    if (clientWebSocket.State == WebSocketState.Closed)
    {
        await ConnectWebSocket();
    }
    else
    {
        await DisconnectWebSocket();
    }
}

if (clientWebSocket.State == WebSocketState.Open)
{
    await DisconnectWebSocket();
}

async Task ConnectWebSocket()
{
    string socketUrl = "wss://directline.botframework.com/v3/directline/conversations/88LWOHkST1KF3WjX5IXFK3-in/stream?watermark=-&t=eyJhbGciOiJSUzI1NiIsImtpZCI6IjhoNW5lWE53VkhvUTdSMDdiYUhNWVZEY3ltcyIsIng1dCI6IjhoNW5lWE53VkhvUTdSMDdiYUhNWVZEY3ltcyIsInR5cCI6IkpXVCJ9.eyJib3QiOiJhenVyZS1uZXctdGVzdC1ib3QiLCJzaXRlIjoieWl0S1E2WEJENmciLCJjb252IjoiODhMV09Ia1NUMUtGM1dqWDVJWEZLMy1pbiIsIm5iZiI6MTY5MzMwNjE3NCwiZXhwIjoxNjkzMzA2MjM0LCJpc3MiOiJodHRwczovL2RpcmVjdGxpbmUuYm90ZnJhbWV3b3JrLmNvbS8iLCJhdWQiOiJodHRwczovL2RpcmVjdGxpbmUuYm90ZnJhbWV3b3JrLmNvbS8ifQ.gLruAF3jVyRsyRklYZD6dVqJlXMSNlKJOdgQClKz32ZTPUcs7HW2TQdUsYRk8oDmveUk3LHstE0pc3p_NGo63Z4aDtcyLGbRhm2KV6JqAGrXqX-fPjqI81rw97XENWkTTlIgXPhac6D_NIpAfA086iIJ7xYpYpxqsex5Z3dkTO2cMFmISOh2DeL6npqEVkZB-if3GhwObQTfed3re4jXey0ra9WJUQ8O3fh-DKxg7mdvwl0OXCWPsKzZuFHYTQWy-6ouV4a5CLei4IoW9cVgJdHSppVpjoJDISLOO4Kua8L0ZN9ht-lZDubIL5zzUpNQJcGRzuz0ky51SD3W4wHbzw"; // Replace with your actual WebSocket URL

    try
    {
        await clientWebSocket.ConnectAsync(new Uri(socketUrl), CancellationToken.None);

        _ = ReceiveMessages();
        Console.WriteLine("WebSocket connected.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"WebSocket connection error: {ex.Message}");
    }
}

async Task DisconnectWebSocket()
{
    try
    {
        await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "User requested closure", CancellationToken.None);
        Console.WriteLine("WebSocket disconnected.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"WebSocket disconnection error: {ex.Message}");
    }
}

async Task ReceiveMessages()
{
    byte[] buffer = new byte[1024];

    try
    {
        while (clientWebSocket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

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
