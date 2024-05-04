using System.Net.WebSockets;
using api.Backend;

namespace api.Services
{
    /// <summary>
    /// Coordinates endpoint "controller"
    /// </summary>
    public class CoordinatesService
    {
        /// <summary>
        /// Streams coordinates to client.
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public async Task StreamCoordinates(WebSocket socket)
        {
            //Here, a call to another container/service should be made to keep the API separated.
            //For the sake of saving time, I will replace it with a class and a loop.
            using (var cts = new CancellationTokenSource())
            {
                CancellationToken ct = cts.Token;

                try
                {
                    var task = Task.Run(async () =>
                    {
                        await foreach (var coords in new CoordinatesProvider().GetCoordinatesBufferAsync(ct))
                        {
                            //Send back streamed coords to client
                            await socket.SendAsync(
                            new ArraySegment<byte>(coords, 0, coords.Length),
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None);
                        }                        
                    });
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Socket disconnected");
                }

                while (socket.State == WebSocketState.Open)
                {
                    //Wait for close message from client
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(new byte[1024 * 2]), new CancellationTokenSource().Token);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                        cts.Cancel();
                        break;
                    }
                }
            }
        }
    }
}
