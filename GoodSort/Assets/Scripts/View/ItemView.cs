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
        [SerializeField] private int m_layerIndex;

        public void SetupView(ItemData data)
        {
            if (m_icon != null)
            {
                m_icon.sprite = GameConfig.Instance.itemIconConfig.GetItemIconByID(data.id);
                m_icon.enabled = data.visualType == ItemVisualType.FullDisplay;
            }
        }

        public void SetLayer(int layerIndex)
        {
            m_layerIndex = layerIndex;
            if (m_icon != null)
            {
                m_icon.color = layerIndex == 1 ? Color.white : Color.gray;
                m_icon.sortingOrder = 10 - layerIndex;
            }
            transform.localPosition = new Vector3(transform.localPosition.x, (layerIndex - 1) * m_heightOffset, 0f);
            SetActiveItem(layerIndex == 1);
        }

        public void OnDragStart()
        {
            if (m_icon != null)
            {
                m_icon.sortingOrder = 100;
            }
        }

        public void OnDragEnd(int layerIndex)
        {
            if (m_icon != null)
            {
                m_icon.sortingOrder = 10 - layerIndex;
            }
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
