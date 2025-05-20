using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameCore;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "ItemDataConfig", menuName = "GameConfig/ItemDataConfig")]
    public class ItemDataConfig : ScriptableObject
    {
        public List<ItemData> items = new();

        public ItemData GetItemDataByID(int id)
        {
            return items.FirstOrDefault(item => item.id == id);
        }
    }
}
