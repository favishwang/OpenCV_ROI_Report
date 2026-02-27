using System.Text.Json;
using OpenCvSharp;

namespace ROI_Report.Class
{
    /// <summary>
    /// ROI 데이터 파일 저장/불러오기 담당
    /// </summary>
    public class RoiFileService
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = true
        };

        public void Save(ROIManager manager, string filePath)
        {
            if (manager.Count == 0)
                throw new InvalidOperationException("저장할 ROI가 없습니다.");

            var fileData = RoiFileData.FromROIManager(manager);
            var json = JsonSerializer.Serialize(fileData, SerializerOptions);
            File.WriteAllText(filePath, json);
        }

        public List<(Rect Region, RoiShapeType ShapeType)> Load(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var fileData = JsonSerializer.Deserialize<RoiFileData>(json);

            if (fileData?.Rois == null || fileData.Rois.Count == 0)
                throw new InvalidOperationException("유효한 ROI 데이터가 없습니다.");

            return fileData.ToROIData();
        }
    }
}
