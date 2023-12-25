using System.Collections;
using UnityEngine;

public class MoveableNodeAction : NodeAction
{
    public delegate void OnMovedHandler();
    public OnMovedHandler OnMoved;

    protected Vector2 _currentPos;
    protected Vector2 _targetPos;

    public bool isMoving { get; set; }

    public MoveableNodeAction(int x, int y) : base(x, y) { }

    #region Movement
    public void MoveToTarget(Vector2 targetPos) => StartCoroutine(MoveCoroutine(targetPos));
    
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

        OnMoved?.Invoke();
    }

    #endregion
}