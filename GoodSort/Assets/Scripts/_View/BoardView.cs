using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace GameCore
{
    public class BoardView : MonoBehaviour
    {
        private List<ShelfView> m_shelfViews = new List<ShelfView>();
        private BoardController m_boardController;
        [SerializeField] private Transform m_boardTransform;
        [SerializeField] private Vector2 m_offset;
        
        public void CreateBoard(BoardController boardController)
        {
            m_boardController = boardController;
            var shelfPrefab = GameConfig.Instance.prefabConfig.shelfPrefab;
            // Init Shelf Here - Add Shelf To List

            foreach (var shelfData in boardController.ShelfData)
            {
                var shelfView = Instantiate(shelfPrefab, m_boardTransform);
                shelfView.InitShelf(shelfData, OnStateBoardChange);
                shelfView.transform.localPosition = new Vector3(m_offset.x * shelfData.position.x, m_offset.y * shelfData.position.y, 0);
                m_shelfViews.Add(shelfView);
            }
        }

        private void OnStateBoardChange(int idItem, Vector2Int shelfStart, Vector2Int shelfEnd)
        {
            m_boardController.UpdateBoardData(idItem, shelfStart, shelfEnd, out var isHaveMatch);
            if (isHaveMatch)
            {
                var shelfMatch = m_shelfViews.Find(p => p.Position == shelfEnd);
                shelfMatch.RemoveLayerTop();
            }
        }
    }
}