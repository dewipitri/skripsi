using UnityEngine;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float chaseRange = 5f;
    public float catchRange = 1f;
    public float moveSpeed = 3f;
    public Transform[] patrolPoints;

    private int currentPoint = 0;
    private Node root;

    private void Start()
    {
        // Buat tree
        Node catchPlayer = new Sequence(new List<Node> {
            new CheckPlayerInRange(transform, player, catchRange),
            new ActionNode(() => CatchPlayer())
        });

        Node chasePlayer = new Sequence(new List<Node> {
            new CheckPlayerInRange(transform, player, chaseRange),
            new ActionNode(() => ChasePlayer())
        });

        Node patrol = new ActionNode(() => Patrol());

        root = new Selector(new List<Node> { catchPlayer, chasePlayer, patrol });
    }

    private void Update()
    {
        root.Evaluate();
    }

    private Node.NodeState CatchPlayer()
    {
        Debug.Log("Player tertangkap!");
        return Node.NodeState.Success;
    }

    private Node.NodeState ChasePlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        return Node.NodeState.Running;
    }

    private Node.NodeState Patrol()
    {
        Transform target = patrolPoints[currentPoint];
        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
            currentPoint = (currentPoint + 1) % patrolPoints.Length;

        return Node.NodeState.Running;
    }
}
