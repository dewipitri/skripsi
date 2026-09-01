using UnityEngine;

namespace Item
{
    public class InteractObject : MonoBehaviour
    {
        public Collider2D areaCollider;
        public Collider2D interactCollider;
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

        public void OnEnter(Collider2D collision)
        {
            if (collision == areaCollider)
            {
                Debug.Log("area collider triggered");
                gameObject.transform.GetChild(0).gameObject.SetActive(true);
            }

            if (collision == interactCollider)
            {
                Debug.Log("interact collider triggered");
                interactable = true;
                player = collision.gameObject;
            }
        }

        public void OnExit(Collider2D collision)
        {
            if (collision == areaCollider)
                gameObject.transform.GetChild(0).gameObject.SetActive(false);

            if (collision == interactCollider) 
            {
                interactable = false;
                player = null;
            }
        }
    }
}
