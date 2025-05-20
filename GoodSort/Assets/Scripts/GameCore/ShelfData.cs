using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector; // Cần nếu dùng các thuộc tính Odin trong struct/class này

namespace GameCore
{
    /// <summary>
    /// Phân loại các loại kệ trong màn chơi.
    /// </summary>
    [Serializable] // Cần để Unity serialize enum này
    public enum ShelfType
    {
        Normal,    // Kệ thường, chứa 3 SlotData, được thuật toán điền item ngẫu nhiên.
        Dispenser, // Kệ đặc biệt, chứa 1 SlotData với item được định nghĩa sẵn.
        Empty      // Vị trí trống, không có kệ.
    }
    /// <summary>
    /// Dữ liệu cho một kệ đơn lẻ trong màn chơi.
    /// </summary>
    [System.Serializable] // Cần để Unity serialize struct này
    public struct ShelfData
    {
        [Tooltip("Loại kệ (Normal, Dispenser, Empty).")]
        public ShelfType shelfType;

        [Tooltip("Vị trí logic hoặc tọa độ tham chiếu của kệ.")]
        // Sử dụng Vector2Int nếu bạn vẫn muốn hệ tọa độ logic 2D
        // Hoặc dùng Vector3 nếu muốn đặt tự do trong không gian 3D
        public Vector2Int position;
        // public Vector3 worldPosition; // Thay thế nếu cần vị trí 3D

        [Tooltip("Danh sách các SlotData chứa item.\n" +
                 "- Normal: Luôn có 3 SlotData (do thuật toán tạo).\n" +
                 "- Dispenser: Có 1 SlotData chứa itemsLists được định nghĩa sẵn.\n" +
                 "- Empty: List rỗng.")]
        [ListDrawerSettings(NumberOfItemsPerPage = 5)] // Tùy chỉnh hiển thị list trong Odin
        // SerializeReference cần thiết nếu SlotData là class và bạn muốn hỗ trợ đa hình (không cần nếu là struct hoặc class đơn giản)
        [SerializeReference]
        // Hiển thị itemsLists chỉ khi là Dispenser và chỉ có 1 SlotData
        [InfoBox("Định nghĩa itemsLists cho Dispenser tại SlotData đầu tiên.", InfoMessageType.None, VisibleIf = "@this.shelfType == ShelfType.Dispenser && this.slotDatas != null && this.slotDatas.Count > 0")]
        public List<SlotData> slotDatas;


        // Constructor tiện lợi (tùy chọn)
        public ShelfData(ShelfType type, Vector2Int pos)
        {
            shelfType = type;
            position = pos;
            slotDatas = new List<SlotData>();
        }

        public bool IsFirstLayerEmpty
        {
            get
            {
                foreach (var slotData in slotDatas)
                {
                    if (slotData.itemsLists[0] != -1)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
