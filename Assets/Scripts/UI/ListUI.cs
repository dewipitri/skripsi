using UnityEngine;

namespace UI
{
    public class ListUI : MonoBehaviour
    {
        [SerializeField] private GameObject listPanel;
        public void ChangeVisibility()
        {
            if (listPanel != null)
            {
                bool visible = listPanel.activeSelf;
                listPanel.SetActive(!visible);
            }
        }
    }
}
