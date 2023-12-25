using UnityEngine;

public class NodeAction : MonoBehaviour
{
    public bool CascadeBlock;
    public bool MovementBlock;

    public int xIndex;
    public int yIndex;

    public NodeAction(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }

    public void SetIndicies(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }
}