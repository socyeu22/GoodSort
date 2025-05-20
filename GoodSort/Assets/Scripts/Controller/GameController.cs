using System.Collections.Generic;
using General;
using UnityEngine;

namespace GameCore
{
    public class GameController : Singleton<GameController>
    {
        private GameData m_gameData;
        [SerializeField] private BoardController m_boardController;
        [SerializeField] private List<LevelData_Flexible> m_levelData;

        private void Awake()
        {
            InitGame();
            InitLevel(0);
        }

        private void InitGame()
        {
            m_gameData = SaveLoadManager.LoadGame();
        }

        private void InitLevel(int level)
        {
            m_boardController.InitBoard(m_levelData[level]);
        }

        private void OnApplicationQuit()
        {
            SaveLoadManager.SaveGame(m_gameData);
        }
    }

    public class GameData
    {
        public int currentLevel;
    }
}