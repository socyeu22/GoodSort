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
                    listItemID = new List<int>(),
                    shelfType = ShelfType.Normal
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
        if (value.listItemID == null)
        {
            value.listItemID = new List<int>();
        }

        // Draw a simplified representation of the list in the cell
        // Since a list can be long, we'll show the count and allow editing one ID at a time
        Rect countRect = new Rect(rect.x, rect.y, rect.width * 0.5f, rect.height);
        Rect idRect = new Rect(rect.x + rect.width * 0.5f, rect.y, rect.width * 0.5f, rect.height);


        string data = "";
        for (int i = 0; i < value.listItemID.Count; i++)
        {
            data += (" " + value.listItemID[i].ToString());
            Debug.Log(data);
        }
        // Display list count
        EditorGUI.LabelField(countRect, data);

        // Allow adding/editing an ID
        int newId = EditorGUI.IntField(idRect, 0);

        // Add the new ID to the list if it's non-zero and user presses Enter or changes focus
        if (newId != 0 && GUI.changed)
        {
            value.listItemID.Add(newId);
        }

        return value;
    }
}
