using UnityEngine;
using UnityEngine.AI;

public class PathFinding : MonoBehaviour
{
    public Transform target;
    NavMeshAgent agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (target) agent.SetDestination(target.position);
    }
}
