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
            foreach (var shelf in shelfData)
            {
                shelf.UpdateTopItemIds();
            }
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
                    if (slotData.itemsLists.Count == 1)
                    {
                        slotData.itemsLists[0] = -1;
                    }
                    else
                    {
                        slotData.itemsLists[0] = slotData.itemsLists[1];
                        slotData.itemsLists.RemoveAt(1);
                    }
                    break;
                }
            }
            shelfStartData.UpdateTopItemIds();

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
                shelfStartData.UpdateTopItemIds();
            }

            bool added = false;
            foreach (var slotData in shelfEndData.slotDatas)
            {
                if (slotData.itemsLists[0] == -1)
                {
                    slotData.itemsLists[0] = itemId;
                    added = true;
                    break;
                }
            }
            shelfEndData.UpdateTopItemIds();

            if (!added) return false;

            bool match = true;
            foreach (var slot in shelfEndData.slotDatas)
            {
                if (slot.itemsLists[0] != itemId)
                {
                    match = false;
                    break;
                }
            }

            if (match)
            {
                foreach (var slot in shelfEndData.slotDatas)
                {
                    slot.itemsLists[0] = -1;
                }
                shelfEndData.UpdateTopItemIds();

                if (shelfEndData.IsFirstLayerEmpty)
                {
                    foreach (var slot in shelfEndData.slotDatas)
                    {
                        if (slot.itemsLists.Count > 1)
                        {
                            slot.itemsLists.RemoveAt(0);
                        }
                    }
                    shelfEndData.UpdateTopItemIds();
                }
            }

            return match;
        }
    }
}