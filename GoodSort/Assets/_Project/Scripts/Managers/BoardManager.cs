using Goodsort.Core;
using Goodsort.Data;
using UnityEngine;

namespace Goodsort.Managers
{
    public class BoardManager : MonoBehaviour
    {
         public GridRuntimeState CurrentGridState { get; private set; }
         // Thêm các tham chiếu khác nếu cần (PoolManager, ItemFactory...)

         public void Initialize()
         {
              Debug.Log("[BoardManager] Initialized.");
         }

         public void InitializeBoard(GeneratedLevelData data)
         {
             // TODO: Xóa board cũ (nếu có)
             CurrentGridState = new GridRuntimeState(data.actualGridSize);
             // TODO: Instantiate CellControllers
             // TODO: Khởi tạo CellRuntimeData[,] từ data.initialStacks và data.initialVisibleItems
             // TODO: Liên kết CellControllers với CellRuntimeData
             Debug.Log($"[BoardManager] Board initialized with size {data.actualGridSize}.");
         }

         // --- Các hàm API ---
         public CellRuntimeData GetCellData(Vector2Int pos) { /* TODO: Implement boundary checks */ return CurrentGridState.gridCells[pos.y, pos.x]; }
         public bool IsValidPosition(Vector2Int pos) { /* TODO: Implement */ return false; }
         public bool IsCellBlocked(Vector2Int pos) { /* TODO: Implement */ return false; }
         public bool IsCellPermanentlyEmpty(Vector2Int pos) { /* TODO: Implement */ return false; }
         public bool IsCellVisibleSlotEmpty(Vector2Int pos, int slotIndex) { /* TODO: Implement */ return false; }
         public bool IsMoveValid(Vector2Int fromPos, Vector2Int toPos, int toSlotIndex) { /* TODO: Implement logic */ return false; }
         public int GetItemInSlot(Vector2Int pos, int slotIndex) { /* TODO: Implement */ return -1; }
         public void UpdateCellVisibleSlot(Vector2Int pos, int slotIndex, int newItemID) { /* TODO: Implement logic */ }
         public void SetCellBlocked(Vector2Int pos, bool isBlocked) { /* TODO: Implement */ }
         public void SetCellPermanentlyEmpty(Vector2Int pos) { /* TODO: Implement */ }
         public Vector3 GetWorldPositionOfSlot(Vector2Int cellPos, int slotIndex) { /* TODO: Implement conversion */ return Vector3.zero; }
    }
}