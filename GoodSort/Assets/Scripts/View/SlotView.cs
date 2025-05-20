using UnityEngine;

namespace GameCore
{
    public class SlotView : MonoBehaviour
    {
        [SerializeField] private SlotData m_slotData = new SlotData();
        public SlotData SlotData
        {
            get => m_slotData;
            set => m_slotData = value;
        }

        public virtual void AddItem(ItemView item)
        {
            item.transform.SetParent(transform);
            item.LayerIndex = transform.childCount;

            if (m_slotData.itemsLists.Count == 0)
            {
                m_slotData.itemsLists.Add(item.Id);
            }
            else
            {
                m_slotData.itemsLists[0] = item.Id;
            }
        }

        public bool RemoveItem(ItemView item)
        {
            if (item.transform.parent == transform)
            {
                item.transform.SetParent(null);
                if (m_slotData.itemsLists.Count > 0)
                {
                    m_slotData.itemsLists[0] = -1;
                }
                RearrangeItems();
                return true;
            }
            return false;
        }

        private void RearrangeItems()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var item = transform.GetChild(i).GetComponent<ItemView>();
                if (item != null)
                {
                    item.LayerIndex = i + 1;
                }
            }
        }
    }
}
