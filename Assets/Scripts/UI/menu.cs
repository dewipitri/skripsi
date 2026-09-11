using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Alternate
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

        public void RetryButton()
        {
            Debug.Log("Retry CLicked");
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);

            //GameManager.Instance.StartTime();
        }

        public void StartButton(string scenename)
        {
            SceneManager.LoadScene(scenename);
            //GameManager.Instance = null;
            //Destroy(GameManager.Instance);
        }

        public void ExitButton()
        {
            Application.Quit();
        }
    }
}
