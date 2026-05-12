using System;
using Unity.Cinemachine;
using UnityEngine;

public class HidePlayer : MonoBehaviour
{
    private bool isPlayerHidden = false;
    public static event Action<bool> OnChangeHideStatus;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerControler player = collision.GetComponent<PlayerControler>();
        if (player != null)
        {
            isPlayerHidden = true;
            player.gameObject.layer = 7;
            player.transform.position = transform.position;
            player.gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerControler player = collision.GetComponent<PlayerControler>();
        if (player != null)
        {
            isPlayerHidden = false;
            player.gameObject.layer = 0;
        }
    }
}
