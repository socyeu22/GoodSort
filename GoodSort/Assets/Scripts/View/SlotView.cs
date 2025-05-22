using UnityEngine;

namespace GameCore
{
    public class SlotView : MonoBehaviour
    {
        [SerializeField] private SlotData m_slotData = new SlotData();
        [SerializeField] private GameObject m_topItemSlot;
        public SlotData SlotData
        {
            get => m_slotData;
            set => m_slotData = value;
        }

        /// <summary>
        ///     Id of the item currently on top of this slot. -1 if empty.
        /// </summary>
        public int TopItemId
        {
            get
            {
                if (m_slotData.itemsLists == null || m_slotData.itemsLists.Count == 0)
                {
                    return -1;
                }
                return m_slotData.itemsLists[0];
            }
        }

        public virtual void AddItem(ItemController item)
        {
            item.transform.SetParent(transform);
            // Reset local position so the item snaps exactly to the slot
            item.transform.localPosition = Vector3.zero;
            item.LayerIndex = transform.childCount;

            if (m_slotData.itemsLists.Count == 0)
            {
                m_slotData.itemsLists.Add(item.Id);
            }
            else
            {
                m_slotData.itemsLists[0] = item.Id;
            }

            UpdateTopItemSlotVisibility();
        }

        public bool RemoveItem(ItemController item)
        {
            if (item.transform.parent == transform)
            {
                item.transform.SetParent(null);
                if (m_slotData.itemsLists.Count > 0)
                {
                    m_slotData.itemsLists[0] = -1;
                }
                RearrangeItems();
                UpdateTopItemSlotVisibility();
                return true;
            }
            return false;
        }

        private void RearrangeItems()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var item = transform.GetChild(i).GetComponent<ItemController>();
                if (item != null)
                {
                    item.LayerIndex = i + 1;
                }
            }
        }

        private void Start()
        {
            UpdateTopItemSlotVisibility();
        }

        private void UpdateTopItemSlotVisibility()
        {
            if (m_topItemSlot != null)
            {
                m_topItemSlot.SetActive(TopItemId != -1);
            }
        }
    }
}
