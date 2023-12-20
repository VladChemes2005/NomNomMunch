using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipTile : MonoBehaviour
{
    public int xIndex;
    public int yIndex;

    public FlipTile(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }

    public void SetIndicies(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }
}
