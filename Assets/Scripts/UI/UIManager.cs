using TMPro;
using UnityEngine;

namespace UI
{
    public class UIManager: MonoBehaviour
    {
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI itemText;

        public GameObject gameoverPanel;

        public GameObject collectibles;

        private void OnEnable()
        {
            //Debug.Log($"Is gamemanager instance == null? {GameManager.Instance == null}");
            GameManager.Instance.RegisterUI(this);
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.UnregisterUI(this);
        }
    }
}