using UnityEngine;
using UnityEngine.AI;

namespace Test
{
    public class EnemySight : MonoBehaviour
    {
        public float viewRadius = 10f;
        public float viewAngle = 120f;
        public LayerMask targetMask;
        public LayerMask obstacleMask;

        public Transform CurrentTarget { get; private set; }

        private NavMeshAgent agent;

        private void Start()
        {
            agent = GetComponentInParent<NavMeshAgent>();
        }

        private void Update()
        {
            if (agent.velocity.sqrMagnitude > 0.01f)
            {
                float angle = Mathf.Atan2(
                    agent.velocity.y,
                    agent.velocity.x
                ) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        public bool CanSeeTarget()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position,
                viewRadius,
                targetMask);

            foreach (var hit in hits)
            {
                Transform target = hit.transform;

                Vector2 directionToTarget =
                    (target.position - transform.position).normalized;

                float angle =
                    Vector2.Angle(transform.right, directionToTarget);

                if (angle < viewAngle / 2f)
                {
                    float distance =
                        Vector2.Distance(transform.position, target.position);

                    RaycastHit2D wall = Physics2D.Raycast(
                        transform.position,
                        directionToTarget,
                        distance,
                        obstacleMask);

                    if (!wall)
                    {
                        CurrentTarget = target;
                        return true;
                    }
                }
            }

            CurrentTarget = null;
            return false;
        }
    }
}