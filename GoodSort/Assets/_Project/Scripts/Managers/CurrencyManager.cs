using Goodsort.Core;
using UnityEngine;

namespace Goodsort.Managers
{
    public class CurrencyManager : MonoBehaviour
    {
        public int CurrentCoins { get; private set; }
        private ISaveLoadService _saveLoadService; // Lấy qua ServiceProvider

        public void Initialize(ISaveLoadService saveLoadService, int startingCoins)
        {
            _saveLoadService = saveLoadService;
            // Load coins từ PlayerData thay vì startingCoins
            // CurrentCoins = _saveLoadService.LoadPlayerDataAsync().Result.playerCoins; // Cần làm async đúng cách
            CurrentCoins = startingCoins; // Tạm thời
            GlobalEvents.InvokeCoinsChanged(CurrentCoins);
            Debug.Log("[CurrencyManager] Initialized.");
        }

        public void AddCoins(int amount) { /* TODO: Implement */ GlobalEvents.InvokeCoinsChanged(CurrentCoins); }
        public bool CanSpendCoins(int amount) { /* TODO: Implement */ return false; }
        public bool SpendCoins(int amount) { /* TODO: Implement */ GlobalEvents.InvokeCoinsChanged(CurrentCoins); return false; }
        // Cần gọi SavePlayerData khi coin thay đổi hoặc định kỳ/khi thoát
    }
}