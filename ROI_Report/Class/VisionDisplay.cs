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

        /// <summary>
        /// 현재 소스 이미지
        /// </summary>
        public Mat? SourceImage => _sourceImage;

        /// <summary>
        /// 이미지를 설정하고 표시용 스케일/오프셋을 계산합니다.
        /// </summary>
        public void SetImage(Mat image, System.Drawing.Size displaySize)
        {
            _sourceImage = image;
            _displaySize = displaySize;
            CalculateScaleAndOffset();
        }

        /// <summary>
        /// 표시 영역 크기가 변경되었을 때 호출합니다.
        /// </summary>
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

        /// <summary>
        /// PictureBox(표시) 좌표를 이미지 좌표로 변환합니다.
        /// </summary>
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

        /// <summary>
        /// 이미지 좌표를 PictureBox(표시) 좌표로 변환합니다.
        /// </summary>
        public System.Drawing.Point ImageToDisplay(OpenCvSharp.Point imagePoint)
        {
            if (_scale <= 0)
                return new System.Drawing.Point(0, 0);

            int x = (int)(imagePoint.X * _scale + _offset.X);
            int y = (int)(imagePoint.Y * _scale + _offset.Y);
            return new System.Drawing.Point(x, y);
        }

        /// <summary>
        /// 이미지 Rect를 표시 좌표 Rect로 변환합니다.
        /// </summary>
        public Rect ImageRectToDisplay(Rect imageRect)
        {
            var topLeft = ImageToDisplay(imageRect.TopLeft);
            var bottomRight = ImageToDisplay(imageRect.BottomRight);
            int w = bottomRight.X - topLeft.X;
            int h = bottomRight.Y - topLeft.Y;
            return new Rect(topLeft.X, topLeft.Y, w, h);
        }

        /// <summary>
        /// 표시 좌표 Rect를 이미지 좌표 Rect로 변환합니다.
        /// </summary>
        public Rect DisplayRectToImage(Rectangle displayRect)
        {
            var topLeft = DisplayToImage(new System.Drawing.Point(displayRect.X, displayRect.Y));
            var bottomRight = DisplayToImage(new System.Drawing.Point(displayRect.Right, displayRect.Bottom));
            int w = bottomRight.X - topLeft.X;
            int h = bottomRight.Y - topLeft.Y;
            return new Rect(topLeft.X, topLeft.Y, w, h);
        }

        /// <summary>
        /// ROI가 표시 영역 내에 있는지 확인합니다.
        /// </summary>
        public bool IsPointInDisplayArea(System.Drawing.Point displayPoint)
        {
            if (_sourceImage == null)
                return false;

            var imgPoint = DisplayToImage(displayPoint);
            return imgPoint.X >= 0 && imgPoint.X < _sourceImage.Width &&
                   imgPoint.Y >= 0 && imgPoint.Y < _sourceImage.Height;
        }

        /// <summary>
        /// ROI를 포함한 이미지를 Bitmap으로 반환합니다.
        /// </summary>
        /// <param name="roiManager">ROI 관리자</param>
        /// <param name="selectionRect">드래그 중인 선택 영역 (빨간색)</param>
        /// <param name="selectedRoiId">리스트에서 선택된 ROI ID (강조 표시)</param>
        /// <param name="selectionShapeType">드래그 중 선택 영역의 모양</param>
        public Bitmap? GetDisplayBitmapWithROIs(ROIManager roiManager, Rect? selectionRect = null, int? selectedRoiId = null, RoiShapeType selectionShapeType = RoiShapeType.Rect)
        {
            if (_sourceImage == null || _sourceImage.Empty())
                return null;

            using var displayMat = _sourceImage.Clone();

            // 기존 ROI 그리기
            foreach (var roi in roiManager.ROIs)
            {
                DrawRoi(displayMat, roi, roi.Id == selectedRoiId);
            }

            // 선택 중인 영역 그리기 (드래그)
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

        /// <summary>
        /// ROI 없이 원본 이미지를 Bitmap으로 반환합니다.
        /// </summary>
        public Bitmap? GetDisplayBitmap(Mat? image = null)
        {
            var mat = image ?? _sourceImage;
            if (mat == null || mat.Empty())
                return null;

            return mat.ToBitmap();
        }
    }
}
