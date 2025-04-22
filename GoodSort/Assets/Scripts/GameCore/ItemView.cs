using System;
using UnityEngine;

namespace GameCore
{
    public class ItemView : MonoBehaviour
    {
        private int m_id;
        [SerializeField] private SpriteRenderer m_icon;
        [SerializeField] private Collider2D m_collider;
        private ShelfView m_curShelfCollide;
        private ShelfView m_curShelf;
        private Action<int, Vector2Int, Vector2Int> m_updateBoardChange;
    
        public void InitItem(int idItem, ShelfView shelfView, Action<int, Vector2Int, Vector2Int> updateBoardChange)
        {
            m_updateBoardChange = updateBoardChange;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Get the Shelf View Collide
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // Check if Out of the Shelf - Return to the old shelf
        }

        public void OnMouseDown()
        {
            
        }

        public void OnMouseDrag()
        {
            
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
            }
        }

        public void SetActiveItem(bool isActive)
        {
            // Set the Layer View - Can click or not - or push to first Layer
        }
    }
}