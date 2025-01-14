using System.Text.Json;
using meryem.hamidialaoui.ObjectDetection;

var sceneDirectory = args[0];
var sceneImages = Directory.GetFiles(sceneDirectory).Select(File.ReadAllBytes).ToList();

var detectionResults = await new ObjectDetection().DetectObjectInScenesAsync(sceneImages);

foreach (var result in detectionResults)
{
    Console.WriteLine(JsonSerializer.Serialize(result.Box));
}