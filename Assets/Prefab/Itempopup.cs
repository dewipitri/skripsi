using UnityEngine;

public class Itempopup : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("player"))
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            Debug.Log("player dekat item");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("player"))
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
