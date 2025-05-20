using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameCore
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private BoardView m_boardView;
        private BoardModel m_boardModel;
        public List<ShelfData> ShelfData => m_boardModel.shelfData;

        public void InitBoard(LevelData_Flexible levelData)
        {
            m_boardModel = new BoardModel(levelData);
            m_boardView.CreateBoard(this);
        }

        public void UpdateBoardData(int itemId, Vector2Int shelfStart, Vector2Int shelfEnd, out bool positionMatch)
        {
            positionMatch = m_boardModel.TryMatch(itemId, shelfStart, shelfEnd);
        }
    }
}