using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;

namespace GameCore
{
    public class ShelfView : MonoBehaviour
    {
        private Dictionary<int, List<ItemView>> m_items = new Dictionary<int, List<ItemView>>(8);
        private Vector2Int m_position;
        [SerializeField] private float m_offsetDistance;
        private List<float> m_positionPlaced = new List<float>(4);
        
        public Vector2Int Position => m_position;
        
        public CircleCollider2D midSlot;
        public CircleCollider2D rightSlot;
        public CircleCollider2D leftSlot;

        private Dictionary<CircleCollider2D, float> m_slotPositions = new Dictionary<CircleCollider2D, float>(3);

        public void InitShelf(ShelfData shelfData, Action<int, Vector2Int, Vector2Int> onStageBoardChange)
        {
            m_slotPositions.Clear();
            m_positionPlaced.Clear();
            m_position = shelfData.position;

            // Collect slot positions based on their local position in the prefab
            // Order: left - mid - right to match visual layout
            if (leftSlot != null)
            {
                float leftPos = leftSlot.transform.localPosition.x;
                m_slotPositions[leftSlot] = leftPos;
                m_positionPlaced.Add(leftPos);
            }

            if (midSlot != null)
            {
                float midPos = midSlot.transform.localPosition.x;
                m_slotPositions[midSlot] = midPos;
                m_positionPlaced.Add(midPos);
            }

            if (rightSlot != null)
            {
                float rightPos = rightSlot.transform.localPosition.x;
                m_slotPositions[rightSlot] = rightPos;
                m_positionPlaced.Add(rightPos);
            }

            var itemPrefab = GameConfig.Instance.prefabConfig.itemPrefab;
            // Init Item - Add to List

            foreach (var slotData in shelfData.slotDatas)
            {
                float posX;
                if (shelfData.shelfType == ShelfType.Dispenser && midSlot != null && m_slotPositions.ContainsKey(midSlot))
                {
                    // Always place items at the middle slot for Dispenser shelves
                    posX = m_slotPositions[midSlot];
                }
                else
                {
                    posX = m_positionPlaced.First();
                    m_positionPlaced.RemoveAt(0);
                }
                for (var i = 0; i < slotData.itemsLists.Count; i++)
                {
                    var items = slotData.itemsLists[i];
                    if (items == -1) continue;
                    var itemData = GameConfig.Instance.itemDataConfig.GetItemDataByID(items);
                    var item = Instantiate(itemPrefab, transform);
                    item.transform.localPosition = new Vector3(posX, 0, 0);
                    item.InitItem(itemData, i + 1, this, onStageBoardChange);
                    if (m_items.ContainsKey(i) == false) m_items.Add(i, new List<ItemView>() { item });
                    else m_items[i].Add(item);
                }
            }
        }

        public bool TryAddToShelf(ItemView item, CircleCollider2D slot)
        {
            if (m_items.First().Value.Count == 3)
            {
                return false;
            }

            if (slot == null || m_slotPositions.ContainsKey(slot) == false)
            {
                return false;
            }

            float targetX = m_slotPositions[slot];
            if (!m_positionPlaced.Contains(targetX))
            {
                return false;
            }

            item.transform.SetParent(this.transform);
            item.transform.localPosition = new Vector3(targetX, 0, 0);
            m_items.First().Value.Add(item);
            m_positionPlaced.Remove(targetX);

            return true;
        }

        public void RemoveFromShelf(ItemView item)
        {
            if (m_items.First().Value.Contains(item))
            {
                m_items.First().Value.Remove(item);
                m_positionPlaced.Add(item.transform.localPosition.x);
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
                itemView.gameObject.SetActive(false);
            }
            
            for (int i = 0; i < cacheList.Count - 1; i++)
            {
                m_items[cacheList[i].Key] = cacheList[i + 1].Value;
            }

            // Optionally remove the last key or keep its original value
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
