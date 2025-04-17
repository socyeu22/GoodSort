namespace Goodsort.Data
{
    /// <summary>
    /// Dữ liệu đầu vào cho ProceduralLevelGenerator để bias độ khó.
    /// </summary>
    public struct REMInputParams
    {
        public int failStreak;
        public float avgCompletionTimeLastN; // Giá trị âm nếu không đủ dữ liệu
        public float winRateLastN;          // Tỷ lệ 0-1, giá trị âm nếu không đủ dữ liệu
        // Có thể thêm các trường khác sau này
    }
}