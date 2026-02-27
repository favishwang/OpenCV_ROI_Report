using OpenCvSharp;

namespace ROI_Report.Class
{
    /// <summary>
    /// ROI 목록을 관리하는 클래스
    /// </summary>
    public class ROIManager
    {
        private readonly List<ROI> _roiList = new();
        private int _nextId = 1;

        public IReadOnlyList<ROI> ROIs => _roiList.AsReadOnly();

        public ROI AddROI(Rect region, RoiShapeType shapeType = RoiShapeType.Rect)
        {
            // 유효한 영역인지 확인 (최소 크기)
            if (region.Width < 2 || region.Height < 2)
                return null!;

            var roi = new ROI(region, _nextId++, shapeType);
            _roiList.Add(roi);
            return roi;
        }

        public bool RemoveROI(int id)
        {
            var roi = _roiList.FirstOrDefault(r => r.Id == id);
            if (roi != null)
            {
                _roiList.Remove(roi);
                ReindexROIs();
                return true;
            }
            return false;
        }

        public void ReindexROIs()
        {
            for (int i = 0; i < _roiList.Count; i++)
            {
                _roiList[i].Id = i + 1;
            }
            _nextId = _roiList.Count + 1;
        }
        
        public void Clear()
        {
            _roiList.Clear();
            _nextId = 1;
        }

        public void LoadFromData(IEnumerable<(Rect Region, RoiShapeType ShapeType)> roiData)
        {
            _roiList.Clear();
            int id = 1;
            foreach (var (region, shapeType) in roiData)
            {
                if (region.Width >= 2 && region.Height >= 2)
                {
                    _roiList.Add(new ROI(region, id++, shapeType));
                }
            }
            _nextId = _roiList.Count + 1;
        }

        public int Count => _roiList.Count;
    }
}
