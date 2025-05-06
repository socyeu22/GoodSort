using System;
using DefaultNamespace;
using UnityEngine;

namespace GameCore
{
    public class ItemView : MonoBehaviour
    {
        private int m_id;
        [SerializeField] private SpriteRenderer m_icon;
        [SerializeField] private Collider2D m_collider;
        [SerializeField] private float m_heightOffset;
        private ShelfView m_curShelfCollide;
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
    
        public void InitItem(int idItem, int layerIndex, ShelfView shelfView, Action<int, Vector2Int, Vector2Int> updateBoardChange)
        {
            m_updateBoardChange = updateBoardChange;
            m_id = idItem;
            m_curShelf = shelfView;
            LayerIndex = layerIndex;
            
            m_icon.sprite = GameConfig.Instance.itemIconConfig.GetItemIconByID(m_id);
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

            // Raycast from the new item position
            RaycastHit2D hit = Physics2D.Raycast(newPos, Vector2.down, Mathf.Infinity);

            if (hit.collider != null)
            {
                ShelfView shelf = hit.collider.GetComponent<ShelfView>();
                Debug.Log(shelf);
                m_curShelfCollide = shelf;
            }
            else
            {
                m_curShelfCollide = null;
            }
        }
        
        public void OnMouseUp()
        {
            // Check the Shelf View Collide - If Null then Return 0 Else Shelf View Receive
            if (m_curShelfCollide != null)
            {
                if (m_curShelfCollide.TryAddToShelf(this))
                {
                    m_curShelf.RemoveFromShelf(this);
                    m_curShelf = m_curShelfCollide;
                    m_updateBoardChange?.Invoke(m_id, m_curShelfCollide.Position, m_curShelfCollide.Position);
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