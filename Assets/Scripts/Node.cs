using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool isUsable;

    public GameObject veg;

    public Node(bool _isUsable, GameObject _veg)
    {
        isUsable = _isUsable;
        veg = _veg;
    }
}
