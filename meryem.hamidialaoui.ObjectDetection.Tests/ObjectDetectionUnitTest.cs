namespace meryem.hamidialaoui.ObjectDetection.Tests;

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

public class ObjectDetectionUnitTest
{
    [Fact]
    public async Task ObjectShouldBeDetectedCorrectly()
    {
        var executingPath = GetExecutingPath();
        var imageScenesData = new List<byte[]>();
        foreach (var imagePath in Directory.EnumerateFiles(Path.Combine(executingPath, "Scenes")))
        {
            var imageBytes = await File.ReadAllBytesAsync(imagePath);
            imageScenesData.Add(imageBytes);
        }

        var detectObjectInScenesResults = await new ObjectDetection().DetectObjectInScenesAsync(imageScenesData);

        
        Assert.Equal("chair", detectObjectInScenesResults[0].Box[0].Label);
        Assert.InRange(detectObjectInScenesResults[0].Box[0].Confidence, 0.9, 1.0);
        Assert.InRange(detectObjectInScenesResults[0].Box[0].Dimensions.X, 90, 100);
        Assert.InRange(detectObjectInScenesResults[0].Box[0].Dimensions.Y, 130, 140);
        Assert.InRange(detectObjectInScenesResults[0].Box[0].Dimensions.Width, 220, 230);
        Assert.InRange(detectObjectInScenesResults[0].Box[0].Dimensions.Height, 260, 270);

        // Vérifie les propriétés du second résultat
        Assert.Equal("car", detectObjectInScenesResults[1].Box[0].Label);
        Assert.InRange(detectObjectInScenesResults[1].Box[0].Confidence, 0.9, 1.0);
        Assert.InRange(detectObjectInScenesResults[1].Box[0].Dimensions.X, 30, 40);
        Assert.InRange(detectObjectInScenesResults[1].Box[0].Dimensions.Y, 110, 120);
        Assert.InRange(detectObjectInScenesResults[1].Box[0].Dimensions.Width, 280, 290);
        Assert.InRange(detectObjectInScenesResults[1].Box[0].Dimensions.Height, 150, 160);

        Assert.Equal("dog", detectObjectInScenesResults[1].Box[2].Label);
        Assert.InRange(detectObjectInScenesResults[1].Box[2].Confidence, 0.4, 0.5);
        Assert.InRange(detectObjectInScenesResults[1].Box[2].Dimensions.X, -5, 0);
        Assert.InRange(detectObjectInScenesResults[1].Box[2].Dimensions.Y, 310, 320);
        Assert.InRange(detectObjectInScenesResults[1].Box[2].Dimensions.Width, 90, 100);
        Assert.InRange(detectObjectInScenesResults[1].Box[2].Dimensions.Height, 100, 110);
    }

    private static string GetExecutingPath()
    {
        var executingAssemblyPath = Assembly.GetExecutingAssembly().Location;
        var executingPath = Path.GetDirectoryName(executingAssemblyPath);
        return executingPath;
    }
}
