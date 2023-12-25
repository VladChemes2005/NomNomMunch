using System.Collections.Generic;
using UnityEngine;

public class BoardVisualisator : MonoBehaviour
{
    public static BoardVisualisator Instance;

    private BoardData _data;

    private void Awake() => Instance = this;

    private void Start()
    {
        _data = BoardDataHandler.Instance.Data;   
    }
}

