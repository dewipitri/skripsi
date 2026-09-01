using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using BehaviorTree;
using System;

namespace Guard
{
    public class AIGuard : MonoBehaviour
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

        private Node _root;
        private Blackboard _blackboard;
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

            _blackboard = new Blackboard();

            _blackboard.Set("Player", GameObject.FindWithTag("Player"));

            _root = new Selector(new List<Node>
            {
                new Sequence(new List<Node>
                {
                    new LeafNode(CheckVision),
                    new LeafNode(ActionChase),

                    new Sequence(new List<Node>
                    {
                        new LeafNode(CheckDistance),
                        new LeafNode(ActionCatch),
                    }),
                }),


                new LeafNode(ActionPatrol)
            });

            _root.SetBlackboard(_blackboard);
            AssignBlackboardRecursively(_root, _blackboard);
        }

        void Update() => _root.Evaluate();

        private void AssignBlackboardRecursively(Node node, Blackboard bb)
        {
            node.SetBlackboard(bb);
            // Jika node adalah Selector/Sequence, kita perlu turun ke bawah
            if (node is Selector s) { /* Iterasi list nodes di dalam selector */ }
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

        NodeState CheckDistance()
        {
            float dist = Vector3.Distance(
                transform.position,
                ((GameObject)_blackboard.Get<GameObject>("Player")).transform.position
                );

            if (dist <= catchRange)
            {
                return NodeState.Success;
            }
            return NodeState.Failure;
        }

        NodeState ActionCatch()
        {
            //float dist = Vector3.Distance(
            //    transform.position, 
            //    ((GameObject)_blackboard.Get<GameObject>("Player")).transform.position
            //    );
            
            //if (dist <= catchRange)
            //{
                //Debug.Log("Player Tertangkap!");
                CatchingTrigger?.Invoke();
                return NodeState.Success;
            //}
            //return NodeState.Failure;
        }

        NodeState CheckVision()
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

                return NodeState.Success;
            }

            _blackboard.Set("target", null);
            _blackboard.Set("canSeePlayer", false);
            isJustSeePlayer = true;

            return NodeState.Failure;
        }

        NodeState ActionPatrol()
        {
            if (waypoints.Length == 0) return NodeState.Failure;

            if (isWaitingPatrol)
            {
                timeWaited -= Time.deltaTime;

                if (timeWaited <= 0)
                {
                    isWaitingPatrol = false;
                    currentIdx = (currentIdx + 1) % waypoints.Length;
                    _agent.SetDestination(waypoints[currentIdx].position);
                }

                return NodeState.Running;
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
            return NodeState.Running;
        }

        NodeState ActionChase()
        {
            Transform target = _blackboard.Get<Transform>("target");

            if (target == null) return NodeState.Failure;

            float distance = Vector2.Distance(_agent.transform.position, target.position);

            if (distance > 0.5f)
            {
                ChangeAnimation();
                ChangeDirection();
                _agent.SetDestination(target.position);
                return NodeState.Running;
            }

            return NodeState.Success;
        }
    }
}