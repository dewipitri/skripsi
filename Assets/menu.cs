using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    public void StartButton(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
