using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace ROI_Report.Forms
{
    public partial class ThresholdSetting_Form : Form
    {
        private Mat? _sourceImage;
        private Mat? _resultImage;

        /// <summary>
        /// 적용된 Threshold 이미지 결과
        /// </summary>
        public Mat? ResultImage => _resultImage;

        /// <summary>
        /// 현재 Threshold 값
        /// </summary>
        public int CurrentThresholdValue => Threshold_Slider.Value;

        /// <summary>
        /// 슬라이더 값 변경 시 발생 (MainForm의 threshold 값 동기화용)
        /// </summary>
        public event Action<int>? ThresholdValueChanged;

        public ThresholdSetting_Form(Mat sourceImage, int initialThreshold = 127)
        {
            InitializeComponent();
            _sourceImage = sourceImage?.Clone();
            Threshold_Slider.Value = Math.Clamp(initialThreshold, 0, 255);
            ThresholdValue.Text = Threshold_Slider.Value.ToString();
            SetupEvents();
            UpdatePreview();
        }

        private void SetupEvents()
        {
            Threshold_Slider.ValueChanged += Threshold_Slider_ValueChanged;
            btnApply.Click += BtnApply_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private void Threshold_Slider_ValueChanged(object? sender, EventArgs e)
        {
            var value = Threshold_Slider.Value;
            ThresholdValue.Text = value.ToString();
            ThresholdValueChanged?.Invoke(value);
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            if (_sourceImage == null || _sourceImage.Empty())
                return;

            Mat gray = _sourceImage.Channels() == 3 ? new Mat() : _sourceImage.Clone();
            if (_sourceImage.Channels() == 3)
                Cv2.CvtColor(_sourceImage, gray, ColorConversionCodes.BGR2GRAY);

            using (gray)
            using (var binary = new Mat())
            using (var display = new Mat())
            {
                Cv2.Threshold(gray, binary, Threshold_Slider.Value, 255, ThresholdTypes.Binary);
                Cv2.CvtColor(binary, display, ColorConversionCodes.GRAY2BGR);

                var old = pictureBoxPreview.Image;
                pictureBoxPreview.Image = display.ToBitmap();
                old?.Dispose();
                pictureBoxPreview.Refresh();
            }
        }

        private void BtnApply_Click(object? sender, EventArgs e)
        {
            if (_sourceImage == null || _sourceImage.Empty())
            {
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            Mat gray = _sourceImage.Channels() == 3 ? new Mat() : _sourceImage.Clone();
            if (_sourceImage.Channels() == 3)
                Cv2.CvtColor(_sourceImage, gray, ColorConversionCodes.BGR2GRAY);

            using (gray)
            using (var binary = new Mat())
            {
                Cv2.Threshold(gray, binary, Threshold_Slider.Value, 255, ThresholdTypes.Binary);
                _resultImage = new Mat();
                Cv2.CvtColor(binary, _resultImage, ColorConversionCodes.GRAY2BGR);
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _sourceImage?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
