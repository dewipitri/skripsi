using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public GameObject vision;
    public NavMeshAgent agent;
    public float chaseRange = 5f;
    public float catchRange = 1f;
    public float moveSpeed = 3f;
    public Transform[] patrolPoints;

    private int currentPoint = 0;
    private Node root;

    private void Start()
    {
        if(agent==null)
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updatePosition = false;
        //vision = GameObject.Child
        // Buat tree
        transform.rotation = Quaternion.Euler(0, 0, 0);
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
        transform.rotation = Quaternion.Euler(0, 0, 0);

        root.Evaluate();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ////var parent = gameObject;
        ////if (collision.transform.parent == gameObject)
        ////{
        ////    collision.
        //}

        Debug.Log(collision.collider);
    }
    //private Node.NodeState CheckVision()
    //{

    //}
    private Node.NodeState CatchPlayer()
    {
        Debug.Log("Player tertangkap!");
        return Node.NodeState.Success;
    }

    private Node.NodeState ChasePlayer()
    {
        Debug.Log("Mengejar player");
        agent.SetDestination(player.position);
        return Node.NodeState.Running;
    }

    private Node.NodeState Patrol()
    {
        Transform target = patrolPoints[currentPoint];
        //agent.SetDestination(target.position);
        //Debug.Log(agent.destination);
        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
            currentPoint = (currentPoint + 1) % patrolPoints.Length;

        return Node.NodeState.Running;
    }
}
