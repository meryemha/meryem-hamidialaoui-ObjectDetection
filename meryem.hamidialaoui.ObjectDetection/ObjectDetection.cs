using meryem.hamidialaoui.ObjectDetection.YoloNamespace;

namespace meryem.hamidialaoui.ObjectDetection;

public class ObjectDetection
{
    public async Task<IList<ObjectDetectionResult>> DetectObjectInScenesAsync(IList<byte[]> imagesSceneData)
    {
        var results = new List<ObjectDetectionResult>();
        var tinyYolo = new Yolo();

        await Task.WhenAll(imagesSceneData.Select(async image =>
        {
            // Appelle Detect pour traiter l'image
            var yoloOutput = tinyYolo.Detect(image);
            
            // Ajoute le résultat à la liste
            results.Add(new ObjectDetectionResult
            {
                ImageData = yoloOutput.ImageData, // Données de l'image traitée
                Box = yoloOutput.Boxes.Select(box => new meryem.hamidialaoui.ObjectDetection.BoundingBox
                {
                    Confidence = box.Confidence,
                    Label = box.Label,
                    Dimensions = new meryem.hamidialaoui.ObjectDetection.BoundingBoxDimensions
                    {
                        X = box.Dimensions.X,
                        Y = box.Dimensions.Y,
                        Width = box.Dimensions.Width,
                        Height = box.Dimensions.Height
                    }
                }).ToList()
            });
        }));

        return results;
    }


    
}
