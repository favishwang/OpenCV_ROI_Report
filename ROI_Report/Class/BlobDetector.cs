using OpenCvSharp;

namespace ROI_Report.Class
{
    /// <summary>
    /// 이미지에서 Blob을 검출하여 ROI 영역을 반환하는 클래스
    /// OpenCV Simple Detector가 아닌 엣지를 검출하고 그냥 영역 계산해서 찾게끔 임시 구현
    /// 아직 Blob의 개념에 대해서 체득하지 못함. -> 학습 필요
    /// </summary>
    public class BlobDetector
    {
        public int MinArea { get; set; } = 100;

        public int MaxArea { get; set; } = 0;

        public int ThresholdValue { get; set; } = 127;

        public List<Rect> Detect(Mat image)
        {
            if (image == null || image.Empty())
                return new List<Rect>();

            Mat gray = image.Channels() == 3 ? new Mat() : image.Clone();
            if (image.Channels() == 3)
                Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);

            using (gray)
            using (var binary = new Mat())
            {
                Cv2.Threshold(gray, binary, ThresholdValue, 255, ThresholdTypes.Binary);
                Cv2.FindContours(binary, out var contours, out _, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

                var result = new List<Rect>();
                foreach (var contour in contours)
                {
                    var area = Cv2.ContourArea(contour);
                    if (area < MinArea)
                        continue;
                    if (MaxArea > 0 && area > MaxArea)
                        continue;

                    var rect = Cv2.BoundingRect(contour);
                    if (rect.Width >= 2 && rect.Height >= 2)
                        result.Add(rect);
                }

                return result;
            }
        }
    }
}
