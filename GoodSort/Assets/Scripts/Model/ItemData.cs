using System;
using DefaultNamespace;
using UnityEngine;

namespace GameCore
{
    /// <summary>
    /// Core data and basic logic for an item.
    /// </summary>
    [Serializable]
    public class ItemData
    {
        public int id;
        public ItemVisualType visualType;

        /// <summary>
        /// Sprite associated with this item.
        /// </summary>
        public Sprite Icon => GameConfig.Instance.itemIconConfig.GetItemIconByID(id);

        /// <summary>
        /// True if the item should be fully visible on the board.
        /// </summary>
        public bool IsFullDisplay => visualType == ItemVisualType.FullDisplay;
    }

    [Serializable]
    public enum ItemVisualType
    {
        FullDisplay,
        SecretDisplay
    }
}
