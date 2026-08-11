using UnityEngine;
using System;

public class ItemCollectible: InteractObject
{
    public static event Action OnInteractObject;

    public override void Interact()
    {
        Debug.Log($"Ambil item {this.name}");
        OnInteractObject?.Invoke();
        Destroy(gameObject);
    }
} 