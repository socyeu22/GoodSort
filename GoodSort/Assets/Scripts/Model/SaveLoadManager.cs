using GameCore;
using UnityEngine;

public static class SaveLoadManager
{
    private const string SaveKey = "GameData";

    public static void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
        
        Debug.Log("Game data saved to PlayerPrefs.");
    }

    public static GameData LoadGame()
    {
        var json = PlayerPrefs.GetString(SaveKey);
        var data = JsonUtility.FromJson<GameData>(json);
        if (PlayerPrefs.HasKey(SaveKey))
        {
            Debug.Log("Game data loaded from PlayerPrefs.");
            return data;
        }

        Debug.LogWarning("No saved data found in PlayerPrefs. Returning new GameData.");
        return new GameData
        {
            currentLevel = 1
        };
    }

    public static void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        Debug.Log("Game data deleted from PlayerPrefs.");
    }
}