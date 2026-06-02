using System;
using System.Collections.Generic;

namespace BehaviorTree1
{
    public class Blackboard
    {
        private Dictionary<string, object> data = new Dictionary<string, object>();

        public void Set(string key, object value) => data[key] = value;

        public T Get<T>(string key)
        {
            if (data.ContainsKey(key)) return (T)data[key];
            return default;
        }

        public bool HasKey(string key) => data.ContainsKey(key);
    }

    public enum NodeState { Running, Success, Failure }

    public abstract class Node
    {
        protected NodeState state;
        protected Blackboard blackboard;

        public NodeState State => state;

        public void SetBlackboard(Blackboard bb) => blackboard = bb;

        public abstract NodeState Evaluate();
    }

    // Selector: Berhenti jika ada yang sukses
    public class Selector : Node
    {
        protected List<Node> nodes = new List<Node>();

        public Selector(List<Node> nodes) => this.nodes = nodes;

        public override NodeState Evaluate()
        {
            foreach (var node in nodes)
            {
                switch (node.Evaluate())
                {
                    case NodeState.Failure: continue;
                    case NodeState.Success: return state = NodeState.Success;
                    case NodeState.Running: return state = NodeState.Running;
                }
            }
            return state = NodeState.Failure;
        }
    }

    // Sequence: Berhenti jika ada yang gagal
    public class Sequence : Node
    {
        protected List<Node> nodes = new List<Node>();

        public Sequence(List<Node> nodes) => this.nodes = nodes;

        public override NodeState Evaluate()
        {
            bool anyChildRunning = false;
            foreach (var node in nodes)
            {
                switch (node.Evaluate())
                {
                    case NodeState.Success: continue;
                    case NodeState.Failure: return state = NodeState.Failure;
                    case NodeState.Running: anyChildRunning = true; continue;
                }
            }
            return state = anyChildRunning ? NodeState.Running : NodeState.Success;
        }
    }

    public class LeafNode : Node
    {
        private Func<NodeState> action;

        public LeafNode(Func<NodeState> action) => this.action = action;

        public override NodeState Evaluate() => action();
    }
}