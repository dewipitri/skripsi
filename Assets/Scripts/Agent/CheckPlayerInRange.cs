using Unity.VisualScripting;
using UnityEngine;

public class CheckPlayerInRange : Node
{
    //private Transform enemy;
    //private Transform player;
    //private float range;

    private bool isPlayerVisible = false;
  
    //public CheckPlayerInRange(Transform enemy, Transform player, float range)
    //{
    //    this.enemy = enemy;
    //    this.player = player;
    //    this.range = range;
    //}

    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerControler player = collision.GetComponent<PlayerControler>();
        if (player != null)
        {
            isPlayerVisible = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        PlayerControler player = collision.GetComponent<PlayerControler>();
        if (player != null)
        {
            isPlayerVisible = false;
        }
    }
    public override NodeState Evaluate()
    {
        state = isPlayerVisible ? NodeState.Success : NodeState.Failure;
        //float dist = Vector2.Distance(enemy.position, player.position);
        //state = dist <= range ? NodeState.Success : NodeState.Failure;
        return state;
    }
}
