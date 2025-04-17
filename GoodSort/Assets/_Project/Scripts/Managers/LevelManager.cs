using Goodsort.Core;
using Goodsort.Data;
using UnityEngine;
using Cysharp.Threading.Tasks; // Using UniTask

namespace Goodsort.Managers
{
    public class LevelManager : MonoBehaviour
    {
        private ProceduralLevelGenerator _levelGenerator;
        private BoardManager _boardManager;
        private TimeManager _timeManager;
        // ... tham chiếu đến các manager khác nếu cần

        // Tham chiếu đến các LevelDataSO fallback
        [SerializeField] private LevelDataSO fallbackEasy;
        [SerializeField] private LevelDataSO fallbackMedium;
        // ...

        public void Initialize()
        {
            // Lấy tham chiếu qua ServiceProvider
            _levelGenerator = ServiceProvider.GetService<ProceduralLevelGenerator>();
            _boardManager = ServiceProvider.GetService<BoardManager>();
            _timeManager = ServiceProvider.GetService<TimeManager>();
             Debug.Log("[LevelManager] Initialized.");
            // ...
        }

        public async UniTask LoadLevelAsync(string levelIdentifier)
        {
            await ServiceProvider.GetService<GameStateManager>().SwitchStateAsync(GameStateType.Loading);

            // Tạm thời load template theo tên, sau này có thể phức tạp hơn
            LevelTemplateSO template = Resources.Load<LevelTemplateSO>($"LevelTemplates/{levelIdentifier}"); // Ví dụ load từ Resources

            if (template == null)
            {
                Debug.LogError($"[LevelManager] Failed to load LevelTemplateSO for identifier: {levelIdentifier}");
                // TODO: Xử lý lỗi (ví dụ: quay lại main menu)
                await ServiceProvider.GetService<GameStateManager>().SwitchStateAsync(GameStateType.MainMenu); // Ví dụ
                return;
            }

            REMInputParams remParams = await ServiceProvider.GetService<REMDataProvider>().GetREMInputParamsAsync(template.templateID); // Pass template ID

            await ServiceProvider.GetService<GameStateManager>().SwitchStateAsync(GameStateType.Generating);
            ServiceProvider.GetService<UIManager>().ShowLoadingIndicator(true); // Giả sử có UIManager

            GenerationResult result = null;
            int maxRetries = ServiceProvider.GetService<GameConfig>().MAX_GENERATION_RETRIES; // Giả sử có GameConfig service

            for (int i = 0; i < maxRetries; i++)
            {
                result = await _levelGenerator.GenerateLevelAsync(template, remParams, i);
                if (result.Success) break;
                Debug.LogWarning($"[LevelManager] Level generation attempt {i + 1} failed. Retrying...");
                await UniTask.Yield(); // Chờ 1 frame
            }

             ServiceProvider.GetService<UIManager>().ShowLoadingIndicator(false);

            GeneratedLevelData levelData;
            if (result != null && result.Success)
            {
                levelData = result.GeneratedData;
                 Debug.Log($"[LevelManager] Level generated successfully from template: {levelData.sourceTemplateID}");
            }
            else
            {
                Debug.LogError($"[LevelManager] Level generation failed permanently for template: {template.templateID}. Loading fallback.");
                levelData = LoadFallbackLevelData(template.targetDifficulty); // Load fallback SO
                if (levelData == null)
                {
                     Debug.LogError("[LevelManager] Fallback LevelDataSO is missing!");
                     await ServiceProvider.GetService<GameStateManager>().SwitchStateAsync(GameStateType.MainMenu); // Ví dụ xử lý lỗi nghiêm trọng
                    return;
                }
                levelData.generationSuccess = false; // Đánh dấu là fallback
            }

            SetupBoard(levelData);
            await ServiceProvider.GetService<GameStateManager>().SwitchStateAsync(GameStateType.Playing);
        }

         private GeneratedLevelData LoadFallbackLevelData(Difficulty difficulty)
         {
             // Logic chọn fallback SO phù hợp dựa trên difficulty
             // Ví dụ đơn giản:
             LevelDataSO fallbackSO = fallbackMedium; // Mặc định
             // if (difficulty == Difficulty.Easy) fallbackSO = fallbackEasy;
             // ...
             if (fallbackSO == null) return null;

             // Chuyển đổi LevelDataSO tĩnh thành GeneratedLevelData (cần implement hàm này)
             // return ConvertStaticLevelToGenerated(fallbackSO);
             Debug.LogWarning("Fallback level loading not fully implemented yet.");
             return null; // Placeholder
         }

        private void SetupBoard(GeneratedLevelData data)
        {
            Debug.Log("[LevelManager] Setting up board...");
            _boardManager.InitializeBoard(data);
            _timeManager.SetTime(data.finalTimeLimit);
            // Khởi tạo các hệ thống khác cần dữ liệu level
        }
    }
     // Cần định nghĩa GenerationResult class/struct
     public class GenerationResult { public bool Success; public GeneratedLevelData GeneratedData; public string ErrorMessage; }
     // Cần định nghĩa GameConfig service/SO
     // Cần định nghĩa REMDataProvider service
     // Cần định nghĩa UIManager service
}