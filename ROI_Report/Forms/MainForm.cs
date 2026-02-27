using OpenCvSharp;
using OpenCvSharp.Extensions;
using ROI_Report.Class;
using ROI_Report.Forms;

namespace ROI_Report
{
    public partial class MainForm : Form
    {
        private readonly VisionDisplay _visionDisplay = new();
        private readonly ROIManager _roiManager = new();
        private readonly RoiFileService _roiFileService = new();
        private readonly BlobDetector _blobDetector = new();
        private readonly ImageContext _imageContext = new();

        private OpenCvSharp.Point _dragStart;
        private OpenCvSharp.Point _dragEnd;
        private bool _isDragging;
        private int? _selectedRoiId;
        private bool _updatingListSelection;
        private RoiShapeType _roiShapeType = RoiShapeType.Rect;

        public MainForm()
        {
            InitializeComponent();
            SetupEvents();
            UpdateRoiShapeButtons();
        }

        private void SetupEvents()
        {
            // PictureBox
            pictureBoxImage.DoubleClick += PictureBoxImage_DoubleClick;
            pictureBoxImage.MouseDown += PictureBox_MouseDown;
            pictureBoxImage.MouseMove += PictureBox_MouseMove;
            pictureBoxImage.MouseUp += PictureBox_MouseUp;
            pictureBoxImage.Resize += PictureBoxImage_Resize;

            // Buttons
            Load_Image_Btn.Click += Load_Image_Btn_Click;
            GrayImage.Click += GrayImage_Click;
            ColorImage.Click += ColorImage_Click;
            Save_Image_Btn.Click += Save_Image_Btn_Click;
            ROI_Rect_Btn.Click += ROI_Rect_Btn_Click;
            ROI_Circle_Btn.Click += ROI_Circle_Btn_Click;
            thresholdButton.Click += ThresholdButton_Click;
            blobButton.Click += BlobButton_Click;

            // ListBox
            listBoxRoi.SelectedIndexChanged += ListBoxRoi_SelectedIndexChanged;
            listBoxRoi.KeyDown += ListBoxRoi_KeyDown;
            listBoxRoi.MouseDown += ListBoxRoi_MouseDown;
        }

        private void PictureBoxImage_Resize(object? sender, EventArgs e)
        {
            if (_imageContext.CurrentImage != null)
            {
                _visionDisplay.UpdateDisplaySize(pictureBoxImage.Size);
                RefreshDisplay();
            }
        }

        private void LoadImage()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "이미지 선택";
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var loaded = Cv2.ImRead(ofd.FileName);
                    if (loaded == null || loaded.Empty())
                    {
                        MessageBox.Show("이미지 로드 실패");
                        return;
                    }

