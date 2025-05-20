using System.Collections.Generic;
using UnityEngine;

namespace GameCore {
    /// <summary>
    /// Dữ liệu cho một slot con bên trong một ShelfData.
    /// Chứa danh sách các item ID xếp chồng theo lớp (layer).
    /// </summary>

    [System.Serializable] // Cần để Unity serialize class này
    public class SlotData
    {
        [Tooltip("Danh sách Item ID theo từng lớp (layer). Index 0 là lớp trên cùng.")]
        // Khởi tạo list để tránh lỗi NullReferenceException
        public List<int> itemsLists = new List<int>();
    }
}
