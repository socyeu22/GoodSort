using System.Collections.Generic;
using System.Linq;
using GameCore;
using UnityEngine;
using System;

namespace DefaultNamespace
{
    public class GenLevelAlgorithm
    {
        // --- Input Fields ---
        private readonly Vector2Int boardSize;
        private readonly ShelfData[,] originalBoardData;
        private readonly Dictionary<int, int> amountPairEachItem;
        private readonly float percentCoverLayer;
        private readonly int depth;

        // --- Output Field ---
        public int finalNumberOfLayers { get; private set; }

        // --- Private Fields ---
        private System.Random rand = new System.Random();
        private int[,] placementBoard;
        private const int MAX_PLACEMENT_ATTEMPTS_PER_ITEM = 500;
        private const int MAX_EXPANSION_CYCLES = 25; // Tăng nhẹ phòng trường hợp depth quá chặt

        public GenLevelAlgorithm(Vector2Int boardSize, ShelfData[,] originalBoardData, Dictionary<int, int> amountPairEachItem, float percentCoverLayer, int depth)
        {
            this.boardSize = boardSize;
            this.originalBoardData = originalBoardData;
            this.amountPairEachItem = amountPairEachItem;
            this.percentCoverLayer = percentCoverLayer;
            // Đảm bảo depth hợp lệ (ít nhất là 0)
            this.depth = Math.Max(0, depth);
        }

        // ========================================================================
        // Main Public Method
        // ========================================================================
        public ShelfData[,] GenLevel()
        {
            if (!ValidateInput()) return null;

            int slotsPerLayer = GetSlotsPerLayer();
            if (slotsPerLayer <= 0)
            {
                Debug.LogError("GenLevelAlgorithm Error: No valid placement slots found (slotsPerLayer <= 0).");
                return null;
            }

            if (amountPairEachItem == null || amountPairEachItem.Count == 0)
            {
                Debug.LogWarning("GenLevelAlgorithm Info: amountPairEachItem is null or empty. Creating an empty board.");
                return CreateEmptyFinalBoard(slotsPerLayer);
            }

            int amountItems = amountPairEachItem.Sum(pair => pair.Value * 3);
            int currentNumberOfLayers = CalculateInitialLayers(slotsPerLayer, amountItems, percentCoverLayer);
            finalNumberOfLayers = currentNumberOfLayers;

            Debug.Log($"GenLevelAlgorithm Info: Initial Calculation - Slots Per Layer={slotsPerLayer}, Initial Layers={currentNumberOfLayers}, Total Items={amountItems}");

            placementBoard = new int[slotsPerLayer, currentNumberOfLayers];
            InitializeBoard(placementBoard, -1);

            var cloneAmountPair = amountPairEachItem.ToDictionary(entry => entry.Key, entry => entry.Value);

            foreach (var itemIDPair in cloneAmountPair)
            {
                int itemID = itemIDPair.Key;
                int itemsToPlace = itemIDPair.Value * 3;
                bool success = PlaceAllItemsOfType(itemID, itemsToPlace, slotsPerLayer, ref currentNumberOfLayers);
                if (!success)
                {
                    return null;
                }
            }

            finalNumberOfLayers = currentNumberOfLayers;
            Debug.Log($"GenLevelAlgorithm Info: Placement successful! Final number of layers: {finalNumberOfLayers}");
            ShelfData[,] finalBoardData = CreateAndMapFinalBoard(slotsPerLayer, finalNumberOfLayers);
            return finalBoardData;
        }

        // ========================================================================
        // Core Placement Logic Methods
        // ========================================================================
        private bool PlaceAllItemsOfType(int itemID, int itemsToPlace, int slotsPerLayer, ref int currentNumberOfLayers)
        {
            int minLayerPlaced = -1;
            int maxLayerPlaced = -1;
            for (int i = 0; i < itemsToPlace; i++)
            {
                bool placed = AttemptPlaceSingleItemStrictDepth(itemID, slotsPerLayer, ref currentNumberOfLayers, ref minLayerPlaced, ref maxLayerPlaced);
                if (!placed)
                {
                    return false;
                }
            }
            return true;
        }

