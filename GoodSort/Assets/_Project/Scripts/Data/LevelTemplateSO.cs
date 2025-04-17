using UnityEngine;
using System.Collections.Generic;

namespace Goodsort.Data
{
    // Struct hoặc class phụ trợ
    [System.Serializable]
    public struct IntRange { public int min; public int max; }

    [System.Serializable]
    public class CellDepthProfile { public int depthMin = 1; public int depthMax = 3; public CellRegion region = CellRegion.All; }
    public enum CellRegion { All, Center, Edge, Custom } // Ví dụ

    [System.Serializable]
    public class ItemSpawnRule { public ItemTypeSO itemType; public int totalCount; } // Đảm bảo tổng là bội số 3 khi thiết kế

    [System.Serializable]
    public class ObstacleSpawnRule { public GameObject obstaclePrefabRef; public float spawnChance = 0.1f; public int baseCounterMin = 2; public int baseCounterMax = 4; }

    public enum Difficulty { Easy, Medium, Hard, VeryHard }

    [CreateAssetMenu(fileName = "LevelTemplate_", menuName = "Goodsort/Level Template", order = 1)]
    public class LevelTemplateSO : ScriptableObject
    {
        [Header("Identification")]
        public string templateID = "DefaultTemplate";
        public Difficulty targetDifficulty = Difficulty.Medium;

        [Header("Grid & Depth")]
        public Vector2Int gridSizeMin = new Vector2Int(5, 6);
        public Vector2Int gridSizeMax = new Vector2Int(6, 7);
        public List<CellDepthProfile> cellDepthConfig = new List<CellDepthProfile>() { new CellDepthProfile() }; // Default 1 profile cho All

        [Header("Items & Obstacles")]
        public List<ItemTypeSO> allowedItemTypes = new List<ItemTypeSO>();
        public List<ItemSpawnRule> itemSpawnRules = new List<ItemSpawnRule>(); // Designer cần đảm bảo tổng số mỗi loại là bội số 3
        public List<ObstacleSpawnRule> obstacleConfig = new List<ObstacleSpawnRule>();

        [Header("Gameplay Parameters")]
        public IntRange initialEmptyVisibleSlotsRange = new IntRange { min = 3, max = 6 };
        public float baseTimeLimit = 120f; // Thời gian cơ sở

        [Header("Generation Rules (Optional MVP+)")]
        public List<GenerationRuleSO> generationRules = new List<GenerationRuleSO>(); // SO chứa các luật phức tạp hơn
    }
    // Định nghĩa GenerationRuleSO nếu cần
    // public abstract class GenerationRuleSO : ScriptableObject { public abstract bool ApplyRule(...); }
}