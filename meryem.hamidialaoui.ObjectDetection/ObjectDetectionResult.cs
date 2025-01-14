namespace meryem.hamidialaoui.ObjectDetection;

public record ObjectDetectionResult
{
    public byte[] ImageData { get; set; }
    public IList<BoundingBox> Box { get; set; }
}

public record BoundingBox
{
    public BoundingBoxDimensions Dimensions { get; set; }
    public string Label { get; set; }
    public float Confidence { get; set; }
}

public record BoundingBoxDimensions
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
}
