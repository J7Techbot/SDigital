using api.Backend;
using api.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        // Improvements Needed:
        // 1. Error Handling
        // 2. Logging      
        // 3. Security        
        // 4. Endpoint Organization        
        // 5. Request Validation        
        // 6. Exception Handling

        //This is most simple example.
        
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton<CoordinatesService>();
        builder.Services.AddSingleton<SignalsService>();
        builder.Services.AddSingleton<SignalsProvider>();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.WithOrigins("http://localhost:8080")
                      .AllowAnyMethod() 
                      .AllowAnyHeader(); 
            });
        });

        var app = builder.Build();

        app.UseCors("AllowAll");
        app.UseWebSockets();
        app.MapGet("/ws/coordinates", async (HttpContext context, CoordinatesService coordinatesService) =>
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await coordinatesService.StreamCoordinates(webSocket);
            }
            else
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Expected a WebSocket request");
            }
        });
        app.MapGet("/getSignals", async (SignalsService signalsService) =>
        {
            var signals = await signalsService.GetSignalsAsync();

            return Results.Ok(signals);
        });

        app.Run();
    }
}