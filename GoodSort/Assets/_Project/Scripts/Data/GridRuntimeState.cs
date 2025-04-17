using UnityEngine;

namespace Goodsort.Data
{
    /// <summary>
    /// Lưu trạng thái logic của toàn bộ bàn chơi.
    /// </summary>
    public class GridRuntimeState
    {
        public CellRuntimeData[,] gridCells;
        public Vector2Int gridSize;

        public GridRuntimeState(Vector2Int size)
        {
            gridSize = size;
            gridCells = new CellRuntimeData[size.y, size.x];
            // Khởi tạo các CellRuntimeData bên trong BoardManager.InitializeBoard
        }
    }
}