        private bool AttemptPlaceSingleItemStrictDepth(int itemID, int slotsPerLayer, ref int currentNumberOfLayers, ref int minLayerPlaced, ref int maxLayerPlaced)
        {
            int expansionCycle = 0;
            while (expansionCycle < MAX_EXPANSION_CYCLES)
            {
                expansionCycle++;
                // 1. Tính khoảng layer hợp lệ (SỬ DỤNG LOGIC MỚI)
                var (lowerBound, upperBound) = CalculatePlacementBoundsStrict(currentNumberOfLayers, minLayerPlaced, maxLayerPlaced, depth);

                // 2. Chỉ thử đặt trong khoảng giới hạn
                var (placedInBounds, layerPlacedInBounds) = TryPlaceInBounds(itemID, slotsPerLayer, lowerBound, upperBound);
                if (placedInBounds)
                {
                    UpdateMinMaxLayers(layerPlacedInBounds, ref minLayerPlaced, ref maxLayerPlaced);
                    return true; // Thành công!
                }

                // 3. Nếu không đặt được trong bounds -> Mở rộng bảng NGAY LẬP TỨC
                Debug.LogWarning($"GenLevelAlgorithm Warning: Cannot place item {itemID} within strict bounds [{lowerBound},{upperBound}). Expanding layers from {currentNumberOfLayers} to {currentNumberOfLayers + 1}. (Cycle {expansionCycle}/{MAX_EXPANSION_CYCLES})");
                ExpandPlacementBoard(slotsPerLayer, ref currentNumberOfLayers);
                // Quan trọng: Sau khi mở rộng, min/max layer cũ vẫn giữ nguyên,
                // nhưng currentNumberOfLayers đã tăng, CalculatePlacementBoundsStrict sẽ tính lại ở vòng lặp sau.
                Debug.Log($"GenLevelAlgorithm Info: Board expanded. Retrying placement for item {itemID}.");
            }

            Debug.LogError($"GenLevelAlgorithm Error: Failed to place item {itemID} after {MAX_EXPANSION_CYCLES} expansion cycles while strictly enforcing depth. The board might be too constrained or an unexpected error occurred.");
            return false;
        }

        private (bool, int) TryPlaceInBounds(int itemID, int slotsPerLayer, int lowerBoundLayer, int upperBoundLayer)
        {
            // Kiểm tra nếu khoảng không hợp lệ (ví dụ: khi depth=0 và min!=max làm upperBound<=lowerBound)
            if (upperBoundLayer <= lowerBoundLayer)
            {
                 // Trong trường hợp này, không có vị trí hợp lệ nào theo ràng buộc depth
                 // Trả về false để kích hoạt mở rộng bảng
                return (false, -1);
            }

            for (int attempt = 0; attempt < MAX_PLACEMENT_ATTEMPTS_PER_ITEM; attempt++)
            {
                int slotIdx = rand.Next(0, slotsPerLayer);
                int layerIdx = rand.Next(lowerBoundLayer, upperBoundLayer);
                if (placementBoard[slotIdx, layerIdx] == -1)
                {
                    placementBoard[slotIdx, layerIdx] = itemID;
                    return (true, layerIdx);
                }
            }
            // Không tìm thấy chỗ trống trong khoảng giới hạn sau nhiều lần thử
            return (false, -1);
        }


        // ========================================================================
        // Board Manipulation Methods (Không đổi)
        // ========================================================================
        private void ExpandPlacementBoard(int slotsPerLayer, ref int currentNumberOfLayers)
        {
            int oldNumberOfLayers = currentNumberOfLayers;
            currentNumberOfLayers++;
            int[,] newBoard = new int[slotsPerLayer, currentNumberOfLayers];
            InitializeBoard(newBoard, -1);
            for (int r = 0; r < slotsPerLayer; r++) {
                for (int c = 0; c < oldNumberOfLayers; c++) {
                    newBoard[r, c] = placementBoard[r, c];
                }
            }
            placementBoard = newBoard;
        }

