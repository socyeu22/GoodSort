using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private BoardView m_boardView;
        private BoardModel m_boardModel;

        public void InitBoard(LevelData levelData)
        {
            m_boardView.CreateBoard(this);
            m_boardModel = new BoardModel(levelData);
        }

        public void UpdateBoardData(int itemId, Vector2Int shelfStart, Vector2Int shelfEnd)
        {
            
        }

        public List<Vector2Int> GetShelfPositionsMatched()
        {
            var list = new List<Vector2Int>();
            // Check inside Board Data to see what will match and return Position for View Update
            return list;
        }
    }

    public class BoardModel
    {
        public List<ShelfData> shelfDatas;

        public BoardModel(LevelData levelData)
        {
            shelfDatas = new List<ShelfData>();
        }

        public void MatchShelf()
        {
            
        }
    }

    public enum ShelfType
    {
        Empty, Normal, Lock
    }
}