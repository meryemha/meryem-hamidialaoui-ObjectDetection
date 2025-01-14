using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using meryem.hamidialaoui.ObjectDetection;
using System.Text.Json;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/results/{fileName}", (string fileName) =>
{
    var resultsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Results");
    var filePath = Path.Combine(resultsDirectory, fileName);

    if (!File.Exists(filePath))
    {
        return Results.NotFound(new { error = "Fichier introuvable." });
    }

    return Results.File(filePath, "image/jpeg");
});


app.MapPost("/detect-objects", async (HttpContext context) =>
{
    try
    {
        var form = await context.Request.ReadFormAsync();
        var sceneImages = form.Files.GetFiles("sceneImages");

        if (sceneImages == null || sceneImages.Count == 0)
        {
            return Results.BadRequest(new { error = "Veuillez fournir au moins une image de scène." });
        }

        var imageDataList = new List<byte[]>();
        foreach (var file in sceneImages)
        {
            using var stream = file.OpenReadStream();
            var imageBytes = new byte[file.Length];
            await stream.ReadAsync(imageBytes);
            imageDataList.Add(imageBytes);
        }

        var objectDetection = new ObjectDetection();
        var detectionResults = await objectDetection.DetectObjectInScenesAsync(imageDataList);

        var resultsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Results");
        if (!Directory.Exists(resultsDirectory))
        {
            Directory.CreateDirectory(resultsDirectory);
        }

        for (int i = 0; i < detectionResults.Count; i++)
        {
            var result = detectionResults[i];
            var outputFilePath = Path.Combine(resultsDirectory, $"image_{i + 1}_annotated.jpg");

            using var image = System.Drawing.Image.FromStream(new MemoryStream(result.ImageData));
            using var graphics = System.Drawing.Graphics.FromImage(image);

            foreach (var box in result.Box)
            {
                var pen = new System.Drawing.Pen(System.Drawing.Brushes.Red, 2);
                graphics.DrawRectangle(pen, box.Dimensions.X, box.Dimensions.Y, box.Dimensions.Width, box.Dimensions.Height);
                graphics.DrawString(box.Label, new System.Drawing.Font("Arial", 12), System.Drawing.Brushes.Red, box.Dimensions.X, box.Dimensions.Y - 20);
            }

            image.Save(outputFilePath);
        }

        return Results.Json(new { message = "Images annotées générées dans le dossier Results." });

    }
    catch (Exception ex)
    {
        return Results.Json(new { error = "Une erreur est survenue", details = ex.Message }, statusCode: 500);
    }
})
.WithOpenApi(operation =>
{
    operation.RequestBody = new Microsoft.OpenApi.Models.OpenApiRequestBody
    {
        Content = new Dictionary<string, Microsoft.OpenApi.Models.OpenApiMediaType>
        {
            ["multipart/form-data"] = new Microsoft.OpenApi.Models.OpenApiMediaType
            {
                Schema = new Microsoft.OpenApi.Models.OpenApiSchema
                {
                    Type = "object",
                    Properties = new Dictionary<string, Microsoft.OpenApi.Models.OpenApiSchema>
                    {
                        ["sceneImages"] = new Microsoft.OpenApi.Models.OpenApiSchema
                        {
                            Type = "array",
                            Items = new Microsoft.OpenApi.Models.OpenApiSchema { Type = "string", Format = "binary" }
                        }
                    },
                    Required = new HashSet<string> { "sceneImages" }
                }
            }
        }
    };
    return operation;
});


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
