using System;

namespace GameCore
{
    [Serializable]
    public class TopItemData : ItemData
    {
        /// <summary>
        /// True if this item should spawn on top of a slot.
        /// </summary>
        public bool isTopItem;
    }
}
