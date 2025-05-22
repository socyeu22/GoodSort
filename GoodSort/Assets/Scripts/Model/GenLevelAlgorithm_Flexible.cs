using System.Collections.Generic;
using System.Linq;
using GameCore; // Đảm bảo using namespace này
using UnityEngine;
using System;

namespace DefaultNamespace
{
    /// <summary>
    /// Thuật toán tạo level với cấu trúc kệ linh hoạt (List<ShelfData>)
    /// và hỗ trợ kệ Dispenser được định nghĩa sẵn.
    /// </summary>
    public class GenLevelAlgorithm_Flexible
    {
        // --- Input Fields ---
        private readonly List<ShelfData> originalShelfList;
        private readonly Dictionary<int, int> amountPairEachItem;
        private readonly float percentCoverLayer;
        private readonly int depth;

        // --- Output Field ---
        public int finalNumberOfLayers { get; private set; }

        // --- Private Fields ---
        private System.Random rand = new System.Random();
        private int[,] placementBoard;
        private const int MAX_PLACEMENT_ATTEMPTS_PER_ITEM = 500;
        private const int MAX_EXPANSION_CYCLES = 25;
        private Dictionary<int, int> normalShelfListIndexToPlacementRow = new Dictionary<int, int>();

        public GenLevelAlgorithm_Flexible(List<ShelfData> originalShelfList, Dictionary<int, int> amountPairEachItem, float percentCoverLayer, int depth)
        {
            this.originalShelfList = originalShelfList ?? new List<ShelfData>();
            this.amountPairEachItem = amountPairEachItem ?? new Dictionary<int, int>();
            this.percentCoverLayer = percentCoverLayer;
            this.depth = Math.Max(0, depth);
        }

        // ========================================================================
        // Main Public Method
        // ========================================================================
        public List<ShelfData> GenLevel()
        {
            if (!ValidateInput()) return null;

            var preProcessResult = PreProcessShelfData();
            if (preProcessResult == null) return null;

            Dictionary<int, int> normalItemsToPlaceCount = preProcessResult.Value.normalItemsNeeded;
            int normalShelfCount = preProcessResult.Value.normalShelfCount;
            int totalNormalSlots = normalShelfCount * 3;

            if (normalShelfCount == 0)
            {
                // Nếu không có kệ Normal, kiểm tra xem có item nào cần đặt không
                if (normalItemsToPlaceCount.Sum(kvp => kvp.Value) > 0) {
                     Debug.LogError("GenLevelAlgorithm Error: No Normal shelves found, but items are still required according to amountPairEachItem and Dispenser contents. Cannot place remaining items.");
                     return null; // Không thể đặt item còn lại
                }
                Debug.LogWarning("GenLevelAlgorithm Info: No 'Normal' shelves found and no items needed for them. Returning original structure.");
                return CreateFinalShelfListFromOriginal(0);
            }

            int totalNormalItemsToPlace = normalItemsToPlaceCount.Sum(kvp => kvp.Value);
            if (totalNormalItemsToPlace == 0)
            {
                Debug.LogWarning("GenLevelAlgorithm Info: No items needed for 'Normal' shelves. Returning original structure.");
                return CreateFinalShelfListFromOriginal(0);
            }

            int currentNumberOfLayers = CalculateInitialLayers(totalNormalSlots, totalNormalItemsToPlace, percentCoverLayer);
            finalNumberOfLayers = currentNumberOfLayers;

            Debug.Log($"GenLevelAlgorithm Info: Normal Shelves Calculation - Slots Per Layer={totalNormalSlots}, Initial Layers={currentNumberOfLayers}, Total Normal Items={totalNormalItemsToPlace}");

            placementBoard = new int[totalNormalSlots, currentNumberOfLayers];
            InitializeBoard(placementBoard, -1);
            BuildNormalShelfMapping();

            List<int> itemsToPlaceOnNormal = CreateFlatItemList(normalItemsToPlaceCount);
            bool placementSuccess = PlaceItemsOnNormalShelves(itemsToPlaceOnNormal, totalNormalSlots, ref currentNumberOfLayers);

            if (!placementSuccess) return null;

            finalNumberOfLayers = currentNumberOfLayers;
            Debug.Log($"GenLevelAlgorithm Info: Placement successful! Final number of layers for Normal shelves: {finalNumberOfLayers}");
            List<ShelfData> finalShelfList = CreateFinalShelfListFromMapping(finalNumberOfLayers);
            return finalShelfList;
        }


        // ========================================================================
        // Pre-processing Method
        // ========================================================================
        private struct PreProcessInfo { public Dictionary<int, int> normalItemsNeeded; public int normalShelfCount; }

