using System;
using System.Collections.Generic;
using DefaultNamespace;
using GameCore;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;
using System.Text;

[CreateAssetMenu(fileName = "LevelData_Flexible", menuName = "Game/LevelData (Flexible Layout)")]
public class LevelData_Flexible : ScriptableObject
{
    [Tooltip("Danh sách tất cả các kệ trong màn chơi. Người thiết kế tự thêm và cấu hình.")]
    [ListDrawerSettings(NumberOfItemsPerPage = 10, Expanded = true)]
    [OdinSerialize] // Giữ lại OdinSerialize nếu dùng Odin
    [PropertyOrder(0)]
    public List<ShelfData> shelfList = new List<ShelfData>();

    [Tooltip("Tổng số bộ 3 item mong muốn cho mỗi ID trên TOÀN BỘ màn chơi (cả Normal và Dispenser).")]
    // Giữ lại SerializeReference ở đây vì Dictionary là reference type và Odin có thể cần
    [OdinSerialize, SerializeReference]
    public Dictionary<int, int> amountPairEachItem;

    [Tooltip("Tỷ lệ mong muốn lấp đầy ban đầu cho mỗi lớp trên kệ Normal (0.0 - 1.0)")]
    [Range(0f, 1f)]
    public float percentCoverLayer = 0.5f;

    [Tooltip("Ràng buộc khoảng cách lớp tối đa giữa các item cùng loại trên kệ Normal")]
    public int depth = 2;

    // --- Nút tạo/cập nhật dữ liệu ---
    [Button("Generate/Update Level Data", ButtonSizes.Large)]
    [PropertyOrder(-1)]
    public void GenerateLevelData()
    {
        if (shelfList == null)
        {
            shelfList = new List<ShelfData>();
            Debug.LogWarning("LevelData Info: shelfList was null, initialized to empty list.");
        }

        var genLevelAlgorithm = new GenLevelAlgorithm_Flexible(this.shelfList, amountPairEachItem, percentCoverLayer, depth);
        List<ShelfData> generatedResult = genLevelAlgorithm.GenLevel();

        if (generatedResult != null)
        {
            this.shelfList = generatedResult;
            Debug.Log($"LevelData Info: Level data generation successful! Final layers (approx): {genLevelAlgorithm.finalNumberOfLayers}");
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            #endif
        }
        else
        {
            Debug.LogError("LevelData Error: Level data generation failed. Check previous logs for details.");
        }
    }

    // --- Hiển thị dạng dọc (Giữ lại để xem tổng quan) ---
    [ShowInInspector]
    [MultiLineProperty(20)]
    [HideLabel]
    [PropertyOrder(1)]
    [PropertyTooltip("Hiển thị dữ liệu các slot theo chiều dọc (Layer là hàng)")]
    [InfoBox("Vertical Slot View (Read-Only)", InfoMessageType.None)]
    public string VerticalSlotsDisplay => GenerateVerticalSlotsString();

    private string GenerateVerticalSlotsString()
    {
        if (shelfList == null || shelfList.Count == 0)
        {
            return "No shelves defined in shelfList.";
        }

        int maxLayers = 0;
        var displayableShelves = new List<(Vector2Int pos, ShelfData data)>();
        foreach (var shelf in shelfList)
        {
            if (shelf.shelfType != ShelfType.Empty)
            {
                displayableShelves.Add((shelf.position, shelf));
                if (shelf.slotDatas != null)
                {
                    foreach (var slot in shelf.slotDatas)
                    {
                        if (slot?.itemsLists != null)
                        {
                            maxLayers = Math.Max(maxLayers, slot.itemsLists.Count);
                        }
                    }
                }
            }
        }

        if (displayableShelves.Count == 0)
        {
            return "No non-empty shelves found to display.";
        }
         if (maxLayers == 0) { /* Vẫn vẽ header */ }

        displayableShelves.Sort((a, b) => {
            int yComp = a.pos.y.CompareTo(b.pos.y);
            return yComp == 0 ? a.pos.x.CompareTo(b.pos.x) : yComp;
        });

        StringBuilder sb = new StringBuilder();
        int cellWidth = 7;

        sb.Append("Layer |");
        foreach (var shelfInfo in displayableShelves) {
            string header = $"P({shelfInfo.pos.x},{shelfInfo.pos.y})";
            sb.Append(header.PadRight(cellWidth)); sb.Append("|");
        }
        sb.AppendLine();
        sb.Append("------|");
        foreach (var shelfInfo in displayableShelves) {
            sb.Append(new string('-', cellWidth)); sb.Append("|");
        }
        sb.AppendLine();

        for (int k = 0; k < Math.Max(1, maxLayers); k++) {
            sb.Append($" L{k}".PadRight(6) + "|");
            foreach (var shelfInfo in displayableShelves) {
                string cellDisplay = "";
                ShelfData currentShelf = shelfInfo.data;
                // Sử dụng shelfType từ GameCore namespace đã import
                if (currentShelf.shelfType == ShelfType.Normal && currentShelf.slotDatas?.Count == 3) {
                    string s1 = GetItemString(currentShelf.slotDatas[0], k);
                    string s2 = GetItemString(currentShelf.slotDatas[1], k);
                    string s3 = GetItemString(currentShelf.slotDatas[2], k);
                    cellDisplay = $"[{s1}|{s2}|{s3}]";
                } else if (currentShelf.shelfType == ShelfType.Dispenser && currentShelf.slotDatas?.Count == 1) {
                    string s1 = GetItemString(currentShelf.slotDatas[0], k);
                    cellDisplay = $"[{s1}]";
                } else { cellDisplay = "[ ? ]"; } // Xử lý các trường hợp khác
                sb.Append(cellDisplay.PadRight(cellWidth)); sb.Append("|");
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    private static string GetItemString(SlotData slot, int layerIndex)
    {
        if (slot?.itemsLists != null && layerIndex >= 0 && layerIndex < slot.itemsLists.Count) {
            int itemID = slot.itemsLists[layerIndex];
            return itemID == -1 ? "-" : itemID.ToString();
        }
        return " ";
    }
}
