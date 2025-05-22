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

        /// <summary>
        /// Loại bỏ các giá trị -1 từ index 1 trở đi trong <see cref="itemsLists"/>.
        /// Item ở index 0 sẽ được giữ nguyên dù có giá trị -1.
        /// </summary>
        public void RemoveNegativeFromIndexOne()
        {
            if (itemsLists == null) return;
            for (int i = itemsLists.Count - 1; i >= 1; i--)
            {
                if (itemsLists[i] == -1)
                {
                    itemsLists.RemoveAt(i);
                }
            }
        }
    }
}
