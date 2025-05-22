using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "ItemIconConfig", menuName = "GameConfig/ItemIconConfig")]
    public class ItemIconConfig : ScriptableObject
    {
        public List<ItemIcon> itemIcons;
        
        public Sprite GetItemIconByID(int id)
        {
            var entry = itemIcons.Find(x => x.id == id);
            return entry != null ? entry.icon : null;
        }
    }

    [Serializable]
    public class ItemIcon
    {
        public int id;
        public Sprite icon;
    }
}