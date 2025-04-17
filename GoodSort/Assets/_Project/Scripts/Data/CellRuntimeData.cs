using System.Collections.Generic;
using UnityEngine;

namespace Goodsort.Data
{
    /// <summary>
    /// Lưu trạng thái động của một Ô Lớn (Large Cell) trong quá trình chơi.
    /// </summary>
    // [System.Serializable] // Không cần serialize runtime data này thường xuyên
    public class CellRuntimeData
    {
        public List<int> hiddenItems = new List<int>(); // *** Sử dụng List<int> ***
        public int[] visibleSlots = new int[3] { -1, -1, -1 }; // -1 là trống
        public bool isBlockedByObstacle = false;
        public bool isPermanentlyEmpty = false;
        public ObstacleController linkedObstacle = null; // Tham chiếu nhanh
        public Vector2Int gridPosition;

        // Constructor hoặc hàm khởi tạo nếu cần
        public CellRuntimeData(Vector2Int pos) { gridPosition = pos; }
    }
}