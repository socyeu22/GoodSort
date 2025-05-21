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

            foreach (var slotData in shelfData.slotDatas)
            {
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

        public bool TryAddToShelf(ItemController item, SlotView slot)
        {
            if (TopLayer.Count == MaxItemsPerLayer)
            {
                return false;
            }

            if (slot == null || !m_slots.Contains(slot))
            {
                return false;
            }

            if (!m_availableSlots.Contains(slot))
            {
                return false;
            }

            slot.AddItem(item);
            item.SetShelfAndSlot(this, slot);
            TopLayer.Add(item);
            m_availableSlots.Remove(slot);

            return true;
        }

        public bool TryAddToShelf(ItemController item)
        {
            if (TopLayer.Count == MaxItemsPerLayer)
            {
                return false;
            }

            SlotView[] order = { m_midSlot, m_rightSlot, m_leftSlot };
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
                    if (!m_availableSlots.Contains(slot))
                    {
                        m_availableSlots.Add(slot);
                    }
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
                    item.LayerIndex = itemLayer.Key + 1;
                }
            }
        }

        public SlotView GetNearestAvailableSlot(Vector3 worldPos)
        {
            if (m_shelfType != ShelfType.Normal)
            {
                return null;
            }

            SlotView nearest = null;
            float minDist = m_offsetDistance;
            foreach (var slot in m_availableSlots)
            {
                float dist = Vector3.Distance(slot.transform.position, worldPos);
                if (dist <= minDist)
                {
                    minDist = dist;
                    nearest = slot;
                }
            }

            return nearest;
        }

        public bool TryGetSnapSlot(Vector3 worldPos, out SlotView slot)
        {
            slot = GetNearestAvailableSlot(worldPos);
            return slot != null;
        }

        public bool TrySnapItem(ItemController item)
        {
            var slot = GetNearestAvailableSlot(item.transform.position);
            if (slot == null)
            {
                return false;
            }
            return TryAddToShelf(item, slot);
        }
        
    }
}
