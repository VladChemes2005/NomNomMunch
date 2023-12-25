using UnityEngine;

public class Node : MonoBehaviour
{
    public bool IsUsable;

    public NodeAction LinkedAction 
    {
        get => _veggie;
        set
        {
            if (value is null)
                return;

            _veggie = value;
        }
    }

    protected NodeAction _veggie;

    public Node(bool isUsable, NodeAction veggie)
    {
        IsUsable = isUsable;
        _veggie = veggie;
    }
}
