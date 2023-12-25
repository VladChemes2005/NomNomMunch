using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardDataHandler : MonoBehaviour
{
    public static BoardDataHandler Instance;
    
    [SerializeField]
    private BoardData _data;
    [HideInInspector]
    public BoardData Data;
    [HideInInspector]
    public Node[,] NodeLayout;
    [HideInInspector]
    public readonly List<Object> TilesToDestroy = new();
    [HideInInspector]
    public readonly List<NodeAction> NodeActionToDestroy = new();

    private HashSet<Object> _managedObjects = new();

    private void Awake() => Instance = this;
    private void Start()
    {
        Data.TileParent = transform.GetChild(0);
        Data = Instantiate(Data);
    }
    public IEnumerator ProcessTurnOnMatchesBoard(bool substractMoves)
    {
        List<Veggie> veggiesToRemove = new();

        foreach (NodeAction node in NodeActionToDestroy)
            if (node is Veggie veggie)
            {
                veggiesToRemove.Add(veggie);
                veggie.isMatched = false;
            }

        BoardUtils.HandleEnergyIncrease();

        RemoveAndRefill(veggiesToRemove);
        GameManager.Instance.ProcessTurn(veggiesToRemove.Count * BoardCycle.PieceValue * BoardCycle.Streak, substractMoves);
        yield return new WaitForSeconds(0.4f);

        if (BoardUtils.CheckBoard(false))
        {
            BoardCycle.Streak += 1;
            StartCoroutine(ProcessTurnOnMatchesBoard(false));
        }
        BoardCycle.Streak = 1;
    }

    public NodeAction InitialisePrefab(NodeAction prefab, Vector2 position, int x, int y, bool isUsable, bool AddToDestroy)
    {
        NodeAction action = InitialisePrefabNonActive(prefab, position) as NodeAction;
        if(AddToDestroy)
            NodeActionToDestroy.Add(action);
        NodeLayout[x, y] = new(isUsable, action);
        NodeLayout[x, y].LinkedAction.SetIndicies(x, y);

        return action;
    }
    public Object InitialisePrefabNonActive(Object prefab, Vector2 position)
    {
        Object @object = InstantiateOnBoard(prefab, position, Quaternion.identity);

        return @object;
    }

    public T InstantiateOnBoard<T>(T @object) where T : Object
    {
        T instantiatedObject = Instantiate(@object, Data.TileParent);
        _managedObjects.Add(instantiatedObject);
        return instantiatedObject;
    }
    public T InstantiateOnBoard<T>(T @object, Vector2 position, Quaternion quaternion) where T : Object
    {
        T instantiatedObject = Instantiate(@object, position, quaternion);

        if (instantiatedObject is GameObject gameObject)
            gameObject.transform.SetParent(Data.TileParent);
        else if (instantiatedObject is Component component)
            component.gameObject.transform.SetParent(Data.TileParent);

        _managedObjects.Add(instantiatedObject);
        return instantiatedObject;
    }

    public void DestroyNodeActions()
    {
        var nodeActions = NodeActionToDestroy;
        if (nodeActions is null)
            return;

        foreach (var nodeAction in nodeActions)
            Destroy(nodeAction.gameObject);

        nodeActions.Clear();
    }
    public void DestroyAll()
    {
        foreach (var item in _managedObjects)
            Destroy(item);

        _managedObjects.Clear();
    }

    public void SwapVeggies(Veggie currentVeg, Veggie targetVeg)
    {
        Debug.Log(currentVeg == targetVeg);
        if (!BoardUtils.IsAdjacent(currentVeg, targetVeg))
            return;
        
        BoardUtils.DoSwap(currentVeg, targetVeg);

        BoardCycle.IsProcessingMove = true;

        StartCoroutine(ProcessMatches(currentVeg, targetVeg));
    }
    private IEnumerator ProcessMatches(Veggie currentVeg, Veggie targetVeg)
    {
        yield return new WaitForSeconds(0.2f);

        if (BoardUtils.CheckBoard(false))
            StartCoroutine(ProcessTurnOnMatchesBoard(true));
        else
            BoardUtils.DoSwap(currentVeg, targetVeg);
        BoardCycle.IsProcessingMove = false;
    }
    private void RemoveAndRefill(List<Veggie> veggiesToRemove)
    {
        List<NodeAction> nodeActionToRemove = new();

        //Define what is necessary to Destroy
        foreach (Veggie veggie in veggiesToRemove)
        {
            int xIndex = veggie.xIndex;
            int yIndex = veggie.yIndex;

            NodeAction nodeToRemove = null;

            if (Data.BoardLayout.Any(tile => tile.x == xIndex && tile.y == yIndex && tile.TileType == TileType.Ice))
            {
                foreach (NodeAction node in NodeActionToDestroy)
                {
                    if (node is not Ice ice || 
                        ice.xIndex != xIndex || 
                        ice.yIndex != yIndex)
                        continue;

                    nodeToRemove = ice;
                    for (int i = 0; i < Data.BoardLayout.Length; i++)
                    {
                        if (Data.BoardLayout[i].x != ice.xIndex || 
                            Data.BoardLayout[i].y != ice.yIndex)
                            continue;
                        
                        Data.BoardLayout[i].TileType = TileType.Normal;
                    }
                    break;
                }
            }
            else if (Data.BoardLayout.Any(tile => tile.x == xIndex && tile.y == BoardUtils.FindIndexOfLowestNull(xIndex) && tile.TileType == TileType.Bug))
            {
                foreach (NodeAction node in NodeActionToDestroy)
                {
                    if (node is not Bug bug || 
                        bug.xIndex != xIndex ||
                        bug.yIndex != yIndex ||
                        yIndex != BoardUtils.FindIndexOfLowestNull(xIndex))
                        continue;

                    Destroy(bug.gameObject);
                    break;
                }
            }
            else
            {
                FlipTile newFlipTile = null;

                foreach (NodeAction node in NodeActionToDestroy)
                {
                    if (node is not FlipTile flipTile || flipTile.xIndex != xIndex || flipTile.yIndex != yIndex)
                        continue;

                    newFlipTile = flipTile;
                    break;
                }

                var goalIndex = BoardUtils.FindByElement(GameManager.Instance.Data.GoalTile, veggie.veggiesType);
                if (goalIndex != -1)
                    GameManager.Instance.Data.GoalTileGoals[goalIndex] -= 1;

                Destroy(veggie.gameObject);

                NodeLayout[xIndex, yIndex] = new(true, null);
            }

            if (nodeToRemove != null)
                nodeActionToRemove.Add(nodeToRemove);
        }

        //Destroy everything that is necessary
        foreach (NodeAction nodeAction in nodeActionToRemove)
        {
            NodeActionToDestroy.Remove(nodeAction);
            Destroy(nodeAction.gameObject);
        }

        // Refill the board
        for (int x = 0; x < Data.width; x++)
            for (int y = 0; y < Data.height; y++)
                if (NodeLayout[x, y].LinkedAction is null && NodeLayout[x, y].IsUsable)
                    RefillVeggie(x, y);
    }
    private void RefillVeggie(int x, int y)
    {
        int yOffset = 1;

        while (y + yOffset < Data.height && NodeLayout[x, y + yOffset].LinkedAction == null)
            yOffset++;
        if (y + yOffset < Data.height && NodeLayout[x, y + yOffset].LinkedAction != null)
        {
            Veggie veggieAbove = NodeLayout[x, y + yOffset].LinkedAction.GetComponent<Veggie>();

            Vector3 targetPos = new Vector3(x - Data.position.x, y - Data.position.y, veggieAbove.transform.position.z);
            veggieAbove.MoveToTarget(targetPos);
            veggieAbove.SetIndicies(x, y);
            NodeLayout[x, y] = NodeLayout[x, y + yOffset];
            NodeLayout[x, y + yOffset] = new(true, null);
        }
        if (y + yOffset == Data.height)
            SpawnVeggieAtTop(x);
    }
    private void SpawnVeggieAtTop(int x)
    {
        int index = BoardUtils.FindIndexOfLowestNull(x);
        if (index == 99)
            return;
        
        int locationToMoveTo = 7 - index;

        int randomIndex = Random.Range(0, Data.VeggiesPrefabs.Length);
        Veggie newVeggie = InitialisePrefab(Data.VeggiesPrefabs[randomIndex], new Vector2(x, Data.height) - Data.position, x, index, true, false) as Veggie;
        Vector3 position = newVeggie.gameObject.transform.position;

        Vector3 targetPosition = new Vector3(position.x, position.y - locationToMoveTo, position.z);
        newVeggie.MoveToTarget(targetPosition);
    }

    public void RemovePlusShape(int startX, int startY)
    {
        for (int i = 0; i < Data.width; i++)
        {
            if (NodeLayout[i, startY].LinkedAction is null)
                continue;

            Veggie veggieToRemove = NodeLayout[i, startY].LinkedAction as Veggie;
            NodeActionToDestroy.Add(veggieToRemove);

            veggieToRemove.isMatched = true;
            Destroy(NodeLayout[i, startY].LinkedAction);
            NodeLayout[i, startY] = new(true, null);
        }
        for (int i = 0; i < Data.height; i++)
        {
            if (NodeLayout[startX, i].LinkedAction is null)
                continue;
            Veggie veggieToRemove = NodeLayout[startX, i].LinkedAction as Veggie;
            NodeActionToDestroy.Add(veggieToRemove);

            veggieToRemove.isMatched = true;
            Destroy(NodeLayout[startX, i].LinkedAction);
            NodeLayout[startX, i] = new(true, null);
        }
    }
    public void RemoveSameType(int startX, int startY)
    {
        VeggieType veggieTypeToRemove = NodeLayout[startX, startY].LinkedAction.GetComponent<Veggie>().veggiesType;
        for (int i = 0; i < Data.width; i++)
        for (int j = 0; j < Data.height; j++)
        {
            if (NodeLayout[i, j].LinkedAction is null ||
                NodeLayout[i, j].LinkedAction.GetComponent<Veggie>().veggiesType != veggieTypeToRemove)
                continue;

            Veggie veggieToRemove = NodeLayout[i, j].LinkedAction.GetComponent<Veggie>();
            NodeActionToDestroy.Add(veggieToRemove);

            veggieToRemove.isMatched = true;
            Destroy(NodeLayout[i, j].LinkedAction);
            NodeLayout[i, j] = new(true, null);
        }
    }

    public List<Veggie> GetVeggiesToRemove()
    {
        List<Veggie> veggiesToRemove = new();
        for (int i = 0; i < NodeActionToDestroy.Count; i++)
            if (NodeActionToDestroy[i] is Veggie veggie)
                veggiesToRemove.Add(veggie);
        return veggiesToRemove;
    }

    public void Remove3x3(int startX, int startY)
    {
        RemoveVeggiesInDirection(startX, startY, 1, 0); // Horizontal
        RemoveVeggiesInDirection(startX, startY, 0, 1); // Vertical
        RemoveVeggiesInDirection(startX, startY, 1, 1); // Diagonal
        RemoveVeggiesInDirection(startX, startY, 1, -1); // Anti-Diagonal
    }

    private void RemoveVeggiesInDirection(int startX, int startY, int deltaX, int deltaY)
    {
        // Loop through the specified direction
        for (int i = -1; i <= 1; i++)
        {
            int x = startX + i * deltaX;
            int y = startY + i * deltaY;

            if (BoardUtils.IsWithinBounds(x, y) && NodeLayout[x, y].LinkedAction != null)
            {
                Veggie veggieToRemove = NodeLayout[x, y].LinkedAction.GetComponent<Veggie>();
                NodeActionToDestroy.Add(veggieToRemove);

                veggieToRemove.isMatched = true;
                Destroy(NodeLayout[x, y].LinkedAction);
                NodeLayout[x, y] = new(true, null);
            }
        }
    }
}

