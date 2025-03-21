using UnityEngine;

namespace Scripts.Utils
{
    public class SafeArea : MonoBehaviour
    {
        // Для отслеживания изменений безопасной зоны (например, при смене ориентации)
        private Rect lastSafeArea = new(0, 0, 0, 0);
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            ApplySafeArea();
        }

        // Обновление safe area в режиме реального времени (например, при смене ориентации)
        private void Update()
        {
            ApplySafeArea();
        }

        private void ApplySafeArea()
        {
            // Получаем безопасную область экрана
            var safeArea = Screen.safeArea;

            // Если safeArea изменилась, пересчитываем якоря
            if (safeArea != lastSafeArea)
            {
                lastSafeArea = safeArea;

                // Вычисляем нормализованные координаты для anchorMin и anchorMax (значения от 0 до 1)
                var anchorMin = safeArea.position;
                var anchorMax = safeArea.position + safeArea.size;
                anchorMin.x /= Screen.width;
                anchorMin.y /= Screen.height;
                anchorMax.x /= Screen.width;
                anchorMax.y /= Screen.height;

                // Устанавливаем якоря, чтобы элемент точно соответствовал безопасной зоне
                rectTransform.anchorMin = anchorMin;
                rectTransform.anchorMax = anchorMax;
                // Обнуляем отступы, чтобы не было дополнительного смещения
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
            }
        }
    }
}