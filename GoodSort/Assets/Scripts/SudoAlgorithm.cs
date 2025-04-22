using System.Collections.Generic;
using System.Linq;
using GameCore;
using UnityEngine;

namespace DefaultNamespace
{
    public class SudoAlgorithm
    {
        public Vector2Int boardSize;
        
        public ShelfData[,] boardData;
        public Dictionary<int, int> amountPairEachItem;
        public float percentCoverLayer;
        public int depth;
        System.Random rand = new System.Random();

        public void GenLevel()
        {
            // Array 2D Array size x * y. x: Number Of Shelf Normals. y: 3 * Number of Layer(included Empty Item)
            int x = 0;
            for (int i = 0; i < boardSize.x; i++)
            {
                for (int j = 0; j < boardSize.y; j++)
                {
                    if(boardData[i,j].shelfType == ShelfType.Empty) continue;
                    x++;
                }
            }

            int y = 0;
            int maxItemPerLayer = x * 3;

            int amountItems = 0;
            foreach (var pairPerItem in amountPairEachItem)
            {
                amountItems += pairPerItem.Value * 3;
            }
            
            float averageItemPerLayer = maxItemPerLayer * percentCoverLayer;
            
            y = (int)(amountItems / averageItemPerLayer) + 1;
            
            // Create Board Done
            int[,] boardRandom = new int[x, y];
            
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    boardRandom[i, j] = -1;
                }
            }
            
            var cloneAmountPair = amountPairEachItem.ToDictionary(entry => entry.Key, entry => entry.Value);

            foreach (var itemIDPair in cloneAmountPair)
            {
                var pairCount = itemIDPair.Value;
                while (pairCount > 0)
                {
                    // Random 3 itemId on board
                    var itemID = itemIDPair.Key;
                    var amountItemGen = 0;
                    var listPositionGen = new List<int>();
                    
                    for (int i = 0; i < 3; i++)
                    {
                        int lowerBoundY = 0;
                        int upperBoundY = y;

                        foreach (var positionGen in listPositionGen)
                        {
                            // Calculate LowerBound - Upper Bound
                        }
                        
                        int randomRow = rand.Next(0, x);
                        int randomCol = rand.Next(lowerBoundY, upperBoundY);
                        
                        while (boardRandom[randomRow, randomCol] == -1)
                        {
                            if (boardRandom[randomRow, randomCol] == -1)
                            {
                                boardRandom[randomRow, randomCol] = itemID;
                                amountItemGen++;
                                listPositionGen.Add(randomCol);
                                break;
                            }

                            randomRow = rand.Next(0, x);
                            randomCol = rand.Next(0, y);
                        }
                    }
                    
                    // Decrease amount pair 1
                    pairCount--;
                }
            }
            
        }
        
        private Vector2Int GetBoundByPosition(List<int> listPosition)
        {
            
        }
    }
}