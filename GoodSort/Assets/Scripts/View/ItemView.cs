using System;
using DefaultNamespace;
using UnityEngine;

namespace GameCore
{
    public class ItemView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer m_icon;
        [SerializeField] private Collider2D m_collider;
        [SerializeField] private float m_heightOffset;
        public bool isTopItem;

        public void SetupView(ItemData data)
        {
            if (m_icon != null)
            {
                m_icon.sprite = data.Icon;
                m_icon.enabled = data.IsFullDisplay;
            }
        }

        public void SetLayer(int layerIndex, bool isTop)
        {
            isTopItem = isTop;
            if (m_icon != null)
            {
                m_icon.color = isTopItem ? Color.white : Color.gray;
                m_icon.sortingOrder = 10 - layerIndex;
            }
            transform.localPosition = new Vector3(transform.localPosition.x, (layerIndex - 1) * m_heightOffset, 0f);
            SetActiveItem(isTopItem);
        }


        private void SetActiveItem(bool isActive)
        {
            if (m_collider != null)
            {
                m_collider.enabled = isActive;
            }
        }
    }
}
