using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using GameCore;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/LevelData")]
public class LevelData : ScriptableObject
{
    public Vector2Int size;
    [OdinSerialize, SerializeReference, TableMatrix (DrawElementMethod = "DrawBoard")] public ShelfData [,] boardData;

    [OdinSerialize, SerializeReference] public Dictionary<int, int> amountPairEachItem;
    public float percentCoverLayer;
    public int depth;

    [Button] 
    public void CreateBoard()
    {
        boardData = new ShelfData[size.x, size.y];

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                boardData[i, j] = new ShelfData()
                {
                    listsItemID = new List<List<int>>(),
                    shelfType = ShelfType.Normal,
                    position = new Vector2Int(i, j)
                };
            }
        }
        
        var genLevelAlgorithm = new GenLevelAlgorithm(size, boardData, amountPairEachItem, percentCoverLayer, depth);
        var result = genLevelAlgorithm.GenLevel();
        Debug.Log($"Result Algorithm : {result}");
    }

    private static ShelfData DrawBoard(Rect rect, ShelfData value)
    {
        // Initialize value if null
        if (value == null)
        {
            value = new ShelfData();
        }

        // Initialize list if null

        // Draw a simplified representation of the list in the cell
        // Since a list can be long, we'll show the count and allow editing one ID at a time
        Rect countRect = new Rect(rect.x, rect.y, rect.width, rect.height);


        string data = "";
        for (int i = 0; i < value.listsItemID.Count; i++)
        {
            data += "{";
            for (int j = 0; j < value.listsItemID[i].Count; j++)
            {
                data += (" " + value.listsItemID[i][j]);
            }

            data += "}";
        }
        EditorGUI.LabelField(countRect, data);

        return value;
    }
}