                    _imageContext.Load(loaded);
                    _roiManager.Clear();
                    _selectedRoiId = null;
                    _visionDisplay.SetImage(_imageContext.CurrentImage!, pictureBoxImage.Size);
                    RefreshRoiList();
                    RefreshDisplay();
                }
            }
        }

        private void PictureBoxImage_DoubleClick(object? sender, EventArgs e)
        {
            LoadImage();
        }

        private void Load_Image_Btn_Click(object? sender, EventArgs e)
        {
            LoadRoiData();
        }

        private void GrayImage_Click(object? sender, EventArgs e)
        {
            if (!_imageContext.HasImage)
            {
                MessageBox.Show("먼저 이미지를 불러오세요.");
                return;
            }

            _imageContext.ApplyGrayscale();
            _visionDisplay.SetImage(_imageContext.CurrentImage!, pictureBoxImage.Size);
            RefreshDisplay();
        }

        private void ColorImage_Click(object? sender, EventArgs e)
        {
            if (!_imageContext.HasImage)
            {
                MessageBox.Show("먼저 이미지를 불러오세요.");
                return;
            }

            _imageContext.ApplyColor();
            _visionDisplay.SetImage(_imageContext.CurrentImage!, pictureBoxImage.Size);
            RefreshDisplay();
        }

        private void Save_Image_Btn_Click(object? sender, EventArgs e)
        {
            SaveRoiData();
        }

        private void LoadRoiData()
        {
            if (!_imageContext.HasImage)
            {
                MessageBox.Show("먼저 이미지를 불러오세요.\n(이미지 영역 더블클릭)");
                return;
            }

            using var ofd = new OpenFileDialog
            {
                Title = "ROI 데이터 불러오기",
                Filter = "JSON 파일|*.json",
                DefaultExt = "json",
                FileName = "ROI_Data.json"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var roiData = _roiFileService.Load(ofd.FileName);
                    _roiManager.LoadFromData(roiData);
                    _selectedRoiId = null;
                    RefreshRoiList();
                    RefreshDisplay();
                    MessageBox.Show($"{_roiManager.Count}개의 ROI를 불러왔습니다.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"불러오기 실패: {ex.Message}");
                }
            }
        }

        private void SaveRoiData()
        {
            using var sfd = new SaveFileDialog
            {
                Title = "ROI 데이터 저장",
                Filter = "JSON 파일|*.json",
                DefaultExt = "json",
                FileName = "ROI_Data.json"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _roiFileService.Save(_roiManager, sfd.FileName);
                    MessageBox.Show("저장되었습니다.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ROI_Rect_Btn_Click(object? sender, EventArgs e)
        {
            _roiShapeType = RoiShapeType.Rect;
            UpdateRoiShapeButtons();
        }

        private void ROI_Circle_Btn_Click(object? sender, EventArgs e)
        {
            _roiShapeType = RoiShapeType.Circle;
            UpdateRoiShapeButtons();
        }

        private void UpdateRoiShapeButtons()
        {
            ROI_Rect_Btn.BackColor = _roiShapeType == RoiShapeType.Rect ? Color.LightBlue : SystemColors.Control;
            ROI_Circle_Btn.BackColor = _roiShapeType == RoiShapeType.Circle ? Color.LightBlue : SystemColors.Control;
        }

        private void ThresholdButton_Click(object? sender, EventArgs e)
        {
            if (!_imageContext.HasImage)
            {
                MessageBox.Show("먼저 이미지를 불러오세요.");
                return;
            }

            var form = new ThresholdSetting_Form(_imageContext.OriginalImage!, _imageContext.ThresholdValue);
            form.ThresholdValueChanged += (value) => _imageContext.ThresholdValue = value;
            if (form.ShowDialog(this) == DialogResult.OK && form.ResultImage != null)
            {
                _imageContext.CommitThreshold(form.CurrentThresholdValue);
                form.ResultImage.Dispose();
                _visionDisplay.SetImage(_imageContext.CurrentImage!, pictureBoxImage.Size);
                RefreshDisplay();
            }
            form.Dispose();
        }

        private void BlobButton_Click(object? sender, EventArgs e)
        {
            if (!_imageContext.HasImage)
            {
                MessageBox.Show("먼저 이미지를 불러오세요.");
                return;
            }

            _blobDetector.ThresholdValue = _imageContext.ThresholdValue;
            var rects = _blobDetector.Detect(_imageContext.CurrentImage!);

            if (rects.Count == 0)
            {
                MessageBox.Show("검출된 Blob이 없습니다.\nThreshold 값을 조정해 보세요.");
                return;
            }

            _roiManager.Clear();
            foreach (var rect in rects)
            {
                _roiManager.AddROI(rect, RoiShapeType.Rect);
            }
            _selectedRoiId = null;
            RefreshRoiList();
            RefreshDisplay();
            MessageBox.Show($"{rects.Count}개의 Blob을 ROI로 추가했습니다.");
        }

        private void PictureBox_MouseDown(object? sender, MouseEventArgs e)
        {
            if (_imageContext.CurrentImage == null || e.Button != MouseButtons.Left)
                return;

            if (!_visionDisplay.IsPointInDisplayArea(e.Location))
                return;

            _isDragging = true;
            _dragStart = _visionDisplay.DisplayToImage(e.Location);
            _dragEnd = _dragStart;
        }

        private void PictureBox_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!_isDragging || _imageContext.CurrentImage == null)
                return;

            _dragEnd = _visionDisplay.DisplayToImage(e.Location);
            RefreshDisplay();
        }

        private void PictureBox_MouseUp(object? sender, MouseEventArgs e)
        {
            if (!_isDragging || e.Button != MouseButtons.Left || _imageContext.CurrentImage == null)
                return;

            _isDragging = false;
            _dragEnd = _visionDisplay.DisplayToImage(e.Location);

            var rect = CreateRectFromPoints(_dragStart, _dragEnd);
            var added = _roiManager.AddROI(rect, _roiShapeType);
            if (added != null)
            {
                _dragStart = default;
                _dragEnd = default;
                RefreshRoiList();
                _selectedRoiId = added.Id;
                _updatingListSelection = true;
                var index = _roiManager.ROIs.ToList().FindIndex(r => r.Id == added.Id);
                if (index >= 0)
                    listBoxRoi.SelectedIndex = index;
                _updatingListSelection = false;
                RefreshDisplay();
            }
        }

        private static OpenCvSharp.Rect CreateRectFromPoints(OpenCvSharp.Point p1, OpenCvSharp.Point p2)
        {
            int x = Math.Min(p1.X, p2.X);
            int y = Math.Min(p1.Y, p2.Y);
            int w = Math.Abs(p2.X - p1.X);
            int h = Math.Abs(p2.Y - p1.Y);
            return new Rect(x, y, w, h);
        }

        private void RefreshDisplay()
        {
            Rect? selectionRect = null;
            if (_isDragging)
            {
                var rect = CreateRectFromPoints(_dragStart, _dragEnd);
                if (rect.Width > 0 && rect.Height > 0)
                    selectionRect = rect;
            }

            var bitmap = _visionDisplay.GetDisplayBitmapWithROIs(_roiManager, selectionRect, _selectedRoiId, _roiShapeType);
            if (bitmap != null)
            {
                var old = pictureBoxImage.Image;
                pictureBoxImage.Image = bitmap;
                old?.Dispose();
            }
        }

        private void RefreshRoiList()
        {
            listBoxRoi.Items.Clear();
            foreach (var roi in _roiManager.ROIs)
            {
                var shapeLabel = roi.ShapeType == RoiShapeType.Circle ? "○" : "□";
                listBoxRoi.Items.Add($"{shapeLabel} {roi.Name} ({roi.Region.X}, {roi.Region.Y}) {roi.Region.Width}x{roi.Region.Height}");
            }
            // ListBox가 첫 항목을 자동 선택하는 것을 방지 - 선택 상태 명시적 복원
            _updatingListSelection = true;
            if (_selectedRoiId.HasValue)
            {
                var index = _roiManager.ROIs.ToList().FindIndex(r => r.Id == _selectedRoiId.Value);
                listBoxRoi.SelectedIndex = index >= 0 ? index : -1;
            }
            else
            {
                listBoxRoi.SelectedIndex = -1;
            }
            _updatingListSelection = false;
        }

        private void ListBoxRoi_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Delete || listBoxRoi.SelectedIndex < 0)
                return;

            var rois = _roiManager.ROIs.ToList();
            if (listBoxRoi.SelectedIndex >= rois.Count)
                return;

            var roiToRemove = rois[listBoxRoi.SelectedIndex];
            _roiManager.RemoveROI(roiToRemove.Id);
            _selectedRoiId = null;
            RefreshRoiList();
            RefreshDisplay();
        }

        private void ListBoxRoi_MouseDown(object? sender, MouseEventArgs e)
        {
            // 빈 영역 클릭 시 선택 해제
            var index = listBoxRoi.IndexFromPoint(e.Location);
            if (index < 0)
            {
                _updatingListSelection = true;
                listBoxRoi.SelectedIndex = -1;
                _selectedRoiId = null;
                _updatingListSelection = false;
                RefreshDisplay();
            }
        }

        private void ListBoxRoi_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_updatingListSelection)
                return;

            if (listBoxRoi.SelectedIndex < 0)
            {
                _selectedRoiId = null;
            }
            else
            {
                var rois = _roiManager.ROIs.ToList();
                if (listBoxRoi.SelectedIndex < rois.Count)
                    _selectedRoiId = rois[listBoxRoi.SelectedIndex].Id;
            }
            RefreshDisplay();
        }
    }
}
