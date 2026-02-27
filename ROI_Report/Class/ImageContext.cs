using OpenCvSharp;

namespace ROI_Report.Class
{
    /// <summary>
    /// 이미지 및 수정 관련 속성을 관리하는 클래스
    /// </summary>
    public class ImageContext
    {
        private Mat? _originalImage;
        private Mat? _currentImage;
        private int _thresholdValue = 127;

        /// <summary>
        /// 원본 이미지 (로드된 이미지, 수정 전)
        /// </summary>
        public Mat? OriginalImage => _originalImage;

        /// <summary>
        /// 현재 표시 이미지 (그레이/컬러/Threshold 등 적용된 결과)
        /// </summary>
        public Mat? CurrentImage => _currentImage;

        /// <summary>
        /// Threshold 값 (0-255)
        /// </summary>
        public int ThresholdValue
        {
            get => _thresholdValue;
            set => _thresholdValue = Math.Clamp(value, 0, 255);
        }

        /// <summary>
        /// 이미지 로드 여부
        /// </summary>
        public bool HasImage => _originalImage != null && !_originalImage.Empty();

        /// <summary>
        /// 새 이미지를 로드합니다.
        /// </summary>
        public void Load(Mat image)
        {
            if (image == null || image.Empty())
                return;

            _originalImage?.Dispose();
            _currentImage?.Dispose();

            _originalImage = image.Clone();
            _currentImage = _originalImage.Clone();
            _thresholdValue = 127;
        }

        /// <summary>
        /// 그레이스케일로 변환합니다.
        /// </summary>
        public void ApplyGrayscale()
        {
            if (_originalImage == null || _originalImage.Empty())
                return;

            _currentImage?.Dispose();
            using var gray = new Mat();
            Cv2.CvtColor(_originalImage, gray, ColorConversionCodes.BGR2GRAY);
            _currentImage = new Mat();
            Cv2.CvtColor(gray, _currentImage, ColorConversionCodes.GRAY2BGR);
            _thresholdValue = 127;
        }

        /// <summary>
        /// 원본 컬러로 복원합니다.
        /// </summary>
        public void ApplyColor()
        {
            if (_originalImage == null || _originalImage.Empty())
                return;

            _currentImage?.Dispose();
            _currentImage = _originalImage.Clone();
            _thresholdValue = 127;
        }

        /// <summary>
        /// Threshold를 적용합니다.
        /// </summary>
        public void ApplyThreshold(int thresholdValue)
        {
            if (_originalImage == null || _originalImage.Empty())
                return;

            _thresholdValue = thresholdValue;
            _currentImage?.Dispose();

            Mat gray = _originalImage.Channels() == 3 ? new Mat() : _originalImage.Clone();
            if (_originalImage.Channels() == 3)
                Cv2.CvtColor(_originalImage, gray, ColorConversionCodes.BGR2GRAY);

            using (gray)
            using (var binary = new Mat())
            {
                Cv2.Threshold(gray, binary, _thresholdValue, 255, ThresholdTypes.Binary);
                _currentImage = new Mat();
                Cv2.CvtColor(binary, _currentImage, ColorConversionCodes.GRAY2BGR);
            }
        }

        /// <summary>
        /// Threshold를 적용하여 CurrentImage를 갱신합니다.
        /// </summary>
        public void CommitThreshold(int thresholdValue)
        {
            ApplyThreshold(thresholdValue);
        }

        /// <summary>
        /// 리소스를 해제합니다.
        /// </summary>
        public void Clear()
        {
            _originalImage?.Dispose();
            _currentImage?.Dispose();
            _originalImage = null;
            _currentImage = null;
            _thresholdValue = 127;
        }
    }
}
