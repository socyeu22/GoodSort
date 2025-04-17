using System.Collections.Generic;
using UnityEngine;

namespace Goodsort.Managers
{
    public interface IPoolableObject { void OnGetFromPool(); void OnReturnToPool(); }

    public class PoolManager : MonoBehaviour
    {
        private Dictionary<GameObject, Queue<GameObject>> _poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
        // TODO: Có thể cần quản lý parent transform cho các object đã pool

        public void Initialize() { Debug.Log("[PoolManager] Initialized."); }

        public GameObject GetObjectFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (!_poolDictionary.TryGetValue(prefab, out Queue<GameObject> objectQueue) || objectQueue.Count == 0)
            {
                // Pool chưa có hoặc đã hết -> Tạo mới
                GameObject newObj = Instantiate(prefab, position, rotation);
                // TODO: Gán tham chiếu prefab gốc cho object mới để ReturnObjectToPool biết
                // newObj.AddComponent<PoolIdentifier>().PrefabReference = prefab; // Ví dụ
                return newObj;
            }

            GameObject objFromPool = objectQueue.Dequeue();
            objFromPool.transform.position = position;
            objFromPool.transform.rotation = rotation;
            objFromPool.SetActive(true);
            objFromPool.GetComponent<IPoolableObject>()?.OnGetFromPool();
            return objFromPool;
        }

        public void ReturnObjectToPool(GameObject instance)
        {
            // TODO: Lấy prefab gốc từ instance (ví dụ: qua PoolIdentifier component)
            GameObject prefab = null; // Placeholder
            if (prefab == null) { Destroy(instance); return; } // Không biết prefab gốc thì hủy

            if (!_poolDictionary.TryGetValue(prefab, out Queue<GameObject> objectQueue))
            {
                objectQueue = new Queue<GameObject>();
                _poolDictionary[prefab] = objectQueue;
            }

            instance.GetComponent<IPoolableObject>()?.OnReturnToPool();
            instance.SetActive(false);
            // TODO: Có thể reset transform về parent của pool
            objectQueue.Enqueue(instance);
        }
         // Cần thêm hàm PrewarmPools nếu muốn tạo sẵn object lúc khởi tạo
    }
     // Cần PoolIdentifier component
}