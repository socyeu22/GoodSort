using UnityEngine;

namespace Goodsort.Data
{
    [CreateAssetMenu(fileName = "ItemType_", menuName = "Goodsort/Item Type", order = 2)]
    public class ItemTypeSO : ScriptableObject
    {
        public int itemID; // ID duy nhất cho loại item này
        public string itemName = "DefaultItem";
        public Sprite itemSprite; // Sprite sẽ hiển thị
        public GameObject itemPrefabRef; // Tham chiếu đến Prefab ItemController (nếu cần cấu hình phức tạp hơn Sprite)
        // Thêm các thuộc tính khác nếu cần (ví dụ: âm thanh match, điểm số cơ bản...)
    }
}