using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameCore
{
    [RequireComponent(typeof(ItemView))]
    public class ItemController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private ItemData m_itemData;
        private ItemView m_view;

        private ShelfView m_curShelfCollider;
        private SlotView m_curSlotCollider;
        private ShelfView m_curShelf;
        private SlotView m_curSlot;
        private Vector3 m_dragOffset;
        private Action<int, Vector2Int, Vector2Int> m_updateBoardChange;

        [SerializeField] private int m_layerIndex;
        private Vector3 m_oldPosition;
        private bool m_isDragging;

        private void BeginDrag(Vector3 pointerWorldPosition)
        {
            if (LayerIndex != 1)
            {
                return;
            }

            Vector3 pos = pointerWorldPosition;
            pos.z = 0f;
            m_dragOffset = transform.position - pos;
            m_oldPosition = transform.localPosition;
            m_view.OnDragStart();
            m_isDragging = true;
        }

        private void UpdateDragPosition(Vector3 pointerWorldPosition)
        {
            if (!m_isDragging)
            {
                return;
            }
            Vector3 pos = pointerWorldPosition;
            pos.z = 0f;
            Vector3 newPos = pos + m_dragOffset;
            transform.position = newPos;
        }

        private void EndDragInternal()
        {
            if (!m_isDragging)
            {
                return;
            }
            m_isDragging = false;
            m_view.OnDragEnd(m_layerIndex);

            bool added = false;
            ShelfView targetShelf = null;
            Vector2Int startPos = m_curShelf.Position;

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

                if (added)
                {
                    targetShelf = m_curShelfCollider;
                }
            }

            if (!added)
            {
                foreach (var shelf in FindObjectsOfType<ShelfView>())
                {
                    if (shelf.TrySnapItem(this))
                    {
                        added = true;
                        targetShelf = shelf;
                        break;
                    }
                }
            }

            if (added && targetShelf != null)
            {
                m_curShelf.RemoveFromShelf(this);
                m_curShelf = targetShelf;
                m_curShelfCollider = targetShelf;
                m_updateBoardChange?.Invoke(Id, startPos, targetShelf.Position);
            }
            else
            {
                transform.localPosition = m_oldPosition;
            }
        }

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

        public void OnBeginDrag(PointerEventData eventData)
        {
            BeginDrag(Camera.main.ScreenToWorldPoint(eventData.position));
        }

        public void OnDrag(PointerEventData eventData)
        {
            UpdateDragPosition(Camera.main.ScreenToWorldPoint(eventData.position));
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

        public void OnEndDrag(PointerEventData eventData)
        {
            EndDragInternal();
        }

        private void OnMouseDown()
        {
            BeginDrag(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

        private void OnMouseDrag()
        {
            UpdateDragPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

        private void OnMouseUp()
        {
            EndDragInternal();
        }

        public void SetShelfAndSlot(ShelfView shelf, SlotView slot)
        {
            m_curShelf = shelf;
            m_curSlot = slot;
        }
    }
}
