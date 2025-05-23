using System;
using DefaultNamespace;
using UnityEngine;

namespace GameCore
{
    [RequireComponent(typeof(ItemView))]
    public class ItemController : MonoBehaviour
    {
        private ItemData m_itemData;
        private ItemView m_view;

        private ShelfView m_curShelf;
        private SlotView m_curSlot;
        private Action<int, Vector2Int, Vector2Int> m_updateBoardChange;

        [SerializeField] private int m_layerIndex;


        public int Id => m_itemData.id;
        public SlotView CurrentSlot => m_curSlot;
        public ShelfView CurrentShelf => m_curShelf;

        public int LayerIndex
        {
            get => m_layerIndex;
            set
            {
                if (m_layerIndex == value) return;
                m_layerIndex = value;
                m_view.SetLayer(m_layerIndex);
            }
        }

        private void Awake()
        {
            m_view = GetComponent<ItemView>();
        }

        public void InitItem(ItemData data, int layerIndex, ShelfView shelfView, SlotView slotView,
            Action<int, Vector2Int, Vector2Int> updateBoardChange)
        {
            m_updateBoardChange = updateBoardChange;
            m_itemData = data;
            m_curShelf = shelfView;
            m_curSlot = slotView;
            LayerIndex = layerIndex;
            m_view.SetupView(data);
        }


        public void SetShelfAndSlot(ShelfView shelf, SlotView slot)
        {
            m_curShelf = shelf;
            m_curSlot = slot;
        }
    }
}
