using UnityEngine;

public class Itempopup: InteractObject
{
    public override void Interact()
    {
        Debug.Log("interact objek");
        Destroy(gameObject);
    }
} 