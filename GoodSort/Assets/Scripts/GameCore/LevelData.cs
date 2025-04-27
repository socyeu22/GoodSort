using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace; // Đảm bảo namespace này đúng
using GameCore;         // Đảm bảo namespace này đúng
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor; // Cần cho EditorGUI, EditorStyles, EditorGUIUtility
using UnityEngine;
using System.Text; // Cần cho StringBuilder

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/LevelData")]
public class LevelData : ScriptableObject
{
    [Tooltip("Kích thước logic của bảng (số hàng, số cột của ShelfData)")]
    public Vector2Int size;

    [Tooltip("Dữ liệu chi tiết của bảng, được tạo hoặc cập nhật bởi nút CreateBoard")]
    [OdinSerialize, SerializeReference, TableMatrix(ResizableColumns = false, SquareCells = true, DrawElementMethod = "DrawBoard")]
    public ShelfData[,] boardData; // Sẽ được điền bởi thuật toán

    [Tooltip("Số lượng bộ 3 item cần tạo cho mỗi ID")]
    [OdinSerialize, SerializeReference]
    public Dictionary<int, int> amountPairEachItem;

    [Tooltip("Tỷ lệ mong muốn lấp đầy ban đầu cho mỗi lớp (0.0 - 1.0)")]
    [Range(0f, 1f)]
    public float percentCoverLayer = 0.5f; // Thêm giá trị mặc định hợp lý

    [Tooltip("Ràng buộc khoảng cách tối đa (số lớp) giữa các item cùng loại")]
    public int depth = 2; // Thêm giá trị mặc định hợp lý

