using Goodsort.Core;
using Goodsort.Data;
using UnityEngine;

namespace Goodsort.Managers
{
    public class GameStateManager : MonoBehaviour // Cần MonoBehaviour nếu dùng Coroutine/Update
    {
        public GameStateType CurrentState { get; private set; } = GameStateType.Loading; // State ban đầu

        public void Initialize()
        {
            Debug.Log("[GameStateManager] Initialized.");
            // Logic khởi tạo khác nếu cần
        }

        public async UniTask SwitchStateAsync(GameStateType newState) // Dùng UniTask
        {
            if (CurrentState == newState) return;

            Debug.Log($"[GameStateManager] Switching state from {CurrentState} to {newState}");
            // Logic Exit state cũ (nếu cần)
            // await ExitStateAsync(CurrentState);

            CurrentState = newState;
            GlobalEvents.InvokeGameStateChanged(newState);

            // Logic Enter state mới (nếu cần)
            // await EnterStateAsync(CurrentState);
        }
        // Các hàm EnterStateAsync/ExitStateAsync có thể thêm sau
    }
}