        private void InitializeBoard(int[,] board, int initialValue)
        {
            int rows = board.GetLength(0);
            int cols = board.GetLength(1);
            // Array.Fill(board, initialValue);
            // Hoặc dùng vòng lặp
            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < cols; j++) {
                    board[i, j] = initialValue;
                }
            }
        }

        // ========================================================================
        // Final Board Creation & Mapping (Không đổi)
        // ========================================================================
         private ShelfData[,] CreateEmptyFinalBoard(int slotsPerLayer)
        {
            finalNumberOfLayers = 1;
            ShelfData[,] emptyBoard = new ShelfData[boardSize.x, boardSize.y];
            for (int i = 0; i < boardSize.x; i++) {
                for (int j = 0; j < boardSize.y; j++) {
                    ShelfType type = (originalBoardData != null && i < originalBoardData.GetLength(0) && j < originalBoardData.GetLength(1))
                                     ? originalBoardData[i, j].shelfType : ShelfType.Normal;
                    emptyBoard[i, j] = new ShelfData {
                        shelfType = type, position = new Vector2Int(i, j), slotDatas = new List<SlotData>()
                    };
                    if (type != ShelfType.Empty) {
                        for (int h = 0; h < 3; h++) {
                             var items = new List<int>(finalNumberOfLayers);
                             for(int k=0; k<finalNumberOfLayers; k++) items.Add(-1);
                             emptyBoard[i,j].slotDatas.Add(new SlotData { itemsLists = items });
                        }
                    }
                }
            }
            return emptyBoard;
        }

        private ShelfData[,] CreateAndMapFinalBoard(int slotsPerLayer, int numberOfLayers)
        {
            ShelfData[,] finalBoard = new ShelfData[boardSize.x, boardSize.y];
            int placementBoardRowIndex = 0;
            for (int i = 0; i < boardSize.x; i++) {
                for (int j = 0; j < boardSize.y; j++) {
                    ShelfType type = (originalBoardData != null && i < originalBoardData.GetLength(0) && j < originalBoardData.GetLength(1))
                                     ? originalBoardData[i, j].shelfType : ShelfType.Normal;
                    finalBoard[i, j] = new ShelfData {
                        shelfType = type, position = new Vector2Int(i, j), slotDatas = new List<SlotData>(3)
                    };
                    if (type != ShelfType.Empty) {
                        if (placementBoardRowIndex + 2 >= placementBoard.GetLength(0)) {
                             Debug.LogError($"GenLevelAlgorithm Error: placementBoardRowIndex out of bounds during mapping.");
                             return finalBoard; // Return partially mapped board to avoid exception
                        }
                        for (int h = 0; h < 3; h++) {
                            var slotData = new SlotData { itemsLists = new List<int>(numberOfLayers) };
                            for (int k = 0; k < numberOfLayers; k++) {
                                // Check bounds for placementBoard just in case numberOfLayers changed drastically
                                if (k < placementBoard.GetLength(1)) {
                                     slotData.itemsLists.Add(placementBoard[placementBoardRowIndex + h, k]);
                                } else {
                                     slotData.itemsLists.Add(-1); // Add empty if layer doesn't exist in placementBoard (shouldn't happen often)
                                     Debug.LogWarning($"GenLevelAlgorithm Warning: Accessing layer {k} outside placementBoard bounds ({placementBoard.GetLength(1)}) during mapping.");
                                }
                            }
                            finalBoard[i, j].slotDatas.Add(slotData);
                        }
                        placementBoardRowIndex += 3;
                    }
                }
            }
             if (placementBoardRowIndex != slotsPerLayer && slotsPerLayer > 0) { // Add check for slotsPerLayer > 0
                 Debug.LogWarning($"GenLevelAlgorithm Warning: Mapping finished, placementBoardRowIndex ({placementBoardRowIndex}) != slotsPerLayer ({slotsPerLayer}).");
             }
            return finalBoard;
        }

        // ========================================================================
        // Calculation & Helper Methods
        // ========================================================================
        private bool ValidateInput() {
             if (originalBoardData == null) { Debug.LogError("GenLevelAlgorithm Input Error: originalBoardData is null."); return false; }
             if (boardSize.x <= 0 || boardSize.y <= 0) { Debug.LogError($"GenLevelAlgorithm Input Error: boardSize ({boardSize}) is invalid."); return false; }
             if (originalBoardData.GetLength(0) < boardSize.x || originalBoardData.GetLength(1) < boardSize.y) { Debug.LogError($"GenLevelAlgorithm Input Error: originalBoardData dimensions [{originalBoardData.GetLength(0)},{originalBoardData.GetLength(1)}] are smaller than boardSize {boardSize}."); return false; }
             // depth is now handled in constructor to be >= 0
             return true;
        }
        private int GetSlotsPerLayer() {
             int count = 0;
             if (originalBoardData == null) return 0;
             for (int i = 0; i < boardSize.x; i++) {
                 for (int j = 0; j < boardSize.y; j++) {
                     if (i < originalBoardData.GetLength(0) && j < originalBoardData.GetLength(1)) {
                         if (originalBoardData[i, j].shelfType != ShelfType.Empty) {
                             count++;
                         }
                     } else { Debug.LogWarning($"Accessing originalBoardData out of bounds at [{i},{j}] during GetSlotsPerLayer."); }
                 }
             }
             return count * 3;
        }
        private int CalculateInitialLayers(int slotsPerLayer, int totalItems, float coverPercent) {
             if (slotsPerLayer <= 0) return 1;
             int maxItemsPerLayer = slotsPerLayer;
             float avgItemsPerLayer = maxItemsPerLayer * Math.Clamp(coverPercent, 0f, 1f);
             int layers = (avgItemsPerLayer > 0.01f)
                 ? (int)Math.Ceiling((float)totalItems / avgItemsPerLayer)
                 : totalItems;
             return Math.Max(1, layers);
        }

        /// <summary>
        /// SỬA ĐỔI: Tính khoảng layer hợp lệ [lower, upper) để đặt item tiếp theo,
        /// đảm bảo phạm vi (max - min) không vượt quá depth.
        /// </summary>
        private (int, int) CalculatePlacementBoundsStrict(int currentNumberOfLayers, int minLayerPlaced, int maxLayerPlaced, int depthValue)
        {
            // Nếu chưa có item nào được đặt hoặc depth < 0 (vô hiệu hóa), cho phép toàn bộ phạm vi
            if (minLayerPlaced == -1) // depthValue đã được đảm bảo >= 0 trong constructor
            {
                return (0, currentNumberOfLayers);
            }

            int currentRange = maxLayerPlaced - minLayerPlaced;

            int lowerBound;
            int upperBound;

            // Nếu phạm vi hiện tại ĐÃ ĐẠT hoặc VƯỢT QUÁ giới hạn depth
            if (currentRange >= depthValue)
            {
                // Chỉ cho phép đặt trong các lớp hiện có để không tăng phạm vi
                lowerBound = minLayerPlaced;
                upperBound = maxLayerPlaced + 1; // +1 vì giới hạn trên là exclusive
            }
            else // Nếu phạm vi hiện tại vẫn nhỏ hơn depth, cho phép mở rộng
            {
                // Cho phép đặt trong khoảng cách 'depth' từ min và max hiện tại
                // Nhưng cần đảm bảo phạm vi tổng không vượt quá depth
                // Ví dụ: depth=2, min=3, max=3. currentRange=0.
                // Có thể đặt từ Max(0, 3-2)=1 đến Min(N, 3+2+1)=Min(N, 6). => [1, 6)
                // Ví dụ: depth=2, min=3, max=4. currentRange=1.
                // Có thể đặt từ Max(0, 3-2)=1 đến Min(N, 4+2+1)=Min(N, 7). => [1, 7)
                // Logic này vẫn giống logic cũ và có thể gây lỗi.

                // --> Logic đúng: Tính toán giới hạn sao cho lớp mới (k) thỏa mãn:
                // Max(k, maxLayerPlaced) - Min(k, minLayerPlaced) <= depthValue

                // Giới hạn dưới: Lớp thấp nhất có thể là minLayerPlaced - (depthValue - currentRange)
                // Ví dụ: depth=2, min=3, max=4 (range=1). Lớp thấp nhất có thể là 3 - (2-1) = 2
                // Ví dụ: depth=2, min=3, max=3 (range=0). Lớp thấp nhất có thể là 3 - (2-0) = 1
                lowerBound = Math.Max(0, minLayerPlaced - (depthValue - currentRange));

                // Giới hạn trên: Lớp cao nhất có thể là maxLayerPlaced + (depthValue - currentRange)
                // Ví dụ: depth=2, min=3, max=4 (range=1). Lớp cao nhất có thể là 4 + (2-1) = 5. UpperBound = 6.
                // Ví dụ: depth=2, min=3, max=3 (range=0). Lớp cao nhất có thể là 3 + (2-0) = 5. UpperBound = 6.
                upperBound = Math.Min(currentNumberOfLayers, maxLayerPlaced + (depthValue - currentRange) + 1);

            }

            // Sanity check cuối cùng
             if (upperBound <= lowerBound)
             {
                 // Nếu tính toán vẫn lỗi (có thể xảy ra với depth=0 khi min!=max),
                 // fallback về đặt trong khoảng hiện tại nếu có thể, hoặc mở rộng nếu không
                 if (maxLayerPlaced >= minLayerPlaced) {
                     lowerBound = minLayerPlaced;
                     upperBound = maxLayerPlaced + 1;
                     Debug.LogWarning($"GenLevelAlgorithm Warning: Strict placement bounds [{lowerBound},{upperBound}) became invalid after calculation. Falling back to current range [{minLayerPlaced},{maxLayerPlaced+1}).");
                 } else {
                     // Trường hợp rất lạ, fallback về toàn cục và hy vọng mở rộng sẽ giải quyết
                     lowerBound = 0;
                     upperBound = currentNumberOfLayers;
                      Debug.LogError($"GenLevelAlgorithm Error: Strict placement bounds invalid and minLayer > maxLayer. Fallback to full range.");
                 }
                 // Đảm bảo upperBound không nhỏ hơn lowerBound sau fallback
                 if (upperBound <= lowerBound && currentNumberOfLayers > lowerBound) {
                     upperBound = lowerBound + 1; // Ít nhất cho phép 1 lớp nếu có thể
                 } else if (upperBound <= lowerBound) {
                     // Không còn gì để làm, sẽ kích hoạt mở rộng bảng
                 }

             }

            return (lowerBound, upperBound);
        }


        private void UpdateMinMaxLayers(int currentLayer, ref int minLayer, ref int maxLayer) {
             if (minLayer == -1) { minLayer = currentLayer; maxLayer = currentLayer; }
             else { minLayer = Math.Min(minLayer, currentLayer); maxLayer = Math.Max(maxLayer, currentLayer); }
        }
    }
}
