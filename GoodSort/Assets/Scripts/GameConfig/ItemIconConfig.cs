using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "ItemIconConfig", menuName = "GameConfig/ItemIconConfig")]
    public class ItemIconConfig : ScriptableObject
    {
        public List<ItemIcon> itemIcons;
        
        public Sprite GetItemIconByID(int id) => itemIcons.Find(x => x.id == id).icon;
    }

    [Serializable]
    public class ItemIcon
    {
        public int id;
        public Sprite icon;
    }
}