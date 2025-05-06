using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;

namespace GameCore
{
    public class ShelfView : MonoBehaviour
    {
        private Dictionary<int, List<ItemView>> m_items = new(8);
        private Vector2Int m_position;
        [SerializeField] private float m_offsetDistance;
        private List<float> m_positionPlaced = new(4);
        
        public Vector2Int Position => m_position;

        public void InitShelf(ShelfData shelfData, Action<int, Vector2Int, Vector2Int> onStageBoardChange)
        {
            for (var i = 0; i < shelfData.slotDatas.Count; i++)
            {
                m_positionPlaced.Add((i - (shelfData.slotDatas.Count - 1) / 2f) * m_offsetDistance);
            }
            
            var itemPrefab = GameConfig.Instance.prefabConfig.itemPrefab;
            // Init Item - Add to List

            foreach (var slotData in shelfData.slotDatas)
            {
                var posX = m_positionPlaced.First();
                m_positionPlaced.RemoveAt(0);
                for (var i = 0; i < slotData.itemsLists.Count; i++)
                {
                    var items = slotData.itemsLists[i];
                    if(items == -1) continue;
                    var item = Instantiate(itemPrefab, transform);
                    item.transform.localPosition = new Vector3(posX, 0, 0);
                    item.InitItem(items, i + 1, this, onStageBoardChange);
                    if(m_items.ContainsKey(i) == false) m_items.Add(i, new List<ItemView>(){item});
                    else m_items[i].Add(item);
                }
            }
        }

        public bool TryAddToShelf(ItemView item)
        {
            if (m_items.First().Value.Count == 3)
            {
                return false;
            }
            
            item.transform.SetParent(this.transform);
            var posX = m_positionPlaced.Min(pX => Math.Abs(pX - item.transform.localPosition.x));
            item.transform.localPosition = new Vector3(posX, 0, 0);
            m_items.First().Value.Add(item);
            m_positionPlaced.Remove(posX);
            
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