    [Button(ButtonSizes.Large)]
    [PropertyOrder(-1)] // Đặt nút lên trên cùng
    public void CreateBoard()
    {
        // --- Bước 1: Khởi tạo boardData ban đầu (nếu cần hoặc để reset) ---
        InitializeEmptyBoardData();

        // --- Bước 2: Tạo và chạy thuật toán ---
        var genLevelAlgorithm = new GenLevelAlgorithm(size, this.boardData, amountPairEachItem, percentCoverLayer, depth);
        ShelfData[,] generatedResult = genLevelAlgorithm.GenLevel(); // Nhận kết quả ShelfData[,]

        // --- Bước 3: Xử lý kết quả ---
        if (generatedResult != null)
        {
            this.boardData = generatedResult;
            Debug.Log($"LevelData Info: Board generation successful! Final layers: {genLevelAlgorithm.finalNumberOfLayers}");
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
        else
        {
            Debug.LogError("LevelData Error: Board generation failed. Check previous logs for details.");
        }
    }

    private void InitializeEmptyBoardData()
    {
        if (size.x <= 0 || size.y <= 0)
        {
            Debug.LogError("LevelData Error: Size must be positive to initialize board.");
            boardData = null;
            return;
        }

        boardData = new ShelfData[size.x, size.y];
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                boardData[i, j] = new ShelfData
                {
                    shelfType = ShelfType.Normal,
                    position = new Vector2Int(i, j),
                    slotDatas = new List<SlotData>()
                };
            }
        }
         #if UNITY_EDITOR
         UnityEditor.EditorUtility.SetDirty(this);
         #endif
    }


    // --- Odin Inspector Drawing Logic (Logic vẽ tùy chỉnh trong Editor) ---

    private static ShelfData DrawBoard(Rect rect, ShelfData value)
    {
        // Khởi tạo value nếu null
        // if (value == null)
        // {
        //     // Gán position dựa trên ngữ cảnh TableMatrix thường không cần thiết ở đây
        //     value = new ShelfData { shelfType = ShelfType.Normal, slotDatas = new List<SlotData>() };
        // }
        // Đảm bảo slotDatas không null
        if (value.slotDatas == null)
        {
            value.slotDatas = new List<SlotData>();
        }

        // --- Vẽ giao diện tùy chỉnh ---

        // 1. Vẽ loại kệ (ShelfType)
        float singleLineHeight = EditorGUIUtility.singleLineHeight;
        Rect typeRect = new Rect(rect.x, rect.y, rect.width, singleLineHeight);
        // Chỉ cho phép thay đổi ShelfType nếu không phải đang Play Mode để tránh lỗi không mong muốn
        EditorGUI.BeginChangeCheck();
        var newType = (ShelfType)EditorGUI.EnumPopup(typeRect, value.shelfType);
        if (EditorGUI.EndChangeCheck() && !Application.isPlaying)
        {
             value.shelfType = newType;
             // Có thể thêm logic reset slotDatas nếu chuyển sang Empty
             if (newType == ShelfType.Empty) {
                 value.slotDatas.Clear();
             }
             // Đánh dấu dirty nếu cần, nhưng Odin thường tự xử lý
        }


        // 2. Hiển thị dữ liệu các lớp nếu không phải Empty
        if (value.shelfType != ShelfType.Empty)
        {
            // Kiểm tra xem có dữ liệu slot hợp lệ không (cần 3 slot)
            if (value.slotDatas.Count == 3 && value.slotDatas[0]?.itemsLists != null)
            {
                int numberOfLayers = value.slotDatas[0].itemsLists.Count;
                if (numberOfLayers > 0)
                {
                    // Tính toán vị trí bắt đầu và chiều cao cho mỗi dòng layer
                    float currentY = typeRect.yMax + 2; // Bắt đầu dưới EnumPopup
                    float availableHeight = rect.height - (currentY - rect.y);
                    int maxLinesToShow = Mathf.FloorToInt(availableHeight / singleLineHeight); // Số dòng tối đa có thể hiển thị

                    StringBuilder sb = new StringBuilder(); // Dùng StringBuilder để hiệu quả

                    // Chỉ lặp qua các lớp có thể hiển thị
                    for (int k = 0; k < Math.Min(numberOfLayers, maxLinesToShow); k++)
                    {
                        // Lấy itemID từ mỗi slot cho lớp k, kiểm tra null và index
                        string s1 = GetItemString(value.slotDatas[0], k);
                        string s2 = GetItemString(value.slotDatas[1], k);
                        string s3 = GetItemString(value.slotDatas[2], k);

                        // Tạo chuỗi cho lớp hiện tại
                        sb.AppendLine($"L{k}: [{s1}|{s2}|{s3}]");
                    }

                    // Nếu có nhiều lớp hơn số dòng có thể hiển thị
                    if (numberOfLayers > maxLinesToShow && maxLinesToShow > 0)
                    {
                         sb.Append("..."); // Thêm dấu ... để chỉ rằng còn nữa
                    }

                    // Vẽ toàn bộ chuỗi multi-line vào vùng còn lại
                    Rect infoRect = new Rect(rect.x, currentY, rect.width, availableHeight);
                    // Sử dụng style hỗ trợ multi-line và word wrap nếu cần (mặc định LabelField có thể làm điều này)
                    // Dùng miniLabel để tiết kiệm không gian
                    EditorGUI.LabelField(infoRect, sb.ToString(), EditorStyles.miniLabel);
                }
                else
                {
                    // Trường hợp có 3 slot nhưng không có layer nào
                    Rect infoRect = new Rect(rect.x, typeRect.yMax + 2, rect.width, rect.height - typeRect.height - 2);
                    EditorGUI.LabelField(infoRect, "(No Layers)", EditorStyles.centeredGreyMiniLabel);
                }
            }
            else if (value.slotDatas.Count != 0 && value.slotDatas.Count != 3)
            {
                // Trường hợp số lượng slot không phải 0 hoặc 3 -> lỗi cấu trúc
                Rect infoRect = new Rect(rect.x, typeRect.yMax + 2, rect.width, rect.height - typeRect.height - 2);
                EditorGUI.LabelField(infoRect, $"Slots: {value.slotDatas.Count} (Error!)", EditorStyles.centeredGreyMiniLabel);
            }
             else
            {
                 // Trường hợp chưa có dữ liệu slot nào được tạo (slotDatas.Count == 0)
                 Rect infoRect = new Rect(rect.x, typeRect.yMax + 2, rect.width, rect.height - typeRect.height - 2);
                 EditorGUI.LabelField(infoRect, "(No Slot Data)", EditorStyles.centeredGreyMiniLabel);
            }
        }

        return value; // Trả về giá trị đã có thể được sửa đổi (shelfType)
    }

    /// <summary>
    /// Hàm trợ giúp lấy chuỗi itemID tại layer k từ SlotData, xử lý null/ngoài giới hạn.
    /// </summary>
    private static string GetItemString(SlotData slot, int layerIndex)
    {
        if (slot?.itemsLists != null && layerIndex >= 0 && layerIndex < slot.itemsLists.Count)
        {
            // Trả về itemID, có thể thay -1 bằng "-" cho dễ nhìn
            int itemID = slot.itemsLists[layerIndex];
            return itemID == -1 ? "-" : itemID.ToString();
        }
        return "?"; // Trả về "?" nếu không hợp lệ
    }
}
