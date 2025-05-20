using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
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