        private PreProcessInfo? PreProcessShelfData()
        {
            Dictionary<int, int> dispenserCounts = new Dictionary<int, int>();
            int normalShelfCount = 0;
            bool hasInvalidDispenser = false;

            foreach (var shelf in originalShelfList)
            {
                if (shelf.shelfType == ShelfType.Dispenser)
                {
                    if (shelf.slotDatas == null || shelf.slotDatas.Count != 1 || shelf.slotDatas[0]?.itemsLists == null)
                    {
                        Debug.LogError($"GenLevelAlgorithm Error: Dispenser shelf at position {shelf.position} has invalid SlotData structure.");
                        hasInvalidDispenser = true; continue;
                    }
                    foreach (int itemID in shelf.slotDatas[0].itemsLists)
                    {
                        if (itemID == -1) continue;
                        if (!dispenserCounts.TryGetValue(itemID, out int count))
                        {
                            count = 0;
                        }
                        dispenserCounts[itemID] = count + 1;
                    }
                }
                else if (shelf.shelfType == ShelfType.Normal) { normalShelfCount++; }
            }
            if (hasInvalidDispenser) return null;

            Dictionary<int, int> normalItemsNeeded = new Dictionary<int, int>();
            bool calculationError = false;
            foreach (var kvp in amountPairEachItem)
            {
                int itemID = kvp.Key; int totalTriplesNeeded = kvp.Value;
                if (totalTriplesNeeded < 0) { Debug.LogError($"ItemID {itemID} has negative triples."); calculationError = true; continue; }
                int targetTotalItems = totalTriplesNeeded * 3;
                dispenserCounts.TryGetValue(itemID, out int countInDispensers);
                int neededForNormal = targetTotalItems - countInDispensers;

                if (neededForNormal < 0)
                {
                    Debug.LogError($"ItemID {itemID}: More in Dispensers ({countInDispensers}) than total required ({targetTotalItems}).");
                    calculationError = true;
                }
                // SỬA ĐỔI: Xóa bỏ kiểm tra neededForNormal % 3 != 0
                // else if (neededForNormal % 3 != 0)
                // {
                //     Debug.LogError($"ItemID {itemID}: Needed for Normal ({neededForNormal}) not divisible by 3. Dispenser: {countInDispensers}, Target: {targetTotalItems}.");
                //     calculationError = true;
                // }
                else if (neededForNormal > 0) // Chỉ cần kiểm tra > 0
                {
                    normalItemsNeeded[itemID] = neededForNormal;
                }
                // Nếu neededForNormal = 0, không cần làm gì cả
            }
            foreach(var kvp in dispenserCounts) { if (!amountPairEachItem.ContainsKey(kvp.Key)) Debug.LogWarning($"ItemID {kvp.Key} in Dispensers but not in amountPairEachItem."); }
            if (calculationError) return null;
            return new PreProcessInfo { normalItemsNeeded = normalItemsNeeded, normalShelfCount = normalShelfCount };
        }

        // ========================================================================
        // Item Placement Logic (Không đổi)
        // ========================================================================
        private List<int> CreateFlatItemList(Dictionary<int, int> itemsCount)
        {
            List<int> flatList = new List<int>(itemsCount.Sum(kvp => kvp.Value));
            foreach (var kvp in itemsCount) { for (int i = 0; i < kvp.Value; i++) flatList.Add(kvp.Key); }
            for (int i = flatList.Count - 1; i > 0; i--) { int j = rand.Next(i + 1); (flatList[i], flatList[j]) = (flatList[j], flatList[i]); }
            return flatList;
        }

        private bool PlaceItemsOnNormalShelves(List<int> itemsToPlace, int slotsPerLayer, ref int currentNumberOfLayers)
        {
            Dictionary<int, (int min, int max)> itemLayerBounds = new Dictionary<int, (int, int)>();
            foreach (int itemID in itemsToPlace)
            {
                if (!itemLayerBounds.TryGetValue(itemID, out var bounds))
                {
                    bounds = (-1, -1);
                }
                (int minLayerPlaced, int maxLayerPlaced) = bounds;
                bool placed = AttemptPlaceSingleItemStrictDepth(itemID, slotsPerLayer, ref currentNumberOfLayers, ref minLayerPlaced, ref maxLayerPlaced);
                if (!placed) return false;
                itemLayerBounds[itemID] = (minLayerPlaced, maxLayerPlaced);
            }
            return true;
        }

