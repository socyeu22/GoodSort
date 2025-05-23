using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;

namespace GameCore
{
    public class ShelfView : MonoBehaviour
    {
        private const int MaxItemsPerLayer = 3;

        private readonly Dictionary<int, List<ItemController>> m_items = new Dictionary<int, List<ItemController>>(8);
        private readonly List<SlotView> m_availableSlots = new List<SlotView>(3);
        private readonly List<SlotView> m_slots = new List<SlotView>(3);

        private Vector2Int m_position;
        [SerializeField] private float m_offsetDistance;
        private ShelfType m_shelfType;

        public Vector2Int Position => m_position;
        public ShelfType ShelfType => m_shelfType;
        public float OffsetDistance => m_offsetDistance;

        [SerializeField] private MidSlotView m_midSlot;
        [SerializeField] private MidSlotView m_rightSlot;
        [SerializeField] private MidSlotView m_leftSlot;

        private List<ItemController> TopLayer => m_items.First().Value;

        private void AddSlot(SlotView slot)
        {
            if (slot == null)
            {
                return;
            }
            m_slots.Add(slot);
            m_availableSlots.Add(slot);
        }

        public void MarkSlotAvailable(SlotView slot)
        {
            if (slot == null)
            {
                return;
            }

            if (!m_availableSlots.Contains(slot))
            {
                m_availableSlots.Add(slot);
            }
        }

        public void MarkSlotUnavailable(SlotView slot)
        {
            if (slot == null)
            {
                return;
            }

            m_availableSlots.Remove(slot);
        }

        private SlotView GetNextAvailableSlot()
        {
            if (m_availableSlots.Count == 0)
            {
                return null;
            }
            var slot = m_availableSlots[0];
            m_availableSlots.RemoveAt(0);
            return slot;
        }

        public void InitShelf(ShelfData shelfData, Action<int, Vector2Int, Vector2Int> onStageBoardChange)
        {
            m_slots.Clear();
            m_availableSlots.Clear();
            m_position = shelfData.position;
            m_shelfType = shelfData.shelfType;

            AddSlot(m_leftSlot);
            AddSlot(m_midSlot);
            AddSlot(m_rightSlot);

            var itemPrefab = GameConfig.Instance.prefabConfig.itemPrefab;
            // Init Item - Add to List

            for (var slotIndex = 0; slotIndex < shelfData.slotDatas.Count; slotIndex++)
            {
                var slotData = shelfData.slotDatas[slotIndex];
                SlotView targetSlot;
                if (shelfData.shelfType == ShelfType.Dispenser && m_midSlot != null)
                {
                    // Always place items at the middle slot for Dispenser shelves
                    targetSlot = m_midSlot;
                    m_availableSlots.Remove(m_midSlot);
                }
                else
                {
                    targetSlot = GetNextAvailableSlot();
                }

                targetSlot.SlotData = slotData;

                for (var i = 0; i < slotData.itemsLists.Count; i++)
                {
                    var items = slotData.itemsLists[i];
                    if (items == -1) continue;
                    var itemData = GameConfig.Instance.itemDataConfig.GetItemDataByID(items);
                    var item = Instantiate(itemPrefab, targetSlot.transform);
                    item.transform.localPosition = Vector3.zero;
                    // Name item for easier tracking in hierarchy
                    string namePrefix = i == 0 ? "TopItem" : $"Item_Index{i}";
                    item.name = $"{namePrefix}_Slot{slotIndex}_ID{itemData.id}";
                    var controller = item.GetComponent<ItemController>();
                    controller.InitItem(itemData, i + 1, this, targetSlot, onStageBoardChange);
                    if (!m_items.ContainsKey(i))
                    {
                        m_items.Add(i, new List<ItemController> { controller });
                    }
                    else
                    {
                        m_items[i].Add(controller);
                    }
                }
            }
        }


        public void RemoveFromShelf(ItemController item)
        {
            RemoveFromShelf(item, item.CurrentSlot);
        }

        public void RemoveFromShelf(ItemController item, SlotView slot)
        {
            if (TopLayer.Contains(item))
            {
                TopLayer.Remove(item);
                if (slot != null)
                {
                    slot.RemoveItem(item);
                    MarkSlotAvailable(slot);
                }
                if (TopLayer.Count == 0)
                {
                    RemoveLayerTop();
                }
            }
        }

        public void RemoveLayerTop()
        {
            var cacheList = m_items.ToList();

            foreach (var itemView in TopLayer)
            {
                if (itemView.CurrentSlot != null)
                {
                    itemView.CurrentSlot.RemoveItem(itemView);
                    MarkSlotAvailable(itemView.CurrentSlot);
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
                    item.LayerIndex = itemLayer.Key + 1;
                }
            }
        }

        
    }
}
