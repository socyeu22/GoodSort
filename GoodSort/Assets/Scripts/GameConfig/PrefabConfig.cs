using GameCore;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Prefab Config", menuName = "GameConfig/Prefab Config")]
    public class PrefabConfig : ScriptableObject
    {
        public ItemController itemPrefab;
        public ShelfView shelfPrefab;
    }
}