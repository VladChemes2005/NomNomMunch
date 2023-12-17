using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool isUsable;

    public GameObject veggie;

    public Node(bool _isUsable, GameObject _veggie)
    {
        isUsable = _isUsable;
        veggie = _veggie;
    }
}
