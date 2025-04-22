using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace GameCore
{
    public class ShelfView : MonoBehaviour
    {
        private Dictionary<int, ItemView> m_items = new();
        private Vector2Int m_position;
        [SerializeField] private float m_offsetDistance;
        
        public Vector2Int Position => m_position;

        public void InitShelf(ShelfData shelfData, Action<int, Vector2Int, Vector2Int> onStageBoardChange)
        {
            var itemPrefab = GameConfig.Instance.prefabConfig.itemPrefab;
            // Init Item - Add to List
        }

        public bool TryAddToShelf(ItemView item)
        {
            // Check Dict ItemViews  == 3 return False;
            // Count < 3 - Snap to nearest Position and Add to list
            return false;
        }

        public void RemoveFromShelf(ItemView item)
        {
            
        }

        public void RemoveLayerTop()
        {
            
        }
        
    }
}