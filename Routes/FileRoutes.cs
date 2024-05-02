using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

public static class FileRoutes
{
    private static string _filePath = "Documents";

    public static WebApplication AddFileRoutes(this WebApplication app)
    {
        app.MapPost("/v1/files", ([FromForm] IFormFile file) =>
        {
            using (var stream = file.OpenReadStream()) // if run OpenReadStream multiple times, this is return a new stream, causing a issue.
            {
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    using (var fileStream = File.Create($"{_filePath}/{file.FileName}"))
                    {
                        stream.CopyTo(fileStream);
                    }

                    return Results.Created($"/files/{file.FileName}", file);
                }
            }

            return Results.BadRequest("Invalid file data");
        })
        .WithName("UploadFileAction")
        .DisableAntiforgery()
        .RequireAuthorization();

        app.MapGet("/v1/files", async ([FromQuery] string fileName) =>
        {
            byte[] buffer = new byte[16 * 1024];
            int bytesRead;
            List<string> result = new List<string>();

            Console.WriteLine($"{_filePath}/{fileName}");

            using (var stream = new FileStream($"./{_filePath}/{fileName}", FileMode.Open, FileAccess.Read))
            {
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    result.Add(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                }
            }

            // while ((bytesRead = file.OpenReadStream().ReadByte()) > 0)

            if (result is null || result.Count == 0)
                return Results.NotFound();

            var enumerator = result.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Console.WriteLine(enumerator.Current);
            }

            return Results.Ok(result);
        })
        .WithName("DownloadFileActionV1")
        .RequireAuthorization();

        app.MapGet("/v2/files", ([FromServices] IWebHostEnvironment env, [FromQuery] string fileName) =>
        {
            //env provide info about the web hosting env an app is running in. ContentRootPath returns the absolute path to the root directory of the project
            var filePath = Path.Combine(env.ContentRootPath, _filePath, fileName);

            Console.WriteLine(filePath);

            if(!File.Exists(filePath))
                return Results.NotFound();

            var provider = new FileExtensionContentTypeProvider();
            if(!provider.TryGetContentType(fileName, out var mimeType))
            {
                mimeType = "application/octet-stream"; // Default MIME type for unknown/other files
            }

            return Results.File(filePath, mimeType, fileName);
        })
        .WithName("DownloadFileActionV2")
        .RequireAuthorization();

        return app;
    }
}