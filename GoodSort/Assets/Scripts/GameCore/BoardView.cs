using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace GameCore
{
    public class BoardView : MonoBehaviour
    {
        private List<ShelfView> m_shelfViews;
        private BoardController m_boardController;
        
        public void CreateBoard(BoardController boardController)
        {
            m_boardController = boardController;
            var shelfPrefab = GameConfig.Instance.prefabConfig.shelfPrefab;
            // Init Shelf Here - Add Shelf To List
        }

        public void OnStateBoardChange(int idItem, Vector2Int shelfStart, Vector2Int shelfEnd)
        {
            m_boardController.UpdateBoardData(idItem, shelfStart, shelfEnd);
            var listMatchPos = m_boardController.GetShelfPositionsMatched();
            
            // Find All the Shelf View have same Position - Then remove the first layer of it
        }

        public void UserAction(int i)
        {
        }
    }
}