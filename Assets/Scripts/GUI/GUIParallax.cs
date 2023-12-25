using UnityEngine;

public class GUIParallax : MonoBehaviour
{
    [SerializeField]
    private Vector2 _offsetMultiplier;

    private Vector2 _basePosition;

    private void OnValidate() => _basePosition = transform.position;

    private void LateUpdate()
    {
        Vector2 mouseToCenterOffset = new
            (
                Input.mousePosition.x - Screen.height, 
                Input.mousePosition.y - Screen.width
            );

        transform.position = _basePosition + mouseToCenterOffset * _offsetMultiplier;
    }
}
