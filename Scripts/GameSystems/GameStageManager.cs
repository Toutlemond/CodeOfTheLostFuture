using UnityEngine;

namespace GameSystems
{
    public enum GameStage {
        Initial,            // Герой ничего не умеет
        BasicProgramming,   // Изучены переменные, циклы
        AdvancedAutomation, // Работа с функциями, ИИ роботов
        FinalChallenge      // Финал игры
    }

    public class GameStageManager : MonoBehaviour {
        public static GameStageManager Instance;
        [SerializeField] public GameStage currentStage = GameStage.Initial;
        void Awake() {
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }
        }

        // Для сохранения/загрузки
        public void SaveProgress() {
            PlayerPrefs.SetInt("GameStage", (int)currentStage);
        }

        public void LoadProgress() {
            currentStage = (GameStage)PlayerPrefs.GetInt("GameStage", 0);
        }
    }
}