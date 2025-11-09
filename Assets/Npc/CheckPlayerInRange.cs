using UnityEngine;

public class CheckPlayerInRange : Node
{
    private Transform enemy;
    private Transform player;
    private float range;

    public CheckPlayerInRange(Transform enemy, Transform player, float range)
    {
        this.enemy = enemy;
        this.player = player;
        this.range = range;
    }

    public override NodeState Evaluate()
    {
        float dist = Vector2.Distance(enemy.position, player.position);
        state = dist <= range ? NodeState.Success : NodeState.Failure;
        return state;
    }
}
