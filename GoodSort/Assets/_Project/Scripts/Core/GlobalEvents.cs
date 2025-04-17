using System;
using System.Collections.Generic; // Cần cho List trong EventArgs ví dụ
using Goodsort.Data; // Để sử dụng các kiểu dữ liệu như GameStateType
using UnityEngine; // Cần cho Vector2Int

namespace Goodsort.Core
{
    // Định nghĩa các lớp EventArgs nếu cần truyền dữ liệu phức tạp
    public class MatchEventArgs : EventArgs
    {
        public Vector2Int CellPosition;
        public List<int> MatchedItemInstanceIDs; // Hoặc ItemControllers nếu cần tham chiếu trực tiếp
        public int MatchedItemTypeID;
    }

    public class MoveEventArgs : EventArgs
    {
        // Thêm các trường cần thiết sau này, ví dụ:
        // public ItemController MovedItem;
        public Vector2Int FromCell;
        public Vector2Int ToCell;
        public int ToSlotIndex;
    }

    /// <summary>
    /// Lớp tĩnh chứa các sự kiện (events) toàn cục của game.
    /// Các hệ thống khác có thể đăng ký (subscribe) và hủy đăng ký (unsubscribe) vào các sự kiện này.
    /// </summary>
    public static class GlobalEvents
    {
        // -- Game State Events --
        public static event Action<GameStateType> OnGameStateChanged;
        public static void InvokeGameStateChanged(GameStateType newState) => OnGameStateChanged?.Invoke(newState);

        // -- Gameplay Events --
        public static event Action<ItemController> OnItemSelected; // Truyền ItemController được chọn
        public static void InvokeItemSelected(ItemController item) => OnItemSelected?.Invoke(item);

        public static event Action<Vector2Int, int> OnEmptySlotSelected; // Truyền vị trí cell và index slot
        public static void InvokeEmptySlotSelected(Vector2Int cellPos, int slotIndex) => OnEmptySlotSelected?.Invoke(cellPos, slotIndex);

        public static event Action<MoveEventArgs> OnMoveComplete;
        public static void InvokeMoveComplete(MoveEventArgs args) => OnMoveComplete?.Invoke(args);

        public static event Action<MatchEventArgs> OnMatchFound;
        public static void InvokeMatchFound(MatchEventArgs args) => OnMatchFound?.Invoke(args);

        public static event Action<Vector2Int> OnPotentialCellCleared; // Khi 1 ô có thể đã trống sau khi di chuyển item đi
        public static void InvokePotentialCellCleared(Vector2Int cellPos) => OnPotentialCellCleared?.Invoke(cellPos);

        public static event Action<Vector2Int> OnStackItemRevealed; // Khi item mới lộ diện từ stack
        public static void InvokeStackItemRevealed(Vector2Int cellPos) => OnStackItemRevealed?.Invoke(cellPos);

         public static event Action<ObstacleController> OnObstacleDestroyed; // Khi obstacle bị phá hủy
        public static void InvokeObstacleDestroyed(ObstacleController obstacle) => OnObstacleDestroyed?.Invoke(obstacle);

        // -- Combo & Reward Events --
        public static event Action<int> OnComboUpdated; // Truyền combo count hiện tại
        public static void InvokeComboUpdated(int comboCount) => OnComboUpdated?.Invoke(comboCount);

         public static event Action<int> OnComboRewardCalculated; // Truyền số coin thưởng
        public static void InvokeComboRewardCalculated(int coinAmount) => OnComboRewardCalculated?.Invoke(coinAmount);

        // -- Currency Events --
        public static event Action<int> OnCoinsChanged; // Truyền tổng số coin mới
        public static void InvokeCoinsChanged(int newCoinTotal) => OnCoinsChanged?.Invoke(newCoinTotal);

        // -- Timer Events --
        public static event Action OnTimeExpired;
        public static void InvokeTimeExpired() => OnTimeExpired?.Invoke();

        // -- Thêm các events khác khi cần --
    }
}