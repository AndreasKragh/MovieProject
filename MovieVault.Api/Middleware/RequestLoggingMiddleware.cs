namespace MovieVault.Api.Middleware;

/// <summary>
/// Logger alle POST-, PUT- og DELETE-requests (de requests, der ændrer data).
/// Loggen skrives via Serilog og ender i MongoDB (konfigureret i appsettings.json).
/// Hver log indeholder: HTTP-metode, request-sti, statuskode og tidspunkt.
/// </summary>
public class RequestLoggingMiddleware
{
    // Kun metoder, der ændrer data, skal logges (GET springes over)
    private static readonly HashSet<string> LoggedMethods =
        new(StringComparer.OrdinalIgnoreCase) { "POST", "PUT", "DELETE" };

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!LoggedMethods.Contains(context.Request.Method))
        {
            await _next(context);
            return;
        }

        // Hvis en uhåndteret undtagelse opstår, svarer serveren 500, så det er standardværdien
        var statusCode = StatusCodes.Status500InternalServerError;

        try
        {
            await _next(context);                   // kør resten af pipeline'en (controlleren osv.)
            statusCode = context.Response.StatusCode;
        }
        finally
        {
            // Tidspunktet (Timestamp) tilføjes automatisk af Serilog.
            // {HttpMethod}, {RequestPath} og {StatusCode} gemmes som separate felter i MongoDB.
            _logger.LogInformation(
                "{HttpMethod} {RequestPath} responded {StatusCode}",
                context.Request.Method,
                context.Request.Path.Value,
                statusCode);
        }
    }
}
