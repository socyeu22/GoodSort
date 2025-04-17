using UnityEngine;
using Goodsort.Core;
using Goodsort.Managers; // Namespace chứa các Managers
using Cysharp.Threading.Tasks; // Using UniTask
using UnityEngine.SceneManagement; // Cần để load scene

namespace Goodsort.Initialization
{
    public class GameInitializer : MonoBehaviour
    {
        // Tham chiếu đến các Prefab Manager nếu cần instantiate
        // Hoặc thêm các Manager làm Component trực tiếp vào GameObject này

        async void Start()
        {
            // Đảm bảo chỉ chạy 1 lần
            DontDestroyOnLoad(this.gameObject);

            Debug.Log("[GameInitializer] Starting Initialization...");

            // Đăng ký các service theo đúng thứ tự
            try
            {
                // 1. Configs (nếu có SO riêng)
                // ServiceProvider.RegisterService<GameConfig>(LoadGameConfigSO());

                // 2. Save/Load
                var saveLoadManager = new SaveLoadManager();
                saveLoadManager.Initialize(); // Initialize trước khi đăng ký
                ServiceProvider.RegisterService<ISaveLoadService>(saveLoadManager);

                // 3. Pooling
                var poolManager = gameObject.AddComponent<PoolManager>(); // Add Component vào chính Initializer
                poolManager.Initialize();
                ServiceProvider.RegisterService<PoolManager>(poolManager);

                // 4. Load Player Data
                PlayerData playerData = await saveLoadManager.LoadPlayerDataAsync();
                // TODO: Có thể cần một service riêng để quản lý PlayerData
                // ServiceProvider.RegisterService<PlayerData>(playerData);

                // 5. Currency
                var currencyManager = gameObject.AddComponent<CurrencyManager>();
                // Truyền ISaveLoadService hoặc PlayerData vào Initialize nếu cần
                currencyManager.Initialize(saveLoadManager, playerData.playerCoins); // Ví dụ
                ServiceProvider.RegisterService<CurrencyManager>(currencyManager);

                // 6. REM Data Provider
                var remDataProvider = new REMDataProvider(); // Class thường
                remDataProvider.Initialize(saveLoadManager); // Cần SaveLoad để đọc lịch sử
                ServiceProvider.RegisterService<REMDataProvider>(remDataProvider);

                // 7. Các Managers khác (GameState, Level, Board, Time, Audio, Ads, Analytics...)
                var gameStateManager = gameObject.AddComponent<GameStateManager>();
                gameStateManager.Initialize();
                ServiceProvider.RegisterService<GameStateManager>(gameStateManager);

                var levelManager = gameObject.AddComponent<LevelManager>();
                // LevelManager Initialize sẽ lấy các service khác từ ServiceProvider
                levelManager.Initialize();
                ServiceProvider.RegisterService<LevelManager>(levelManager);

                 // ... Đăng ký các manager khác tương tự ...
                 // BoardManager, TimeManager thường được tạo/quản lý bởi LevelManager hoặc Game Scene

                Debug.Log("[GameInitializer] All essential services registered.");

                // Chuyển sang Scene Main Menu sau khi khởi tạo xong
                await LoadMainMenuSceneAsync();

            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[GameInitializer] Initialization Failed: {ex.Message}\n{ex.StackTrace}");
                // TODO: Hiển thị lỗi cho người dùng hoặc xử lý khác
            }
        }

        private async UniTask LoadMainMenuSceneAsync()
        {
            // Thay "MainMenu" bằng tên Scene Main Menu thực tế của bạn
            Debug.Log("[GameInitializer] Loading Main Menu Scene...");
            await SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
            // Có thể cần thêm logic sau khi load scene xong
        }
    }
     // Cần tạo class REMDataProvider
     // Cần tạo các class Manager còn lại với hàm Initialize cơ bản
}