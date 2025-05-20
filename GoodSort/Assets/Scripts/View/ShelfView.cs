using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;

namespace GameCore
{
    public class ShelfView : MonoBehaviour
    {
        private Dictionary<int, List<ItemController>> m_items = new Dictionary<int, List<ItemController>>(8);
        private Vector2Int m_position;
        [SerializeField] private float m_offsetDistance;
        private List<SlotView> m_availableSlots = new List<SlotView>(3);
        
        public Vector2Int Position => m_position;
        
        public GameObject midSlot;
        public GameObject rightSlot;
        public GameObject leftSlot;
        private Dictionary<SlotView, float> m_slotPositions = new Dictionary<SlotView, float>(3);

        public void InitShelf(ShelfData shelfData, Action<int, Vector2Int, Vector2Int> onStageBoardChange)
        {
            m_slotPositions.Clear();
            m_availableSlots.Clear();
            m_position = shelfData.position;

            // Collect slot positions based on their local position in the prefab
            // Order: left - mid - right to match visual layout
            if (leftSlot != null)
            {
                float leftPos = leftSlot.transform.localPosition.x;
                m_slotPositions[leftSlot.GetComponent<MidSlotView>()] = leftPos;
                m_availableSlots.Add(leftSlot.GetComponent<MidSlotView>());
            }

            if (midSlot != null)
            {
                float midPos = midSlot.transform.localPosition.x;
                m_slotPositions[midSlot.GetComponent<MidSlotView>()] = midPos;
                m_availableSlots.Add(midSlot.GetComponent<MidSlotView>());
            }

            if (rightSlot != null)
            {
                float rightPos = rightSlot.transform.localPosition.x;
                m_slotPositions[rightSlot.GetComponent<MidSlotView>()] = rightPos;
                m_availableSlots.Add(rightSlot.GetComponent<MidSlotView>());
            }

            var itemPrefab = GameConfig.Instance.prefabConfig.itemPrefab;
            // Init Item - Add to List

            foreach (var slotData in shelfData.slotDatas)
            {
                SlotView targetSlot;
                if (shelfData.shelfType == ShelfType.Dispenser && midSlot != null && m_slotPositions.ContainsKey(midSlot.GetComponent<MidSlotView>()))
                {
                    // Always place items at the middle slot for Dispenser shelves
                    targetSlot = midSlot.GetComponent<MidSlotView>();
                }
                else
                {
                    targetSlot = m_availableSlots.First();
                    m_availableSlots.RemoveAt(0);
                }

                targetSlot.SlotData = slotData;

                for (var i = 0; i < slotData.itemsLists.Count; i++)
                {
                    var items = slotData.itemsLists[i];
                    if (items == -1) continue;
                    var itemData = GameConfig.Instance.itemDataConfig.GetItemDataByID(items);
                    var item = Instantiate(itemPrefab, targetSlot.transform);
                    item.transform.localPosition = new Vector3(0, 0, 0);
                    var controller = item.GetComponent<ItemController>();
                    controller.InitItem(itemData, i + 1, this, targetSlot, onStageBoardChange);
                    if (m_items.ContainsKey(i) == false) m_items.Add(i, new List<ItemController>() { controller });
                    else m_items[i].Add(controller);
                }
            }
        }

        public bool TryAddToShelf(ItemController item, SlotView slot)
        {
            if (m_items.First().Value.Count == 3)
            {
                return false;
            }

            if (slot == null || m_slotPositions.ContainsKey(slot) == false)
            {
                return false;
            }

            if (!m_availableSlots.Contains(slot))
            {
                return false;
            }

            slot.AddItem(item);
            item.SetShelfAndSlot(this, slot);
            m_items.First().Value.Add(item);
            m_availableSlots.Remove(slot);

            return true;
        }

        public bool TryAddToShelf(ItemController item)
        {
            if (m_items.First().Value.Count == 3)
            {
                return false;
            }

            SlotView[] order = { midSlot.GetComponent<MidSlotView>(), rightSlot.GetComponent<MidSlotView>(), leftSlot.GetComponent<MidSlotView>() };
            foreach (var slot in order)
            {
                if (slot != null && m_availableSlots.Contains(slot))
                {
                    return TryAddToShelf(item, slot);
                }
            }

            return false;
        }

        public void RemoveFromShelf(ItemController item)
        {
            if (m_items.First().Value.Contains(item))
            {
                m_items.First().Value.Remove(item);
                if (item.CurrentSlot != null)
                {
                    item.CurrentSlot.RemoveItem(item);
                    if (!m_availableSlots.Contains(item.CurrentSlot))
                    {
                        m_availableSlots.Add(item.CurrentSlot);
                    }
                }
                if (m_items.First().Value.Count == 0)
                {
                    RemoveLayerTop();
                }
            }
        }

        public void RemoveLayerTop()
        {
            var cacheList = m_items.ToList();

            foreach (var itemView in m_items.First().Value)
            {
                if (itemView.CurrentSlot != null)
                {
                    itemView.CurrentSlot.RemoveItem(itemView);
                    if (!m_availableSlots.Contains(itemView.CurrentSlot))
                    {
                        m_availableSlots.Add(itemView.CurrentSlot);
                    }
                }
                itemView.gameObject.SetActive(false);
            }

            for (int i = 0; i < cacheList.Count - 1; i++)
            {
                m_items[cacheList[i].Key] = cacheList[i + 1].Value;
            }

            if (m_items.Count > 1)
            {
                m_items.Remove(cacheList.Last().Key);
            }

            foreach (var itemLayer in m_items)
            {
                foreach (var item in itemLayer.Value)
                {
                    item.LayerIndex = itemLayer.Key;
                }
            }
        }
        
    }
}
