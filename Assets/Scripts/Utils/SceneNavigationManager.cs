using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Utils
{
    public class SceneNavigationManager : MonoBehaviour
    {
        // Singleton-паттерн для удобного доступа из других скриптов
        public static SceneNavigationManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // Не уничтожаем объект при загрузке новой сцены
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            // Обработка системной кнопки "Назад" (Escape)
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            if (SceneManager.sceneCount > 1)
                UnloadActiveScene();
            else
                Application.Quit();
        }

        /// <summary>
        ///     Загружает сцену с указанным именем в аддитивном режиме.
        ///     После загрузки новая сцена становится активной.
        /// </summary>
        public void LoadSceneAdditively(string sceneName)
        {
            StartCoroutine(LoadSceneAdditiveCoroutine(sceneName));
        }

        /// <summary>
        ///     Перегрузка: загружает текущую сцену (ее копию) поверх уже загруженной.
        /// </summary>
        public void LoadCurrentSceneAdditively()
        {
            var currentSceneName = SceneManager.GetActiveScene().name;
            LoadSceneAdditively(currentSceneName);
        }

        private IEnumerator LoadSceneAdditiveCoroutine(string sceneName)
        {
            var asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            yield return new WaitUntil(() => asyncLoad is {isDone: true});
            // После загрузки делаем новую сцену активной (она всегда последняя в списке)
            var loadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            SceneManager.SetActiveScene(loadedScene);
        }

        /// <summary>
        ///     Выгружает активную сцену, если загружено более одной.
        ///     Позволяет вернуться к предыдущей сцене.
        /// </summary>
        public void UnloadActiveScene()
        {
            if (SceneManager.sceneCount <= 1) return;
            var activeScene = SceneManager.GetActiveScene();
            StartCoroutine(UnloadSceneCoroutine(activeScene));
        }

        private IEnumerator UnloadSceneCoroutine(Scene scene)
        {
            var asyncUnload = SceneManager.UnloadSceneAsync(scene);
            yield return new WaitUntil(() => asyncUnload is {isDone: true});
        }
    }
}