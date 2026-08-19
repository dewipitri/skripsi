using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System;

namespace Test
{
    public class AITester : MonoBehaviour
    {
        [Header("Patrol")]
        public Transform[] waypoints;

        [Header("Agent Capability")]
        public float catchRange = 0.75f;
        public float waitTimePatrol = 2.5f;
        public float waitTimeVision = 0.5f;

        [Header("Layer Mask")]
        public LayerMask targetMask;
        public LayerMask obstacleMask;

        private BehaviorTree1.Node _root;
        private BehaviorTree1.Blackboard _blackboard;
        private NavMeshAgent _agent;

        private int currentIdx = 0;
        private float timeWaited = 0;
        private bool isWaitingPatrol = false;
        private bool isWaitingVision = false;
        private bool isJustSeePlayer = true;
        public static event Action CatchingTrigger;
        private EnemySight vision;

        void Start()
        {
            vision = GetComponentInChildren<EnemySight>();
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
                    new BehaviorTree1.LeafNode(ActionChase),

                    new BehaviorTree1.Sequence(new List<BehaviorTree1.Node>
                    {
                        new BehaviorTree1.LeafNode(CheckDistance),
                        new BehaviorTree1.LeafNode(ActionCatch),
                    }),
                }),


                new BehaviorTree1.LeafNode(ActionPatrol)
            });

            _root.SetBlackboard(_blackboard);
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

        void ChangeDirection()
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();

            if (_agent.velocity.x != 0)
                sprite.flipX = _agent.velocity.x < 0;
        }

        void ChangeAnimation()
        {
            Animator animator = GetComponent<Animator>();

            if (_agent.velocity != Vector3.zero)
                animator.Play("walking");
            else
                animator.Play("idle");
        }

        BehaviorTree1.NodeState CheckDistance()
        {
            float dist = Vector3.Distance(
                transform.position,
                ((GameObject)_blackboard.Get<GameObject>("Player")).transform.position
                );

            if (dist <= catchRange)
            {
                return BehaviorTree1.NodeState.Success;
            }
            return BehaviorTree1.NodeState.Failure;
        }

        BehaviorTree1.NodeState ActionCatch()
        {
            //float dist = Vector3.Distance(
            //    transform.position, 
            //    ((GameObject)_blackboard.Get<GameObject>("Player")).transform.position
            //    );
            
            //if (dist <= catchRange)
            //{
                //Debug.Log("Player Tertangkap!");
                CatchingTrigger?.Invoke();
                return BehaviorTree1.NodeState.Success;
            //}
            //return BehaviorTree1.NodeState.Failure;
        }

        BehaviorTree1.NodeState CheckVision()
        {
            GameObject attention = gameObject.transform.Find("Attention").gameObject;

            if (isWaitingVision)
            {
                if (isJustSeePlayer)
                {
                    attention.SetActive(true);
                    isJustSeePlayer = false;
                }

                timeWaited -= Time.deltaTime;

                //Debug.Log(timeWaited);

                if (timeWaited <= 0)
                {
                    attention.SetActive(false);
                    isWaitingVision = false;
                }

            }

            if (vision.CanSeeTarget())
            {
                if (isJustSeePlayer)
                {
                    isWaitingVision = true;

                    timeWaited = waitTimeVision;
                }

                _blackboard.Set("target", vision.CurrentTarget);
                _blackboard.Set("canSeePlayer", true);

                return BehaviorTree1.NodeState.Success;
            }

            _blackboard.Set("target", null);
            _blackboard.Set("canSeePlayer", false);
            isJustSeePlayer = true;

            return BehaviorTree1.NodeState.Failure;
        }

        BehaviorTree1.NodeState ActionPatrol()
        {
            if (waypoints.Length == 0) return BehaviorTree1.NodeState.Failure;

            if (isWaitingPatrol)
            {
                timeWaited -= Time.deltaTime;

                if (timeWaited <= 0)
                {
                    isWaitingPatrol = false;
                    currentIdx = (currentIdx + 1) % waypoints.Length;
                    _agent.SetDestination(waypoints[currentIdx].position);
                }

                return BehaviorTree1.NodeState.Running;
            }
            ChangeAnimation();
            ChangeDirection();

            _agent.isStopped = false;
            if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
            {
                isWaitingPatrol = true;
                timeWaited = waitTimePatrol;

                _agent.isStopped = true;
            }
            return BehaviorTree1.NodeState.Running;
        }

        BehaviorTree1.NodeState ActionChase()
        {
            Transform target = _blackboard.Get<Transform>("target");

            if (target == null) return BehaviorTree1.NodeState.Failure;

            float distance = Vector2.Distance(_agent.transform.position, target.position);

            if (distance > 0.5f)
            {
                ChangeAnimation();
                ChangeDirection();
                _agent.SetDestination(target.position);
                return BehaviorTree1.NodeState.Running;
            }

            return BehaviorTree1.NodeState.Success;
        }
    }
}