        private bool AttemptPlaceSingleItemStrictDepth(int itemID, int slotsPerLayer, ref int currentNumberOfLayers, ref int minLayerPlaced, ref int maxLayerPlaced)
        {
            int expansionCycle = 0;
            while (expansionCycle < MAX_EXPANSION_CYCLES)
            {
                expansionCycle++;
                var (lowerBound, upperBound) = CalculatePlacementBoundsStrict(currentNumberOfLayers, minLayerPlaced, maxLayerPlaced, depth);
                var (placedInBounds, layerPlacedInBounds) = TryPlaceInBounds(itemID, slotsPerLayer, lowerBound, upperBound);
                if (placedInBounds) { UpdateMinMaxLayers(layerPlacedInBounds, ref minLayerPlaced, ref maxLayerPlaced); return true; }
                Debug.LogWarning($"Cannot place item {itemID} in bounds [{lowerBound},{upperBound}). Expanding layers {currentNumberOfLayers}->{currentNumberOfLayers + 1}. Cycle {expansionCycle}");
                ExpandPlacementBoard(slotsPerLayer, ref currentNumberOfLayers);
                // Debug.Log($"Board expanded. Retrying placement for item {itemID}.");
            }
            Debug.LogError($"Failed to place item {itemID} after {MAX_EXPANSION_CYCLES} expansions.");
            return false;
        }

        private (bool, int) TryPlaceInBounds(int itemID, int slotsPerLayer, int lowerBoundLayer, int upperBoundLayer)
        {
            if (upperBoundLayer <= lowerBoundLayer) return (false, -1);
            for (int attempt = 0; attempt < MAX_PLACEMENT_ATTEMPTS_PER_ITEM; attempt++)
            {
                int slotIdx = rand.Next(0, slotsPerLayer);
                int layerIdx = rand.Next(lowerBoundLayer, upperBoundLayer);
                if (placementBoard[slotIdx, layerIdx] == -1) { placementBoard[slotIdx, layerIdx] = itemID; return (true, layerIdx); }
            }
            return (false, -1);
        }

        private void UpdateMinMaxLayers(int currentLayer, ref int minLayer, ref int maxLayer)
        {
            if (minLayer == -1) { minLayer = currentLayer; maxLayer = currentLayer; }
            else { minLayer = Math.Min(minLayer, currentLayer); maxLayer = Math.Max(maxLayer, currentLayer); }
        }

        private (int, int) CalculatePlacementBoundsStrict(int currentNumberOfLayers, int minLayerPlaced, int maxLayerPlaced, int depthValue)
        {
             if (minLayerPlaced == -1) return (0, currentNumberOfLayers);
             int currentRange = maxLayerPlaced - minLayerPlaced;
             int lowerBound; int upperBound;
             if (currentRange >= depthValue) { lowerBound = minLayerPlaced; upperBound = maxLayerPlaced + 1; }
             else { lowerBound = Math.Max(0, minLayerPlaced - (depthValue - currentRange)); upperBound = Math.Min(currentNumberOfLayers, maxLayerPlaced + (depthValue - currentRange) + 1); }
              if (upperBound <= lowerBound) {
                  if (maxLayerPlaced >= minLayerPlaced) { lowerBound = minLayerPlaced; upperBound = maxLayerPlaced + 1; Debug.LogWarning($"Strict bounds invalid. Fallback to current range [{minLayerPlaced},{maxLayerPlaced+1})."); }
                  else { lowerBound = 0; upperBound = currentNumberOfLayers; Debug.LogError($"Strict bounds invalid & min>max. Fallback to full range."); }
                  if (upperBound <= lowerBound && currentNumberOfLayers > lowerBound) upperBound = lowerBound + 1;
                  else if (upperBound <= lowerBound) { /* Will trigger expansion */ }
              }
             return (lowerBound, upperBound);
        }


        // ========================================================================
        // Board Manipulation & Mapping Methods (Không đổi)
        // ========================================================================
        private void ExpandPlacementBoard(int slotsPerLayer, ref int currentNumberOfLayers)
        {
            int oldNumberOfLayers = currentNumberOfLayers; currentNumberOfLayers++;
            int[,] newBoard = new int[slotsPerLayer, currentNumberOfLayers]; InitializeBoard(newBoard, -1);
            int rowsToCopy = Math.Min(slotsPerLayer, placementBoard.GetLength(0)); int colsToCopy = Math.Min(oldNumberOfLayers, placementBoard.GetLength(1));
            for (int r = 0; r < rowsToCopy; r++) { for (int c = 0; c < colsToCopy; c++) newBoard[r, c] = placementBoard[r, c]; }
            placementBoard = newBoard;
        }

        private void InitializeBoard(int[,] board, int initialValue)
        {
             int rows = board.GetLength(0); int cols = board.GetLength(1);
             for (int i = 0; i < rows; i++) { for (int j = 0; j < cols; j++) board[i, j] = initialValue; }
        }

        private void BuildNormalShelfMapping()
        {
            normalShelfListIndexToPlacementRow.Clear(); int currentPlacementRow = 0;
            for (int i = 0; i < originalShelfList.Count; i++) { if (originalShelfList[i].shelfType == ShelfType.Normal) { normalShelfListIndexToPlacementRow[i] = currentPlacementRow; currentPlacementRow += 3; } }
        }

