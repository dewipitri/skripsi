using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace Test
{
    public class AITest : MonoBehaviour
    {
        public Transform[] waypoints;
        public float viewRadius = 10f;
        public float viewAngle = 120f;
        public float catchRange = 0.75f;
        public LayerMask targetMask;
        public LayerMask obstacleMask;

        private BehaviorTree1.Node _root;
        private BehaviorTree1.Blackboard _blackboard;
        private NavMeshAgent _agent;

        private int currentIdx = 0;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, viewRadius);
            Vector3 leftBoundary = Quaternion.Euler(0, 0, -viewAngle / 2) * transform.right;
            Vector3 rightBoundary = Quaternion.Euler(0, 0, viewAngle / 2) * transform.right;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewRadius);
            Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewRadius);
        }

        void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;

            _blackboard = new BehaviorTree1.Blackboard();

            _blackboard.Set("Player", GameObject.FindWithTag("Player"));

            _root = new BehaviorTree1.Selector(new List<BehaviorTree1.Node>
            {
                new BehaviorTree1.Sequence(new List<BehaviorTree1.Node>
                {
                    new BehaviorTree1.LeafNode(CheckVision),
                    new BehaviorTree1.LeafNode(ActionMoveToTarget),
                    new BehaviorTree1.LeafNode(() => {
                        // Logika Catch
                        float dist = Vector3.Distance(transform.position, ((GameObject)_blackboard.Get<GameObject>("Player")).transform.position);
                        if(dist <= catchRange) {
                            Debug.Log("Player Tertangkap!");
                            return BehaviorTree1.NodeState.Success;
                        }
                        return BehaviorTree1.NodeState.Failure;
                    })
                }),

                new BehaviorTree1.LeafNode(ActionPatrol)
            });

            _root.SetBlackboard(_blackboard);
            // Set blackboard ke semua anak secara rekursif (opsional, tergantung implementasi)
            AssignBlackboardRecursively(_root, _blackboard);
        }

        void Update() => _root.Evaluate();

        private void AssignBlackboardRecursively(BehaviorTree1.Node node, BehaviorTree1.Blackboard bb)
        {
            node.SetBlackboard(bb);
            // Jika node adalah Selector/Sequence, kita perlu turun ke bawah
            if (node is BehaviorTree1.Selector s) { /* Iterasi list nodes di dalam selector */ }
            // Catatan: Idealnya Base Node memiliki list children agar rekursi ini mudah.
        }

        void ChangeDirection ()
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();

            if (_agent.velocity.x != 0)
                sprite.flipX = _agent.velocity.x > 0;
        }

        void ChangeAnimation ()
        {
            Animator animator = GetComponent<Animator>();

            if (_agent.velocity != Vector3.zero)
                animator.Play("walking");
            else
                animator.Play("idle");
        }

        BehaviorTree1.NodeState CheckVision()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);

            foreach (var hit in hits)
            {
                Transform target = hit.transform;

                Vector2 directionToTarget = (target.position - transform.position).normalized;

                Vector2 agentForward = _agent.velocity;

                float angleToTarget = Vector2.Angle(agentForward, directionToTarget);

                if (angleToTarget < viewAngle / 2f)
                {
                    float distanceToTarget = (target.position - transform.position).magnitude;
                    RaycastHit2D walls = Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask);

                    if (walls)
                    {
                        // Debug.DrawRay(transform.position, target.position, Color.red);
                        return BehaviorTree1.NodeState.Failure;
                    }
                    else
                    {
                        // Debug.DrawRay(transform.position, target.position, Color.green);
                        _blackboard.Set("target", target);
                        _blackboard.Set("canSeePlayer", true);
                        return BehaviorTree1.NodeState.Success;
                    }
                }
            }

            _blackboard.Set("target", null);
            _blackboard.Set("canSeePlayer", false);
            return BehaviorTree1.NodeState.Failure;
        }

        BehaviorTree1.NodeState ActionPatrol()
        {
            if (waypoints.Length == 0) return BehaviorTree1.NodeState.Failure;

            float angle = Mathf.Atan2(_agent.velocity.y, _agent.velocity.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, 0, angle);
            ChangeAnimation();
            ChangeDirection();
            
            _agent.isStopped = false;
            if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
            {
                currentIdx = (currentIdx + 1) % waypoints.Length;
                _agent.SetDestination(waypoints[currentIdx].position);
            }
            return BehaviorTree1.NodeState.Running;
        }

        BehaviorTree1.NodeState ActionMoveToTarget()
        {
            Transform target = _blackboard.Get<Transform>("target");

            if (target == null) return BehaviorTree1.NodeState.Failure;

            float distance = Vector2.Distance(_agent.transform.position, target.position);

            if (distance > 0.5f)
            {
                float angle = Mathf.Atan2(_agent.velocity.y, _agent.velocity.x) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(0, 0, angle);
                ChangeAnimation();
                ChangeDirection();
                _agent.SetDestination(target.position);
                return BehaviorTree1.NodeState.Running;
            }

            return BehaviorTree1.NodeState.Success;
        }
    }
}