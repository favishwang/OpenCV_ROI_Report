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

        /// <summary>
        /// 새 ROI를 추가합니다.
        /// </summary>
        /// <param name="region">ROI 영역 (이미지 좌표 기준)</param>
        /// <param name="shapeType">ROI 모양 (Rect 또는 Circle)</param>
        /// <returns>추가된 ROI</returns>
        public ROI AddROI(Rect region, RoiShapeType shapeType = RoiShapeType.Rect)
        {
            // 유효한 영역인지 확인 (최소 크기)
            if (region.Width < 2 || region.Height < 2)
                return null!;

            var roi = new ROI(region, _nextId++, shapeType);
            _roiList.Add(roi);
            return roi;
        }

        /// <summary>
        /// 지정된 ROI를 제거합니다.
        /// </summary>
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

        /// <summary>
        /// ROI ID를 1부터 순차적으로 재정렬합니다.
        /// </summary>
        public void ReindexROIs()
        {
            for (int i = 0; i < _roiList.Count; i++)
            {
                _roiList[i].Id = i + 1;
            }
            _nextId = _roiList.Count + 1;
        }

        /// <summary>
        /// 모든 ROI를 제거합니다.
        /// </summary>
        public void Clear()
        {
            _roiList.Clear();
        }

        /// <summary>
        /// ROI 개수를 반환합니다.
        /// </summary>
        public int Count => _roiList.Count;
    }
}
