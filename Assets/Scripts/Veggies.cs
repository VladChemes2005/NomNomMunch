using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Veggies : MonoBehaviour
{

    public VegType veggiesType;
        
    public int xIndex;
    public int yIndex;

    public bool isMatched;
    private Vector2 currentPos;
    private Vector2 targetPos;
    
    public bool isMoving;

    public Veggies(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum VegType
{
    cabbage,
    carrot,
    corn,
    cucumber,
    eggplant,
    green_onion,
    onion,
    pepper,
    potato,
    tomato
}