using UnityEngine;
using UnityEngine.SceneManagement;

public class NextMap : MonoBehaviour
{
    public bool isEnd = false;
    public string nextScene;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
