using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Veggie : MonoBehaviour
{

    public VeggieType veggiesType;
        
    public int xIndex;
    public int yIndex;

    public bool isMatched;
    private Vector2 currentPos;
    private Vector2 targetPos;
    
    public bool isMoving;

    public Veggie(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }
    
    public void SetIndicies(int _x, int _y)
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

    #region
    // Move target
    public void MoveToTarget(Vector2 targetPos)
    {
        StartCoroutine(MoveCoroutine(targetPos));

    }
    
    //Move coroutine
    private IEnumerator MoveCoroutine(Vector2 targetPos)
    {
        isMoving = true;
        float duration = 0.2f;

        Vector2 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            transform.position = Vector2.Lerp(startPosition, targetPos, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }
    #endregion
}

public enum VeggieType
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