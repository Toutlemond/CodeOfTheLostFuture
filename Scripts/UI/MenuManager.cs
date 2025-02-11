using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MenuManager : MonoBehaviour
    {
        // Ссылка на панель настроек
        public GameObject settingsPanel;

        // Метод для кнопки "Новая игра"
        public void NewGame()
        {
            SceneManager.LoadScene("MainScene");
        }

        // Метод для кнопки "Загрузить"
        public void LoadGame()
        {
            // Реализуй логику загрузки игры здесь
            Debug.Log("Load Game нажата");
        }

        // Метод для кнопки "Игроки"
        public void Players()
        {
            // Реализуй логику для управления игроками здесь
            Debug.Log("Players нажата");
        }

        // Метод для кнопки "Настройки"
        public void OpenSettings()
        {
            settingsPanel.SetActive(true);
        }

        // Метод для закрытия панели настроек
        public void CloseSettings()
        {
            settingsPanel.SetActive(false);
        }

        // Метод для кнопки "Выход"
        public void ExitGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
