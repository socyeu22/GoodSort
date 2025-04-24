using System;
using System.Collections.Generic;
using GameCore;
using UnityEngine;


[Serializable]
public class ShelfData 
{
    public ShelfType shelfType;
    public Vector2Int position;
    public List<List<int>> listsItemID;
}
