using Goodsort.Data;
using System.IO;
using UnityEngine;
using Cysharp.Threading.Tasks; // Using UniTask

namespace Goodsort.Managers
{
    public interface ISaveLoadService { UniTask SavePlayerDataAsync(PlayerData data); UniTask<PlayerData> LoadPlayerDataAsync(); }

    public class SaveLoadManager : ISaveLoadService // Implement Interface
    {
        private string _saveFilePath;

        public void Initialize()
        {
            _saveFilePath = Path.Combine(Application.persistentDataPath, "playerData.json");
            Debug.Log($"[SaveLoadManager] Initialized. Save path: {_saveFilePath}");
        }

        public async UniTask SavePlayerDataAsync(PlayerData data)
        {
            try
            {
                string json = JsonUtility.ToJson(data, true); // Dùng JsonUtility đơn giản
                // Cần xử lý đặc biệt cho Dictionary nếu dùng JsonUtility
                // Hoặc chuyển sang dùng Newtonsoft.Json
                await File.WriteAllTextAsync(_saveFilePath, json);
                Debug.Log("[SaveLoadManager] Player data saved.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveLoadManager] Error saving data: {e.Message}");
            }
        }

        public async UniTask<PlayerData> LoadPlayerDataAsync()
        {
            if (!File.Exists(_saveFilePath))
            {
                 Debug.Log("[SaveLoadManager] No save file found. Creating new PlayerData.");
                return new PlayerData(); // Trả về dữ liệu mới nếu chưa có file
            }

            try
            {
                string json = await File.ReadAllTextAsync(_saveFilePath);
                PlayerData loadedData = JsonUtility.FromJson<PlayerData>(json);
                // Cần xử lý load Dictionary đặc biệt nếu dùng JsonUtility
                 Debug.Log("[SaveLoadManager] Player data loaded.");
                return loadedData ?? new PlayerData(); // Trả về data mới nếu parse lỗi
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveLoadManager] Error loading data: {e.Message}");
                return new PlayerData(); // Trả về data mới nếu có lỗi
            }
        }
    }
}