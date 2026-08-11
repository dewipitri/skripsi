using UnityEngine;

public class TriggerRelay : MonoBehaviour
{
    [SerializeField] private InteractObject interactObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            interactObject.OnEnter(GetComponent<Collider2D>());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            interactObject.OnEnter(GetComponent<Collider2D>());
    }
}
