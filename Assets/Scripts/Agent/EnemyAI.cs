using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class Blackboard
{
    public Transform player;
    public GameObject vision;
    public NavMeshAgent agent;
    public Transform[] patrolPoints;

    
    public int currentPoint = 0;
}
public class EnemyAI : MonoBehaviour
{
    public Blackboard blackboard;
    //public Transform player;
    //public GameObject vision;
    //public NavMeshAgent agent;
    public float chaseRange = 5f;
    public float catchRange = 1f;
    public float moveSpeed = 3f;
    //public Transform[] patrolPoints;
    Animator animator;
    //private int currentPoint = 0;
    private Node root;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if(blackboard.agent==null)
        //agent = GetComponent<NavMeshAgent>();
        //agent.updateRotation = false;

        blackboard.agent = GetComponent<NavMeshAgent>();
        blackboard.agent.updateRotation = false;

        //agent.updatePosition = false;
        //vision = GameObject.Child
        // Buat tree
        transform.rotation = Quaternion.Euler(0, 0, 0);
        Node catchPlayer = new Sequence(new List<Node> {
            //new CheckPlayerInRange(transform, blackboard.player, catchRange),
            new CheckPlayerInRange(),
            new ActionNode(() => CatchPlayer())
        });

        Node chasePlayer = new Sequence(new List<Node> {
            new CheckPlayerInRange(),
            new ActionNode(() => ChasePlayer())
        });

        Node patrol = new ActionNode(() => Patrol());

        root = new Selector(new List<Node> { catchPlayer, chasePlayer, patrol });
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        animator.SetFloat("Speed", blackboard.agent.velocity.magnitude);
        root.Evaluate();
    }

    private Node.NodeState CatchPlayer()
    {
        Debug.Log("Player tertangkap!");
        return Node.NodeState.Success;
    }

    private Node.NodeState ChasePlayer()
    {
        Debug.Log("Mengejar player");
        blackboard.agent.SetDestination(blackboard.player.position);
        return Node.NodeState.Running;
    }

    private Node.NodeState Patrol()
    {
        Transform target = blackboard.patrolPoints[blackboard.currentPoint];
         blackboard.agent.SetDestination(target.position);
         
        //Debug.Log(agent.destination);
        //transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
            blackboard.currentPoint = (blackboard.currentPoint + 1) % blackboard.patrolPoints.Length;

        return Node.NodeState.Running;
    }
}
