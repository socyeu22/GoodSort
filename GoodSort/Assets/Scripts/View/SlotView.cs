using UnityEngine;
using DefaultNamespace;
using System.Collections.Generic;

namespace GameCore
{
    public class SlotView : MonoBehaviour
    {
        [SerializeField] private SlotData m_slotData = new SlotData();
        [SerializeField] private GameObject m_topItemSlot;
        private ItemController m_topItemController;
        [SerializeField] private int m_topItemSlotId = -1;
        /// <summary>
        ///     Item id currently shown by the TopItemSlot. -1 if empty.
        /// </summary>
        public int TopItemSlotId => m_topItemSlotId;
        public SlotData SlotData
        {
            get => m_slotData;
            set => m_slotData = value;
        }

        /// <summary>
        ///     Id of the item currently on top of this slot. -1 if empty.
        /// </summary>
        public int TopItemId
        {
            get
            {
                if (m_slotData.itemsLists == null || m_slotData.itemsLists.Count == 0)
                {
                    return -1;
                }
                return m_slotData.itemsLists[0];
            }
        }

        /// <summary>
        ///     Returns true if this slot can accept a new top item.
        /// </summary>
        public bool IsTopSlotAvailable => TopItemId == -1;

        public virtual void AddItem(ItemController item)
        {
            item.transform.SetParent(transform);
            // Reset local position so the item snaps exactly to the slot
            item.transform.localPosition = Vector3.zero;
            item.LayerIndex = transform.childCount;

            if (m_slotData.itemsLists.Count == 0)
            {
                m_slotData.itemsLists.Add(item.Id);
            }
            else
            {
                m_slotData.itemsLists[0] = item.Id;
            }

            UpdateTopItemSlot();
        }

        public bool RemoveItem(ItemController item)
        {
            if (item.transform.parent == transform)
            {
                item.transform.SetParent(null);
                if (m_slotData.itemsLists.Count > 0)
                {
                    if (m_slotData.itemsLists.Count == 1)
                    {
                        m_slotData.itemsLists[0] = -1;
                    }
                    else
                    {
                        m_slotData.itemsLists[0] = m_slotData.itemsLists[1];
                        m_slotData.itemsLists.RemoveAt(1);
                    }
                }
                RearrangeItems();
                UpdateTopItemSlot();
                return true;
            }
            return false;
        }

        private void RearrangeItems()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var item = transform.GetChild(i).GetComponent<ItemController>();
                if (item != null)
                {
                    item.LayerIndex = i + 1;
                }
            }
        }

        private void Awake()
        {
            CreateDefaultTopItemSlot();
            UpdateTopItemSlot();
        }

        private void Start()
        {
            // If a real item was spawned for the top layer, use it as the top item slot
            foreach (Transform child in transform)
            {
                if (child.gameObject == m_topItemSlot) continue;
                var controller = child.GetComponent<ItemController>();
                if (controller != null && controller.Id == TopItemId && TopItemId != -1)
                {
                    Destroy(m_topItemSlot);
                    m_topItemSlot = child.gameObject;
                    m_topItemController = controller;
                    m_topItemSlot.name = "TopItemSlot";
                    break;
                }
            }

            UpdateTopItemSlot();
        }

        private void CreateDefaultTopItemSlot()
        {
            if (m_topItemSlot == null)
            {
                foreach (Transform child in transform)
                {
                    if (child.name == "TopItemSlot")
                    {
                        m_topItemSlot = child.gameObject;
                        break;
                    }
                }
            }

            if (m_topItemSlot == null)
            {
                var prefab = GameConfig.Instance.prefabConfig.itemPrefab;
                if (prefab == null)
                {
                    return;
                }

                var item = Instantiate(prefab, transform);
                item.transform.localPosition = Vector3.zero;
                item.gameObject.name = "TopItemSlot";
                m_topItemSlot = item.gameObject;
            }

            m_topItemController = m_topItemSlot.GetComponent<ItemController>();
            if (m_topItemController != null)
            {
                var dummy = new ItemData { id = -1, visualType = ItemVisualType.FullDisplay };
                m_topItemController.InitItem(dummy, 1, null, this, null);
            }
        }

        public void SetTopItemSlotId(int itemId)
        {
            // Ensure the placeholder top item slot exists when updating id
            if (m_topItemSlot == null)
            {
                CreateDefaultTopItemSlot();
            }
            if (m_slotData.itemsLists == null)
            {
                m_slotData.itemsLists = new List<int> { itemId };
            }
            else if (m_slotData.itemsLists.Count == 0)
            {
                m_slotData.itemsLists.Add(itemId);
            }
            else
            {
                m_slotData.itemsLists[0] = itemId;
            }

            UpdateTopItemSlot();
        }

        public void UpdateTopItemSlot()
        {
            if (m_topItemSlot == null)
            {
                CreateDefaultTopItemSlot();
            }

            m_topItemSlotId = TopItemId;

            if (m_topItemController != null && m_topItemSlotId != -1)
            {
                var data = GameConfig.Instance.itemDataConfig.GetItemDataByID(m_topItemSlotId);
                if (data != null)
                {
                    m_topItemController.InitItem(data, 1, null, this, null);
                }
            }

            // Always keep the top item slot active so it can receive snapped items
            m_topItemSlot.SetActive(true);
        }
    }
}
