using General;

namespace DefaultNamespace
{
    public class GameConfig : Singleton<GameConfig>
    {
        public ItemIconConfig itemIconConfig;
        public PrefabConfig prefabConfig;
    }
}