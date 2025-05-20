using System;

namespace GameCore
{
    [Serializable]
    public class ItemData
    {
        public int id;
        public ItemVisualType visualType;
    }

    [Serializable]
    public enum ItemVisualType
    {
        FullDisplay,
        SecretDisplay
    }
}
