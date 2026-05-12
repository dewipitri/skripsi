using UnityEngine;

public class InteractObject : MonoBehaviour
{
    protected bool interactable = false;
    protected GameObject player;

    protected virtual void OnEnable()
    {
        PlayerControler.OnInteractItem += TriggerInteract;
    }

    protected virtual void OnDisable()
    {
        PlayerControler.OnInteractItem -= TriggerInteract;
    }

    protected void TriggerInteract()
    {
        if (interactable) Interact();
    }

    public virtual void Interact() { }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            interactable = true;
            player = collision.gameObject;
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            interactable = false;
            player = null;
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
