using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class CarouselController : MonoBehaviour
    {
        public ScrollRect scrollRect;
        public RectTransform content;

        private float itemWidth;
        private int currentIndex = 0;

        private void Start()
        {
            RectTransform rect = content.GetComponentInChildren<RectTransform>();
            itemWidth = rect.rect.size.x;
        }

        public void Next()
        {
            currentIndex++;

            if (currentIndex >= content.childCount)
                currentIndex = content.childCount - 1;

            MoveToCurrentItem();
        }

        public void Previous()
        {
            currentIndex--;

            if (currentIndex < 0)
                currentIndex = 0;

            MoveToCurrentItem();
        }

        void MoveToCurrentItem()
        {
            float targetX = currentIndex * itemWidth;
            Debug.Log("Index: " + currentIndex);

            Vector2 position = content.anchoredPosition;
            position.x = -targetX;

            content.anchoredPosition = position;
        }

        public void UnloadTutorial()
        {
            SceneManager.UnloadSceneAsync("Tutorial");
        }
    }
}