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

        /// <summary>
        /// SlotData that this top item currently belongs to.
        /// </summary>
        public SlotData slotData;

        /// <summary>
        /// SlotView displaying this top item.
        /// </summary>
        [NonSerialized]
        public SlotView slotView;

        /// <summary>
        /// Assign the owning slot data and view.
        /// </summary>
        public void AssignSlot(SlotData data, SlotView view)
        {
            slotData = data;
            slotView = view;
        }

        /// <summary>
        /// Swap the slot information between two top items.
        /// </summary>
        public static void Swap(TopItemData first, TopItemData second)
        {
            var tempData = first.slotData;
            var tempView = first.slotView;

            first.AssignSlot(second.slotData, second.slotView);
            second.AssignSlot(tempData, tempView);
        }
    }
}
