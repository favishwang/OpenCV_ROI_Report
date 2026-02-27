using System.Text.Json;
using System.Text.Json.Serialization;
using OpenCvSharp;

namespace ROI_Report.Class
{
    /// <summary>
    /// ROI 파일 저장/로드용 데이터 구조
    /// </summary>
    public class RoiFileData
    {
        [JsonPropertyName("rois")]
        public List<RoiItemData> Rois { get; set; } = new();

        public static RoiFileData FromROIManager(ROIManager manager)
        {
            var data = new RoiFileData();
            foreach (var roi in manager.ROIs)
            {
                data.Rois.Add(new RoiItemData
                {
                    X = roi.Region.X,
                    Y = roi.Region.Y,
                    Width = roi.Region.Width,
                    Height = roi.Region.Height,
                    ShapeType = roi.ShapeType.ToString()
                });
            }
            return data;
        }

        public List<(Rect Region, RoiShapeType ShapeType)> ToROIData()
        {
            var result = new List<(Rect, RoiShapeType)>();
            foreach (var item in Rois)
            {
                var shapeType = item.ShapeType.Equals("Circle", StringComparison.OrdinalIgnoreCase)
                    ? RoiShapeType.Circle
                    : RoiShapeType.Rect;
                result.Add((new Rect(item.X, item.Y, item.Width, item.Height), shapeType));
            }
            return result;
        }
    }

    public class RoiItemData
    {
        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("shapeType")]
        public string ShapeType { get; set; } = "Rect";
    }
}
