using System.Collections.Generic;

public abstract class Node
{
    public enum NodeState { Running, Success, Failure }
    protected NodeState state;
    public NodeState State => state;
    public abstract NodeState Evaluate();
}

public class Selector : Node
{
    private List<Node> nodes;
    public Selector(List<Node> nodes) => this.nodes = nodes;

    public override NodeState Evaluate()
    {
        foreach (Node node in nodes)
        {
            switch (node.Evaluate())
            {
                case NodeState.Running:
                    state = NodeState.Running;
                    return state;
                case NodeState.Success:
                    state = NodeState.Success;
                    return state;
                default:
                    continue;
            }
        }
        state = NodeState.Failure;
        return state;
    }
}

public class Sequence : Node
{
    private List<Node> nodes;
    public Sequence(List<Node> nodes) => this.nodes = nodes;

    public override NodeState Evaluate()
    {
        bool anyRunning = false;

        foreach (Node node in nodes)
        {
            switch (node.Evaluate())
            {
                case NodeState.Failure:
                    state = NodeState.Failure;
                    return state;
                case NodeState.Running:
                    anyRunning = true;
                    break;
                case NodeState.Success:
                    continue;
            }
        }
        state = anyRunning ? NodeState.Running : NodeState.Success;
        return state;
    }
}