        private List<ShelfData> CreateFinalShelfListFromMapping(int numberOfLayers)
        {
            List<ShelfData> finalShelfList = new List<ShelfData>(originalShelfList.Count);
            for (int i = 0; i < originalShelfList.Count; i++)
            {
                ShelfData originalShelf = originalShelfList[i]; ShelfData newShelf = new ShelfData(originalShelf.shelfType, originalShelf.position);
                if (originalShelf.shelfType == ShelfType.Normal) {
                    if (normalShelfListIndexToPlacementRow.TryGetValue(i, out int startRowIndex)) {
                        for (int h = 0; h < 3; h++) {
                            SlotData slotData = new SlotData { itemsLists = new List<int>(numberOfLayers) }; int currentRow = startRowIndex + h;
                            if (currentRow < placementBoard.GetLength(0)) {
                                for (int k = 0; k < numberOfLayers; k++) {
                                    if (k < placementBoard.GetLength(1)) slotData.itemsLists.Add(placementBoard[currentRow, k]);
                                    else { slotData.itemsLists.Add(-1); Debug.LogWarning($"Layer {k} out of bounds ({placementBoard.GetLength(1)}) during mapping."); }
                                }
                            } else { Debug.LogError($"Row index {currentRow} out of bounds ({placementBoard.GetLength(0)}). Mapping failed for shelf index {i}."); for (int k = 0; k < numberOfLayers; k++) slotData.itemsLists.Add(-1); }
                            newShelf.slotDatas.Add(slotData);
                        }
                    } else { Debug.LogError($"No mapping for Normal shelf index {i}."); for (int h = 0; h < 3; h++) newShelf.slotDatas.Add(new SlotData()); }
                } else if (originalShelf.shelfType == ShelfType.Dispenser) {
                    if (originalShelf.slotDatas != null && originalShelf.slotDatas.Count == 1 && originalShelf.slotDatas[0]?.itemsLists != null) newShelf.slotDatas.Add(new SlotData { itemsLists = new List<int>(originalShelf.slotDatas[0].itemsLists) });
                    else { Debug.LogError($"Dispenser shelf at pos {originalShelf.position} (index {i}) has invalid data."); newShelf.slotDatas.Add(new SlotData()); }
                }
                newShelf.UpdateTopItemIds();
                finalShelfList.Add(newShelf);
            }
            return finalShelfList;
        }

        private List<ShelfData> CreateFinalShelfListFromOriginal(int numberOfLayersForEmptyNormal)
        {
             List<ShelfData> finalShelfList = new List<ShelfData>(originalShelfList.Count);
             foreach (var originalShelf in originalShelfList) {
                 ShelfData newShelf = new ShelfData(originalShelf.shelfType, originalShelf.position);
                 if (originalShelf.shelfType == ShelfType.Dispenser) {
                     if (originalShelf.slotDatas != null && originalShelf.slotDatas.Count == 1 && originalShelf.slotDatas[0]?.itemsLists != null) newShelf.slotDatas.Add(new SlotData { itemsLists = new List<int>(originalShelf.slotDatas[0].itemsLists) });
                     else { Debug.LogError($"Dispenser shelf at pos {originalShelf.position} has invalid data."); newShelf.slotDatas.Add(new SlotData()); }
                 } else if (originalShelf.shelfType == ShelfType.Normal) {
                     for(int h=0; h<3; h++) { var items = new List<int>(numberOfLayersForEmptyNormal); for(int k=0; k<numberOfLayersForEmptyNormal; k++) items.Add(-1); newShelf.slotDatas.Add(new SlotData{ itemsLists = items }); }
                 }
                 newShelf.UpdateTopItemIds();
                 finalShelfList.Add(newShelf);
             }
             return finalShelfList;
        }


        // ========================================================================
        // Calculation & Helper Methods
        // ========================================================================
        private bool ValidateInput()
        {
            bool isValid = true;
            if (originalShelfList == null) { Debug.LogError("GenLevelAlgorithm Input Error: originalShelfList is null."); isValid = false; }
            // depth đã được xử lý trong constructor
            return isValid;
        }

        private int GetSlotsPerLayer() {
             int count = 0; if (originalShelfList == null) return 0;
             foreach(var shelf in originalShelfList) { if (shelf.shelfType == ShelfType.Normal) count++; }
             return count * 3;
        }
        private int CalculateInitialLayers(int slotsPerLayer, int totalItems, float coverPercent) {
             if (slotsPerLayer <= 0) return 1; int maxItemsPerLayer = slotsPerLayer;
             float avgItemsPerLayer = maxItemsPerLayer * Mathf.Clamp(coverPercent, 0f, 1f);
             int layers = (avgItemsPerLayer > 0.01f) ? (int)Math.Ceiling((float)totalItems / avgItemsPerLayer) : totalItems;
             return Math.Max(1, layers);
        }
    }
}
