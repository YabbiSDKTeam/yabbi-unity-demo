using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Utils
{
    public class AppBar : MonoBehaviour
    {
        // Ссылка на кнопку "Назад"
        public Button backButton;

        // Параметр, определяющий, показывать ли кнопку "Назад"
        public bool showBackButton = true;

        private void Start()
        {
            if (backButton == null) return;
            // Устанавливаем видимость кнопки согласно параметру
            backButton.gameObject.SetActive(showBackButton);

            // Назначаем обработчик, только если кнопка видима
            if (showBackButton) backButton.onClick.AddListener(OnBackButtonClicked);
        }

        // Метод, вызываемый при нажатии кнопки "Назад"
        private static void OnBackButtonClicked()
        {
            SceneNavigationManager.Instance.UnloadActiveScene();
        }
    }
}