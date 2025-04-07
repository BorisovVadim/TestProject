using Microsoft.IO;
using System.Text;
using TestProject.Data;
using TestProject.Models;

namespace TestProject.Services;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager = new();

    public LoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, AppDbContext db)
    {
        var requestBody = await ReadRequestBody(context);
        var originalResponseBodyStream = context.Response.Body;
        using var responseBody = _recyclableMemoryStreamManager.GetStream();
        context.Response.Body = responseBody;

        await _next(context);

        var responseText = await ReadResponseBody(context.Response);

        var logEntry = new LogEntry
        {
            Method = context.Request.Method,
            Path = context.Request.Path,
            RequestBody = requestBody,
            ResponseBody = responseText
        };

        db.LogEntries.Add(logEntry);
        await db.SaveChangesAsync();

        await responseBody.CopyToAsync(originalResponseBodyStream);
    }

    /// <summary>
    /// Метод позволяет читать тело запроса несколько раз. 
    /// Оставляет тело запроса открытым после прочтения.
    /// После прочтения возвращает курсор в начало, чтобы оно не было пустым.
    /// </summary>
    private async Task<string> ReadRequestBody(HttpContext context)
    {
        context.Request.EnableBuffering();
        using var streamReader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await streamReader.ReadToEndAsync();
        context.Request.Body.Position = 0;
        return body;
    }

    /// <summary>
    /// Метод утсанавливает позицию потока в начало для прочтения ответа.
    /// Считывает весь текст из тела ответа.
    /// Возвращает курсор в начало для отправки тела ответа клиенту.
    /// </summary>
    private async Task<string> ReadResponseBody(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var text = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);
        return text;
    }
}
