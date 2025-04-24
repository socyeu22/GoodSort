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
        public Vector2Int boardSize;
        public ShelfData[,] boardData;
        public Dictionary<int, int> amountPairEachItem;
        public float percentCoverLayer;
        public int depth;

        // --- Output Fields ---
        public int[,] generatedBoard { get; private set; }
        public int finalNumberOfLayers { get; private set; }

        // --- Private Fields ---
        private System.Random rand = new System.Random();
        private const int MAX_PLACEMENT_ATTEMPTS_PER_ITEM = 500;
        private const int MAX_PLACEMENT_ATTEMPTS_GLOBAL = 1000;
        private const int MAX_EXPANSION_CYCLES = 15;

        public GenLevelAlgorithm(Vector2Int boardSize, ShelfData[,] boardData, Dictionary<int, int> amountPairEachItem, float percentCoverLayer, int depth)
        {
            this.boardSize = boardSize;
            this.boardData = boardData;
            this.amountPairEachItem = amountPairEachItem;
            this.percentCoverLayer = percentCoverLayer;
            this.depth = depth;
        }

        // ========================================================================
        // Main Public Method
        // ========================================================================

        /// <summary>
        /// Hàm chính để tạo dữ liệu level. Điều phối các bước chính.
        /// </summary>
        /// <returns>True nếu tạo thành công, False nếu có lỗi.</returns>
        public bool GenLevel()
        {
            // --- 1. Validation & Initial Setup ---
            if (!ValidateInput()) return false;

            int slotsPerLayer = GetSlotsPerLayer();
            if (slotsPerLayer <= 0)
            {
                Debug.LogError("Không có vị trí kệ hợp lệ nào (slotsPerLayer <= 0).");
                return false;
            }

            // Xử lý trường hợp không yêu cầu item nào
            if (amountPairEachItem == null || amountPairEachItem.Count == 0)
            {
                return CreateEmptyBoard(slotsPerLayer);
            }

            int amountItems = amountPairEachItem.Sum(pair => pair.Value * 3);
            int numberOfLayers = CalculateInitialLayers(slotsPerLayer, amountItems, percentCoverLayer);

            Debug.Log($"Initial Calculation: Slots Per Layer={slotsPerLayer}, Initial Layers={numberOfLayers}. Total Items={amountItems}");

            generatedBoard = new int[slotsPerLayer, numberOfLayers];
            InitializeBoard(generatedBoard, -1); // Khởi tạo với -1

            var cloneAmountPair = amountPairEachItem.ToDictionary(entry => entry.Key, entry => entry.Value);

            // --- 2. Placement Loop ---
            foreach (var itemIDPair in cloneAmountPair)
            {
                int itemID = itemIDPair.Key;
                int itemsToPlace = itemIDPair.Value * 3;

                // Gọi hàm xử lý đặt tất cả item của loại này
                bool success = PlaceAllItemsOfType(itemID, itemsToPlace, slotsPerLayer, ref numberOfLayers);
                if (!success)
                {
                    // Lỗi đã được log bên trong, dừng quá trình tạo
                    return false;
                }
            }

            // --- 3. Finalization ---
            finalNumberOfLayers = numberOfLayers; // Lưu số lớp cuối cùng
            Debug.Log($"Level generation successful! Final number of layers: {finalNumberOfLayers}");
            return true;
        }

        // ========================================================================
        // Core Placement Logic Methods
        // ========================================================================

        /// <summary>
        /// Quản lý việc đặt tất cả các vật phẩm cho một itemID cụ thể.
        /// </summary>
        private bool PlaceAllItemsOfType(int itemID, int itemsToPlace, int slotsPerLayer, ref int currentNumberOfLayers)
        {
            int minLayerPlaced = -1; // Theo dõi min/max layer cho riêng loại item này
            int maxLayerPlaced = -1;

            for (int i = 0; i < itemsToPlace; i++)
            {
                // Gọi hàm cố gắng đặt một item, có xử lý mở rộng
                bool placed = AttemptPlaceSingleItemWithExpansion(itemID, slotsPerLayer, ref currentNumberOfLayers, ref minLayerPlaced, ref maxLayerPlaced);
                if (!placed)
                {
                    Debug.LogError($"Failed to place item {itemID} (instance {i + 1}/{itemsToPlace}) even after expansions. Aborting.");
                    return false; // Không thể đặt được item này -> dừng
                }
            }
            return true; // Đặt thành công tất cả item của loại này
        }

        /// <summary>
        /// Cố gắng đặt một vật phẩm đơn lẻ, thực hiện mở rộng bảng nếu cần thiết.
        /// Quản lý vòng lặp thử/mở rộng.
        /// </summary>
        private bool AttemptPlaceSingleItemWithExpansion(int itemID, int slotsPerLayer, ref int currentNumberOfLayers, ref int minLayerPlaced, ref int maxLayerPlaced)
        {
            int expansionCycle = 0;

            while (expansionCycle < MAX_EXPANSION_CYCLES)
            {
                expansionCycle++;

                // Tính toán khoảng layer hợp lệ cho lần thử này
                var (lowerBound, upperBound) = CalculatePlacementBounds(currentNumberOfLayers, minLayerPlaced, maxLayerPlaced, depth);

                // Thử đặt trong khoảng giới hạn
                var (placedInBounds, layerPlacedInBounds) = TryPlaceInBounds(itemID, slotsPerLayer, lowerBound, upperBound);
                if (placedInBounds)
                {
                    UpdateMinMaxLayers(layerPlacedInBounds, ref minLayerPlaced, ref maxLayerPlaced);
                    return true; // Thành công
                }

                // Thử đặt toàn cục
                var (placedGlobally, layerPlacedGlobally) = TryPlaceGlobally(itemID, slotsPerLayer, currentNumberOfLayers);
                if (placedGlobally)
                {
                    UpdateMinMaxLayers(layerPlacedGlobally, ref minLayerPlaced, ref maxLayerPlaced);
                     Debug.LogWarning($"Placed item {itemID} globally [Layer:{layerPlacedGlobally}] after failing in bounds.");
                    return true; // Thành công
                }

                // Nếu không đặt được -> Bảng đầy -> Mở rộng
                 Debug.LogWarning($"Board full trying to place item {itemID}. Expanding layers from {currentNumberOfLayers} to {currentNumberOfLayers + 1}.");
                 ExpandBoard(slotsPerLayer, ref currentNumberOfLayers);
                 Debug.Log($"Board expanded. Retrying placement for item {itemID}.");

                 // Vòng lặp while sẽ chạy lại với bảng đã mở rộng
            }

            // Nếu chạy hết số lần mở rộng mà vẫn không đặt được
            Debug.LogError($"Failed to place item {itemID} after {MAX_EXPANSION_CYCLES} expansion cycles. Aborting.");
            return false;
        }

        /// <summary>
        /// Thử tìm và đặt item trong khoảng layer giới hạn.
        /// </summary>
        /// <returns>Tuple (bool success, int layerIndex) </returns>
        private (bool, int) TryPlaceInBounds(int itemID, int slotsPerLayer, int lowerBoundLayer, int upperBoundLayer)
        {
            if (upperBoundLayer <= lowerBoundLayer) return (false, -1); // Khoảng không hợp lệ

            for (int attempt = 0; attempt < MAX_PLACEMENT_ATTEMPTS_PER_ITEM; attempt++)
            {
                int slotIdx = rand.Next(0, slotsPerLayer);
                int layerIdx = rand.Next(lowerBoundLayer, upperBoundLayer); // upperBoundLayer là exclusive
                if (generatedBoard[slotIdx, layerIdx] == -1)
                {
                    generatedBoard[slotIdx, layerIdx] = itemID;
                    return (true, layerIdx);
                }
            }
            return (false, -1); // Không tìm thấy sau các lần thử
        }

        /// <summary>
        /// Thử tìm và đặt item ở bất kỳ vị trí nào trên bảng hiện tại.
        /// </summary>
        /// <returns>Tuple (bool success, int layerIndex) </returns>
        private (bool, int) TryPlaceGlobally(int itemID, int slotsPerLayer, int currentNumberOfLayers)
        {
             for (int attempt = 0; attempt < MAX_PLACEMENT_ATTEMPTS_GLOBAL; attempt++)
            {
                int slotIdx = rand.Next(0, slotsPerLayer);
                int layerIdx = rand.Next(0, currentNumberOfLayers);
                if (generatedBoard[slotIdx, layerIdx] == -1)
                {
                    generatedBoard[slotIdx, layerIdx] = itemID;
                    return (true, layerIdx);
                }
            }
            return (false, -1); // Không tìm thấy sau các lần thử
        }

        // ========================================================================
        // Board Manipulation Methods
        // ========================================================================

        /// <summary>
        /// Mở rộng mảng generatedBoard thêm một lớp (chiều thứ 2).
        /// </summary>
        private void ExpandBoard(int slotsPerLayer, ref int currentNumberOfLayers)
        {
            int oldNumberOfLayers = currentNumberOfLayers;
            currentNumberOfLayers++; // Tăng số lớp

            int[,] newBoard = new int[slotsPerLayer, currentNumberOfLayers];
            InitializeBoard(newBoard, -1); // Khởi tạo bảng mới với -1

            // Sao chép dữ liệu cũ
            for (int r = 0; r < slotsPerLayer; r++)
            {
                // Array.Copy hiệu quả hơn cho việc sao chép hàng loạt
                Array.Copy(generatedBoard, r * oldNumberOfLayers, newBoard, r * currentNumberOfLayers, oldNumberOfLayers);
                // Dòng trên cần xem lại, cách tính index mảng 2D trong Array.Copy phức tạp. Dùng vòng lặp an toàn hơn.
                // for (int c = 0; c < oldNumberOfLayers; c++) {
                //     newBoard[r, c] = generatedBoard[r, c];
                // }
            }
            // Sửa lại bằng vòng lặp cho chính xác:
             for (int r = 0; r < slotsPerLayer; r++) {
                 for (int c = 0; c < oldNumberOfLayers; c++) { // Chỉ copy các lớp cũ
                     newBoard[r, c] = generatedBoard[r, c];
                 }
             }


            generatedBoard = newBoard; // Cập nhật tham chiếu
        }

        /// <summary>
        /// Khởi tạo bảng với giá trị ban đầu.
        /// </summary>
        private void InitializeBoard(int[,] board, int initialValue)
        {
            int rows = board.GetLength(0);
            int cols = board.GetLength(1);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    board[i, j] = initialValue;
                }
            }
        }

        // ========================================================================
        // Calculation & Helper Methods
        // ========================================================================

         /// <summary>
        /// Tạo bảng trống khi không có item nào được yêu cầu.
        /// </summary>
        private bool CreateEmptyBoard(int slotsPerLayer) {
            generatedBoard = new int[slotsPerLayer, 1];
            InitializeBoard(generatedBoard, -1);
            finalNumberOfLayers = 1;
            return true;
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của các tham số đầu vào.
        /// </summary>
        private bool ValidateInput()
        {
            if (boardData == null || boardSize.x <= 0 || boardSize.y <= 0 || depth < 0)
            {
                Debug.LogError("Input validation failed: boardData null, boardSize invalid, or depth negative.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Đếm số lượng slot hợp lệ (không Empty) trên mỗi lớp từ boardData.
        /// </summary>
        private int GetSlotsPerLayer()
        {
             int count = 0;
             if (boardData == null) return 0; // Thêm kiểm tra null
             for (int i = 0; i < boardSize.x; i++) {
                 for (int j = 0; j < boardSize.y; j++) {
                     if (i < boardData.GetLength(0) && j < boardData.GetLength(1)) {
                         if (boardData[i, j].shelfType != ShelfType.Empty) {
                             count++;
                         }
                     } else {
                          Debug.LogWarning($"Accessing boardData out of bounds at [{i},{j}] during GetSlotsPerLayer.");
                     }
                 }
             }
             return count;
        }

        /// <summary>
        /// Tính toán số lớp ban đầu cần thiết.
        /// </summary>
        private int CalculateInitialLayers(int slotsPerLayer, int totalItems, float coverPercent)
        {
            if (slotsPerLayer <= 0) return 1;
            int maxItemsPerLayer = slotsPerLayer * 3;
            if (maxItemsPerLayer <= 0) return Math.Max(1, totalItems);

            float avgItemsPerLayer = maxItemsPerLayer * Math.Clamp(coverPercent, 0f, 1f);
            int layers = (avgItemsPerLayer > 0.01f)
                ? (int)Math.Ceiling((float)totalItems / avgItemsPerLayer)
                : totalItems;
            return Math.Max(1, layers);
        }

        /// <summary>
        /// Tính toán khoảng layer hợp lệ [lower, upper) để đặt item tiếp theo.
        /// </summary>
        /// <returns>Tuple (int lowerBound, int upperBound - exclusive)</returns>
        private (int, int) CalculatePlacementBounds(int currentNumberOfLayers, int minLayerPlaced, int maxLayerPlaced, int depthValue)
        {
            int lowerBound = 0;
            int upperBound = currentNumberOfLayers; // Exclusive

            if (minLayerPlaced != -1 && depthValue >= 0)
            {
                lowerBound = Math.Max(0, minLayerPlaced - (depthValue * 3));
                upperBound = Math.Min(currentNumberOfLayers, maxLayerPlaced + (depthValue * 3) + 1);
            }

            // Đảm bảo lower <= upper
            if (upperBound <= lowerBound)
            {
                // Nếu khoảng không hợp lệ (ví dụ depth=0 và min/max đã khác nhau),
                // hoặc khoảng cách quá lớn, mở rộng ra toàn bộ để tránh lỗi
                lowerBound = 0;
                upperBound = currentNumberOfLayers;
            }
            return (lowerBound, upperBound);
        }

        /// <summary>
        /// Cập nhật layer nhỏ nhất và lớn nhất đã đặt item.
        /// </summary>
        private void UpdateMinMaxLayers(int currentLayer, ref int minLayer, ref int maxLayer)
        {
            if (minLayer == -1)
            {
                minLayer = currentLayer;
                maxLayer = currentLayer;
            }
            else
            {
                minLayer = Math.Min(minLayer, currentLayer);
                maxLayer = Math.Max(maxLayer, currentLayer);
            }
        }
    }
}