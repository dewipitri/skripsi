using UnityEngine;
using System;

public class Itempopup: InteractObject
{
    public static event Action OnInteractObject;
    public override void Interact()
    {
        Debug.Log("interact objek");
        OnInteractObject?.Invoke();
        Destroy(gameObject);
    }
} 