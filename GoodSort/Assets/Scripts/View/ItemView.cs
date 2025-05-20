using System;
using DefaultNamespace;
using UnityEngine;

namespace GameCore
{
    public class ItemView : MonoBehaviour
    {
        private int m_id;
        public int Id => m_id;
        private ItemData m_itemData;
        [SerializeField] private SpriteRenderer m_icon;
        [SerializeField] private Collider2D m_collider;
        [SerializeField] private float m_heightOffset;
        private ShelfView m_curShelfCollider;
        private SlotView m_curSlotCollider;
        private ShelfView m_curShelf;
        private SlotView m_curSlot;
        private Vector3 m_dragOffset;
        private Action<int, Vector2Int, Vector2Int> m_updateBoardChange;

        [SerializeField] private int m_layerIndex;
        private Vector3 m_oldPosition;
        private bool m_isDragging;
        public SlotView CurrentSlot => m_curSlot;
        public ShelfView CurrentShelf => m_curShelf;
        public int LayerIndex
        {
            get => m_layerIndex;
            set
            {
                if (m_layerIndex == value)
                {
                    return;
                }
                
                m_layerIndex = value;
                m_icon.color = m_layerIndex == 1 ? Color.white : Color.gray;
                m_icon.sortingOrder = 10 - m_layerIndex;
                transform.localPosition = new Vector3(transform.localPosition.x, (m_layerIndex - 1) * m_heightOffset, 0f);
                SetActiveItem(m_layerIndex == 1);
            }
        }
    
        public void InitItem(ItemData data, int layerIndex, ShelfView shelfView, SlotView slotView,
            Action<int, Vector2Int, Vector2Int> updateBoardChange)
        {
            m_updateBoardChange = updateBoardChange;
            m_itemData = data;
            m_id = data.id;
            m_curShelf = shelfView;
            m_curSlot = slotView;
            LayerIndex = layerIndex;

            m_icon.sprite = GameConfig.Instance.itemIconConfig.GetItemIconByID(m_id);
            m_icon.enabled = data.visualType == GameCore.ItemVisualType.FullDisplay;
        }

        public void OnMouseDown()
        {
            if (LayerIndex != 1)
            {
                return;
            }

            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;
            m_dragOffset = transform.position - mouseWorld;
            m_oldPosition = transform.localPosition;
            // Ensure dragged item renders on top of others
            m_icon.sortingOrder = 100;
            m_isDragging = true;
        }

        public void OnMouseDrag()
        {
            if (!m_isDragging)
            {
                return;
            }
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;
            Vector3 newPos = mouseWorld + m_dragOffset;
            transform.position = newPos;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            ShelfView shelf = other.GetComponentInParent<ShelfView>();
            if (shelf != null)
            {
                SlotView slot = other.GetComponent<SlotView>();
                if (slot != null)
                {
                    m_curShelfCollider = shelf;
                    m_curSlotCollider = slot;
                }
                else
                {
                    m_curShelfCollider = shelf;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            ShelfView shelf = other.GetComponentInParent<ShelfView>();
            if (shelf != null)
            {
                SlotView slot = other.GetComponent<SlotView>();
                if (slot != null)
                {
                    if (slot == m_curSlotCollider)
                    {
                        m_curSlotCollider = null;
                    }
                }
                else if (shelf == m_curShelfCollider)
                {
                    m_curShelfCollider = null;
                    m_curSlotCollider = null;
                }
            }
        }
        
        public void OnMouseUp()
        {
            if (!m_isDragging)
            {
                return;
            }
            m_isDragging = false;
            // Reset order after dragging
            m_icon.sortingOrder = 10 - m_layerIndex;

            bool added = false;
            if (m_curShelfCollider != null)
            {
                if (m_curSlotCollider != null)
                {
                    added = m_curShelfCollider.TryAddToShelf(this, m_curSlotCollider);
                }
                else
                {
                    added = m_curShelfCollider.TryAddToShelf(this);
                }
            }

            if (added)
            {
                Vector2Int startPos = m_curShelf.Position;
                m_curShelf.RemoveFromShelf(this);
                m_curShelf = m_curShelfCollider;
                m_updateBoardChange?.Invoke(m_id, startPos, m_curShelfCollider.Position);
            }
            else
            {
                transform.localPosition = m_oldPosition;
            }
        }

        private void SetActiveItem(bool isActive)
        {
            m_collider.enabled = isActive;
        }

        public void SetShelfAndSlot(ShelfView shelf, SlotView slot)
        {
            m_curShelf = shelf;
            m_curSlot = slot;
        }

    }
}
