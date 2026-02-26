using OpenCvSharp;

namespace ROI_Report.Class
{
    /// <summary>
    /// ROI 그리기 모드
    /// </summary>
    public enum RoiShapeType
    {
        Rect,
        Circle
    }

    /// <summary>
    /// ROI(Region of Interest) 영역을 나타내는 클래스
    /// </summary>
    public class ROI
    {
        public Rect Region { get; set; }
        public int Id { get; set; }
        public RoiShapeType ShapeType { get; set; }
        public string Name => $"ROI_{Id}";

        /// <summary>
        /// Circle일 때 중심점
        /// </summary>
        public OpenCvSharp.Point Center => new(
            Region.X + Region.Width / 2,
            Region.Y + Region.Height / 2);

        /// <summary>
        /// Circle일 때 반지름
        /// </summary>
        public int Radius => Math.Min(Region.Width, Region.Height) / 2;

        public ROI(Rect region, int id, RoiShapeType shapeType = RoiShapeType.Rect)
        {
            Region = region;
            Id = id;
            ShapeType = shapeType;
        }

        public ROI(int x, int y, int width, int height, int id, RoiShapeType shapeType = RoiShapeType.Rect)
        {
            Region = new Rect(x, y, width, height);
            Id = id;
            ShapeType = shapeType;
        }
    }
}
