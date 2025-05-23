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


        public int Id => m_itemData.id;
        public SlotView CurrentSlot => m_curSlot;
        public ShelfView CurrentShelf => m_curShelf;


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
            m_view.SetupView(data);
            SetLayer(layerIndex, layerIndex == 1);
        }

        public void SetLayer(int layerIndex, bool isTopItem)
        {
            m_view.SetLayer(layerIndex, isTopItem);
        }


        public void SetShelfAndSlot(ShelfView shelf, SlotView slot)
        {
            m_curShelf = shelf;
            m_curSlot = slot;
        }
    }
}
