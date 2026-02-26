using OpenCvSharp;
using OpenCvSharp.Extensions;
using ROI_Report.Class;

namespace ROI_Report
{
    public partial class MainForm : Form
    {
        private readonly VisionDisplay _visionDisplay = new();
        private readonly ROIManager _roiManager = new();

        private Mat? _originalImage;
        private Mat? _currentImage;
        private OpenCvSharp.Point _dragStart;
        private OpenCvSharp.Point _dragEnd;
        private bool _isDragging;
        private int? _selectedRoiId;
        private bool _updatingListSelection;
        private RoiShapeType _roiShapeType = RoiShapeType.Rect;

        public MainForm()
        {
            InitializeComponent();
            SetupPictureBoxEvents();
            UpdateRoiShapeButtons();
        }

        private void SetupPictureBoxEvents()
        {
            pictureBoxImage.MouseDown += PictureBox_MouseDown;
            pictureBoxImage.MouseMove += PictureBox_MouseMove;
            pictureBoxImage.MouseUp += PictureBox_MouseUp;
            pictureBoxImage.Resize += (s, e) =>
            {
                if (_currentImage != null)
                {
                    _visionDisplay.UpdateDisplaySize(pictureBoxImage.Size);
                    RefreshDisplay();
                }
            };
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
                    _originalImage?.Dispose();
                    _currentImage?.Dispose();

                    var loaded = Cv2.ImRead(ofd.FileName);
                    if (loaded == null || loaded.Empty())
                    {
                        MessageBox.Show("이미지 로드 실패");
                        return;
                    }

                    _originalImage = loaded.Clone();
                    _currentImage = _originalImage.Clone();

                    _roiManager.Clear();
                    _selectedRoiId = null;
                    _visionDisplay.SetImage(_currentImage, pictureBoxImage.Size);
                    RefreshRoiList();
                    RefreshDisplay();
                }
            }
        }

        private void Load_Image_Btn_Click(object? sender, EventArgs e)
        {
            LoadImage();
        }

        private void GrayImage_Click(object? sender, EventArgs e)
        {
            if (_originalImage == null || _originalImage.Empty())
            {
                MessageBox.Show("먼저 이미지를 불러오세요.");
                return;
            }

            _currentImage?.Dispose();
            using var gray = new Mat();
            Cv2.CvtColor(_originalImage, gray, ColorConversionCodes.BGR2GRAY);
            _currentImage = new Mat();
            Cv2.CvtColor(gray, _currentImage, ColorConversionCodes.GRAY2BGR);

            _visionDisplay.SetImage(_currentImage, pictureBoxImage.Size);
            RefreshDisplay();
        }

        private void ColorImage_Click(object? sender, EventArgs e)
        {
            if (_originalImage == null || _originalImage.Empty())
            {
                MessageBox.Show("먼저 이미지를 불러오세요.");
                return;
            }

            _currentImage?.Dispose();
            _currentImage = _originalImage.Clone();

            _visionDisplay.SetImage(_currentImage, pictureBoxImage.Size);
            RefreshDisplay();
        }

        private void Save_Image_Btn_Click(object? sender, EventArgs e)
        {
            if (_currentImage == null || _currentImage.Empty())
            {
                MessageBox.Show("먼저 이미지를 불러오세요.");
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = "이미지 저장",
                Filter = "JPEG 파일|*.jpg|PNG 파일|*.png|BMP 파일|*.bmp",
                DefaultExt = "jpg",
                FileName = "ROI_Image.jpg"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var bitmap = _visionDisplay.GetDisplayBitmapWithROIs(_roiManager, null, _selectedRoiId, _roiShapeType);
                if (bitmap != null)
                {
                    try
                    {
                        bitmap.Save(sfd.FileName);
                        MessageBox.Show("저장되었습니다.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"저장 실패: {ex.Message}");
                    }
                    finally
                    {
                        bitmap.Dispose();
                    }
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

        private void PictureBox_MouseDown(object? sender, MouseEventArgs e)
        {
            if (_currentImage == null || e.Button != MouseButtons.Left)
                return;

            if (!_visionDisplay.IsPointInDisplayArea(e.Location))
                return;

            _isDragging = true;
            _dragStart = _visionDisplay.DisplayToImage(e.Location);
            _dragEnd = _dragStart;
        }

        private void PictureBox_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!_isDragging || _currentImage == null)
                return;

            _dragEnd = _visionDisplay.DisplayToImage(e.Location);
            RefreshDisplay();
        }

        private void PictureBox_MouseUp(object? sender, MouseEventArgs e)
        {
            if (!_isDragging || e.Button != MouseButtons.Left || _currentImage == null)
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
