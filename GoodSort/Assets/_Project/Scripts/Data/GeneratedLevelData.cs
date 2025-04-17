using System.Collections.Generic;
using UnityEngine;

namespace Goodsort.Data
{
    /// <summary>
    /// Chứa dữ liệu cụ thể của một level đã được sinh ra, sẵn sàng cho việc setup.
    /// </summary>
    public class GeneratedLevelData
    {
        public string sourceTemplateID;
        public Vector2Int actualGridSize;
        public List<List<int>> initialStacks; // List<List<int>>[y][x] -> List<int> itemIDs
        public int[] initialVisibleItems;     // Mảng 1D [y*width*3 + x*3 + slotIdx] = itemID
        public List<ObstacleData> initialObstacles;
        public float finalTimeLimit;
        public bool generationSuccess; // Đánh dấu thành công hay là dữ liệu fallback
    }

    // Struct phụ trợ nếu cần định nghĩa ObstacleData rõ hơn ở đây
    // [System.Serializable] public struct ObstacleData { public Vector2Int position; public int finalCounter; public GameObject prefabRef; }
}