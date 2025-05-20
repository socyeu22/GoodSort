using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    public class SlotView : MonoBehaviour
    {
        private readonly List<ItemView> m_items = new List<ItemView>();
        public IReadOnlyList<ItemView> Items => m_items;

        public virtual void AddItem(ItemView item)
        {
            if (!m_items.Contains(item))
            {
                m_items.Add(item);
                item.transform.SetParent(transform);
                item.LayerIndex = m_items.Count;
            }
        }

        public bool RemoveItem(ItemView item)
        {
            int index = m_items.IndexOf(item);
            if (index >= 0)
            {
                m_items.RemoveAt(index);
                RearrangeItems();
                return true;
            }
            return false;
        }

        private void RearrangeItems()
        {
            for (int i = 0; i < m_items.Count; i++)
            {
                if (m_items[i] != null)
                {
                    m_items[i].LayerIndex = i + 1;
                }
            }
        }
    }
}
