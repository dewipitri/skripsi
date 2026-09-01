using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class menu : MonoBehaviour
    {
        public void PlayButton(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            SceneManager.LoadSceneAsync("Tutorial", LoadSceneMode.Additive);
        }

        public void UnloadTutorial()
        {
            SceneManager.UnloadSceneAsync("Tutorial", UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        }

        public void StartButton(string scenename)
        {
            SceneManager.LoadScene(scenename);
        }

        public void ExitButton()
        {
            Application.Quit();
        }
    }
}
