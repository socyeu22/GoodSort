using System;
using System.Collections.Generic;

namespace Goodsort.Data
{
    /// <summary>
    /// Lưu trữ dữ liệu của người chơi cần được lưu trữ bền vững.
    /// </summary>
    [System.Serializable]
    public class PlayerData
    {
        public int playerCoins = 0;
        public int highestLevelReached = 0;
        // Dictionary không serialize tốt với JsonUtility mặc định, cần custom hoặc dùng List<KeyValuePair>
        // Hoặc dùng một giải pháp serialization khác như Newtonsoft.Json
        // Tạm thời để Dictionary, SaveLoadManager sẽ xử lý việc này
        public Dictionary<string, int> levelFailStreaks = new Dictionary<string, int>(); // Key là templateID hoặc level group ID
        public List<float> lastNCompletionTimes = new List<float>(); // Dùng List để dễ serialize, quản lý như Queue trong code
        public int recentWins = 0;
        public int recentLosses = 0;
        public long lastDailyBonusClaimedBinary = 0; // Lưu DateTime.ToBinary()
        public int currentDailyStreak = 0;
        public int currentWinningStreak = 0;

        // Constructor mặc định
        public PlayerData() { }

        // Hàm tiện ích để quản lý lastNCompletionTimes như Queue
        public void AddCompletionTime(float time, int capacity = 5)
        {
            if (lastNCompletionTimes == null) lastNCompletionTimes = new List<float>(capacity);
            lastNCompletionTimes.Add(time);
            while (lastNCompletionTimes.Count > capacity)
            {
                lastNCompletionTimes.RemoveAt(0);
            }
        }
    }
}