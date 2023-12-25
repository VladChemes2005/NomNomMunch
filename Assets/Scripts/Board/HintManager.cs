using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    public float hintDelay;
    private float hintDelaySeconds;
    public GameObject hintParticle;
    public GameObject currentHint;
    public List<NodeAction> possibleMoves = new();
    public NodeAction move;
    public static HintManager Instance;

    private void Awake() => Instance = this;
    private void Start() => hintDelaySeconds = hintDelay;
    private void Update()
    {
        if(GameManager.Instance.Data.IsGameEnded)
            enabled = false;
        else
        {
            hintDelaySeconds -= Time.deltaTime;
            if (hintDelaySeconds <= 0 && currentHint == null)
            {
                MarkHint();
                hintDelaySeconds = hintDelay;
            }
        }
    }
    private bool Deadlocked()
    {
        BoardDataHandler dataHandler = BoardDataHandler.Instance;
        BoardData data = dataHandler.Data;

        possibleMoves.Clear();

        for(int x = 1; x < data.width -1; x++) 
        for (int y = 1; y < data.height -1; y++)
        {
            if (!dataHandler.NodeLayout[x, y].IsUsable || dataHandler.NodeLayout[x, y].LinkedAction is null)
                continue;

            if (!CheckValidMoves(x, y, Vector2Int.up) ||
                !CheckValidMoves(x, y, Vector2Int.right) ||
                !CheckValidMoves(x, y, Vector2Int.left) ||
                !CheckValidMoves(x, y, Vector2Int.down))
                return false;
        }
        return true;
    }

    private bool CheckValidMoves(int x, int y, Vector2Int direction)
    {
        BoardDataHandler dataHandler = BoardDataHandler.Instance;
        BoardData data = dataHandler.Data;

        if (dataHandler.NodeLayout[x + direction.x, y + direction.y].IsUsable && dataHandler.NodeLayout[x + direction.x, y + direction.y].LinkedAction is not null)
        {
            BoardUtils.InvisibleSwap(dataHandler.NodeLayout[x, y].LinkedAction as Veggie, dataHandler.NodeLayout[x + direction.x, y + direction.y].LinkedAction as Veggie);
            var boardValid = BoardUtils.CheckBoard(true);
            BoardUtils.InvisibleSwap(dataHandler.NodeLayout[x + direction.x, y + direction.y].LinkedAction as Veggie, dataHandler.NodeLayout[x, y].LinkedAction as Veggie);

            if (!boardValid)
                return true;

            possibleMoves.Add(dataHandler.NodeLayout[x, y].LinkedAction);
            return false;
        }
        return true;
    }

    private void MarkHint()
    {
        if (Deadlocked())
            return;

        move = possibleMoves[0];
        currentHint = Instantiate(hintParticle, move.transform.position, Quaternion.identity);
    }

    public void DestroyHint()
    {
        if (currentHint is null)
            return;

        Destroy(currentHint);
        currentHint = null;
        hintDelaySeconds = hintDelay;
    }

}
