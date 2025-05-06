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

    public class BoardModel
    {
        public List<ShelfData> shelfData;

        public BoardModel(LevelData_Flexible levelData)
        {
            shelfData = levelData.shelfList;
        }
        
        private ShelfData GetShelfData(Vector2Int position) => shelfData.Find(x => x.position == position);

        public bool TryMatch(int itemId, Vector2Int shelfStart, Vector2Int shelfEnd)
        {
            var shelfStartData = GetShelfData(shelfStart);
            var shelfEndData = GetShelfData(shelfEnd);
            
            foreach (var slotData in shelfStartData.slotDatas)
            {
                if (slotData.itemsLists[0] == itemId)
                {
                    slotData.itemsLists[0] = -1;
                    break;
                }
            }

            if (shelfStartData.IsFirstLayerEmpty)
            {
                foreach (var slotData in shelfStartData.slotDatas)
                {
                    if (slotData.itemsLists.Count == 1)
                    {
                        break;
                    }
                    slotData.itemsLists.RemoveAt(0);
                }
            }

            foreach (var slotData in shelfEndData.slotDatas)
            {
                if (slotData.itemsLists[0] == -1)
                {
                    slotData.itemsLists[0] = itemId;
                    if (slotData.itemsLists.TrueForAll(id => id == itemId))
                    {
                        return true;
                    }

                    return false;
                }
            }

            return false;
        }
    }
}