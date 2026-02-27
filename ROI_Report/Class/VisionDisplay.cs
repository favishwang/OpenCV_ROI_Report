using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace ROI_Report.Class
{
    /// <summary>
    /// 비전 관련 표시 및 좌표 변환을 담당하는 클래스
    /// </summary>
    public class VisionDisplay
    {
        private Mat? _sourceImage;
        private System.Drawing.Size _displaySize;
        private double _scale;
        private System.Drawing.Point _offset;

        public Mat? SourceImage => _sourceImage;

        public void SetImage(Mat image, System.Drawing.Size displaySize)
        {
            _sourceImage = image;
            _displaySize = displaySize;
            CalculateScaleAndOffset();
        }

        public void UpdateDisplaySize(System.Drawing.Size displaySize)
        {
            _displaySize = displaySize;
            if (_sourceImage != null)
                CalculateScaleAndOffset();
        }

        private void CalculateScaleAndOffset()
        {
            if (_sourceImage == null || _displaySize.Width <= 0 || _displaySize.Height <= 0)
                return;

            double scaleX = (double)_displaySize.Width / _sourceImage.Width;
            double scaleY = (double)_displaySize.Height / _sourceImage.Height;
            _scale = Math.Min(scaleX, scaleY);

            int scaledWidth = (int)(_sourceImage.Width * _scale);
            int scaledHeight = (int)(_sourceImage.Height * _scale);
            _offset = new System.Drawing.Point(
                (_displaySize.Width - scaledWidth) / 2,
                (_displaySize.Height - scaledHeight) / 2);
        }

        public OpenCvSharp.Point DisplayToImage(System.Drawing.Point displayPoint)
        {
            if (_sourceImage == null || _scale <= 0)
                return new OpenCvSharp.Point(0, 0);

            int x = (int)((displayPoint.X - _offset.X) / _scale);
            int y = (int)((displayPoint.Y - _offset.Y) / _scale);

            // 범위 제한
            x = Math.Clamp(x, 0, _sourceImage.Width - 1);
            y = Math.Clamp(y, 0, _sourceImage.Height - 1);

            return new OpenCvSharp.Point(x, y);
        }
        
        public System.Drawing.Point ImageToDisplay(OpenCvSharp.Point imagePoint)
        {
            if (_scale <= 0)
                return new System.Drawing.Point(0, 0);

            int x = (int)(imagePoint.X * _scale + _offset.X);
            int y = (int)(imagePoint.Y * _scale + _offset.Y);
            return new System.Drawing.Point(x, y);
        }

        public Rect ImageRectToDisplay(Rect imageRect)
        {
            var topLeft = ImageToDisplay(imageRect.TopLeft);
            var bottomRight = ImageToDisplay(imageRect.BottomRight);
            int w = bottomRight.X - topLeft.X;
            int h = bottomRight.Y - topLeft.Y;
            return new Rect(topLeft.X, topLeft.Y, w, h);
        }

        public Rect DisplayRectToImage(Rectangle displayRect)
        {
            var topLeft = DisplayToImage(new System.Drawing.Point(displayRect.X, displayRect.Y));
            var bottomRight = DisplayToImage(new System.Drawing.Point(displayRect.Right, displayRect.Bottom));
            int w = bottomRight.X - topLeft.X;
            int h = bottomRight.Y - topLeft.Y;
            return new Rect(topLeft.X, topLeft.Y, w, h);
        }

        public bool IsPointInDisplayArea(System.Drawing.Point displayPoint)
        {
            if (_sourceImage == null)
                return false;

            var imgPoint = DisplayToImage(displayPoint);
            return imgPoint.X >= 0 && imgPoint.X < _sourceImage.Width &&
                   imgPoint.Y >= 0 && imgPoint.Y < _sourceImage.Height;
        }

        public Bitmap? GetDisplayBitmapWithROIs(ROIManager roiManager, Rect? selectionRect = null, int? selectedRoiId = null, RoiShapeType selectionShapeType = RoiShapeType.Rect)
        {
            if (_sourceImage == null || _sourceImage.Empty())
                return null;

            using var displayMat = _sourceImage.Clone();

            foreach (var roi in roiManager.ROIs)
            {
                DrawRoi(displayMat, roi, roi.Id == selectedRoiId);
            }

            if (selectionRect.HasValue && selectionRect.Value.Width > 0 && selectionRect.Value.Height > 0)
            {
                if (selectionShapeType == RoiShapeType.Circle)
                {
                    var center = new OpenCvSharp.Point(selectionRect.Value.X + selectionRect.Value.Width / 2, selectionRect.Value.Y + selectionRect.Value.Height / 2);
                    var radius = Math.Min(selectionRect.Value.Width, selectionRect.Value.Height) / 2;
                    if (radius > 0)
                        Cv2.Circle(displayMat, center, radius, Scalar.Red, 2);
                }
                else
                {
                    Cv2.Rectangle(displayMat, selectionRect.Value, Scalar.Red, 2);
                }
            }

            return displayMat.ToBitmap();
        }

        private static void DrawRoi(Mat mat, ROI roi, bool isSelected)
        {
            var color = isSelected ? Scalar.Cyan : Scalar.Green;
            int thickness = isSelected ? 4 : 2;

            if (roi.ShapeType == RoiShapeType.Circle && roi.Radius > 0)
            {
                Cv2.Circle(mat, roi.Center, roi.Radius, color, thickness);
                Cv2.PutText(mat, roi.Name, new OpenCvSharp.Point(roi.Region.X, roi.Region.Y - 5),
                    HersheyFonts.HersheySimplex, 0.6, color, 1);
            }
            else
            {
                Cv2.Rectangle(mat, roi.Region, color, thickness);
                Cv2.PutText(mat, roi.Name, roi.Region.TopLeft,
                    HersheyFonts.HersheySimplex, 0.6, color, 1);
            }
        }

        public Bitmap? GetDisplayBitmap(Mat? image = null)
        {
            var mat = image ?? _sourceImage;
            if (mat == null || mat.Empty())
                return null;

            return mat.ToBitmap();
        }
    }
}
