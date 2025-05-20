using System;
using DefaultNamespace;
using UnityEngine;

namespace GameCore
{
    public class ItemView : MonoBehaviour
    {
        private int m_id;
        private ItemData m_itemData;
        [SerializeField] private SpriteRenderer m_icon;
        [SerializeField] private Collider2D m_collider;
        [SerializeField] private float m_heightOffset;
        private ShelfView m_curShelfCollider;
        private CircleCollider2D m_curSlotCollider;
        private ShelfView m_curShelf;
        private Vector3 m_dragOffset;
        private Action<int, Vector2Int, Vector2Int> m_updateBoardChange;

        [SerializeField] private int m_layerIndex;
        private Vector3 m_oldPosition;
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
    
        public void InitItem(ItemData data, int layerIndex, ShelfView shelfView, Action<int, Vector2Int, Vector2Int> updateBoardChange)
        {
            m_updateBoardChange = updateBoardChange;
            m_itemData = data;
            m_id = data.id;
            m_curShelf = shelfView;
            LayerIndex = layerIndex;

            m_icon.sprite = GameConfig.Instance.itemIconConfig.GetItemIconByID(m_id);
            m_icon.enabled = data.visualType == GameCore.ItemVisualType.FullDisplay;
        }

        public void OnMouseDown()
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;
            m_dragOffset = transform.position - mouseWorld;
            m_oldPosition = transform.localPosition;
        }

        public void OnMouseDrag()
        {
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
                var circle = other as CircleCollider2D;
                if (circle != null &&
                    (circle == shelf.leftSlot || circle == shelf.midSlot || circle == shelf.rightSlot))
                {
                    m_curShelfCollider = shelf;
                    m_curSlotCollider = circle;
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
                var circle = other as CircleCollider2D;
                if (circle != null && (circle == shelf.leftSlot || circle == shelf.midSlot || circle == shelf.rightSlot))
                {
                    if (circle == m_curSlotCollider)
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
            // Check the Shelf View Collide - If Null then Return 0 Else Shelf View Receive
            if (m_curShelfCollider != null && m_curSlotCollider != null)
            {
                if (m_curShelfCollider.TryAddToShelf(this, m_curSlotCollider))
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
            else
            {
                transform.localPosition = m_oldPosition;
            }
        }

        private void SetActiveItem(bool isActive)
        {
            m_collider.enabled = isActive;
        }

    }
}
