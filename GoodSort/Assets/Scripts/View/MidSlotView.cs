using UnityEngine;

namespace GameCore
{
    /// <summary>
    ///     Extended slot used for snapping items by distance.
    ///     Holds information about whether the slot currently has an item
    ///     and which item id it accepts.
    /// </summary>
    public class MidSlotView : SlotView
    {
        [SerializeField] private int m_acceptItemId = -1;

        /// <summary>
        ///     Item id that this slot can accept. -1 means accept all items.
        /// </summary>
        public int AcceptItemId
        {
            get => m_acceptItemId;
            set => m_acceptItemId = value;
        }

        /// <summary>
        ///     Returns true if the slot does not contain any item at layer 1.
        /// </summary>
        public bool IsEmpty => TopItemId == -1;

        /// <summary>
        ///     Check if the slot can snap the given item id.
        /// </summary>
        public bool CanSnap(int itemId)
        {
            var shelf = GetComponentInParent<ShelfView>();
            if (shelf == null || shelf.ShelfType != ShelfType.Normal)
            {
                return false;
            }

            if (TopItemId != -1)
            {
                return false;
            }

            return m_acceptItemId == -1 || m_acceptItemId == itemId;
        }
    }
}
