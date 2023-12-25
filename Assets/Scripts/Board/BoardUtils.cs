using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class BoardUtils 
{
    public static int FindByElement(VeggieType[] array, VeggieType veggie)
    {
        for (int i = 0; i < array.Length; i++)
            if (array[i] == veggie)
                return i;
        return -1;
    }
    public static void InitializeBoard()
    {
        BoardDataHandler dataHandler = BoardDataHandler.Instance;
        BoardData data = dataHandler.Data;

        dataHandler.DestroyNodeActions();

        dataHandler.NodeLayout = new Node[data.width, data.height];
        data.position = new Vector2
        (
            (float)(data.width - 1) / 2 - 2,
            (float)((data.height - 1) / 2 + 0.5)
        );

        for (int y = 0; y < data.width; y++)
        for (int x = 0; x < data.height; x++)
        {
            Vector2 position = new Vector2(x,y) - data.position;

            if (data.ArrayLayout.rows[y].row[x])
                dataHandler.NodeLayout[x, y] = new(false, null);
            else
            {
                if (data.BoardLayout.Any(tile => tile.x == x && tile.y == y && tile.TileType == TileType.Ice))
                    dataHandler.InitialisePrefab(data.PrefabPack.IcePrefab, position, x, y, false, true);

                if (GameManager.Instance.Data.Requirements.IsFlipMap)
                    dataHandler.InitialisePrefab(data.PrefabPack.FlipTilePrefab, position, x, y, false, true);

                int randomIndex = Random.Range(0, data.VeggiesPrefabs.Length);

                if (data.BoardLayout.Any(tile => tile.x == x && tile.y == y && tile.TileType == TileType.Bug))
                    dataHandler.InitialisePrefab(data.PrefabPack.BugPrefab, position, x ,y, false, true);
                else
                    dataHandler.InitialisePrefab(data.VeggiesPrefabs[randomIndex], position, x, y, true, true);

                dataHandler.TilesToDestroy.Add(dataHandler.InitialisePrefabNonActive(data.PrefabPack.TilePrefab, position));
            }
        }

      //  if (CheckBoard(false))
            //InitializeBoard();
    }
    public static bool CheckBoard(bool isInvisible)
    {
        BoardDataHandler dataHandler = BoardDataHandler.Instance;
        BoardData data = dataHandler.Data;

        if (GameManager.Instance.Data.IsGameEnded)
            return false;

        bool hasMatched = false;

        for (int i = 0; i < dataHandler.NodeActionToDestroy.Count; i++)
            if (dataHandler.NodeActionToDestroy[i] is Veggie)
                dataHandler.NodeActionToDestroy.RemoveAt(i);

        foreach (Node node in dataHandler.NodeLayout)
            if (node.LinkedAction is not null && node.LinkedAction is Veggie veggie)
                veggie.isMatched = false;

        for (int x = 0; x < data.width; x++)
        for (int y = 0; y < data.height; y++)
        {
            //checking if position node is usable
            if (!dataHandler.NodeLayout[x, y].IsUsable)
                continue;

            //then proceed to get potion class in node
            Veggie veggie = dataHandler.NodeLayout[x, y].LinkedAction as Veggie;

            if (veggie.isMatched)
                continue;

            //run some matching logic
            MatchResult matchedVeggies = IsConnected(veggie);

            if (matchedVeggies.connectedVeggies.Count < 3)
                continue;

            MatchResult superMatchedVeggies = SuperMatch(matchedVeggies);

            if (!isInvisible)
            {
                dataHandler.NodeActionToDestroy.AddRange(superMatchedVeggies.connectedVeggies);

                foreach (Veggie veg in superMatchedVeggies.connectedVeggies)
                    veg.isMatched = true;
            }

            hasMatched = true;
        }
        return hasMatched;
    }
    public static void HandleEnergyIncrease()
    {
        List<Veggie> veggiesToRemove = BoardDataHandler.Instance.GetVeggiesToRemove();

        int matchCount = veggiesToRemove.Count;

        float amount = (matchCount >= 4) ? 5 * matchCount : 0;

        IncreaseEnergy(amount);
    }
    public static void IncreaseEnergy(float amount)
    {
        BoardCycle.Energy += amount;
        Debug.Log("Energy Level Increased: " + BoardCycle.Energy);

        //ensuring the energy level does not exceed the maximum
        BoardCycle.Energy = Mathf.Clamp(BoardCycle.Energy, 0f, BoardCycle.MaxEnergy);
    }
    public static int FindIndexOfLowestNull(int x)
    {
        BoardDataHandler dataHandler = BoardDataHandler.Instance;
        BoardData data = dataHandler.Data;

        int lowestNull = 99;
        for (int y = 6; y >= 0; y--)
            if (dataHandler.NodeLayout[x, y].LinkedAction is null && dataHandler.NodeLayout[x, y].IsUsable)
                lowestNull = y;

        return lowestNull;
    }
    public static MatchResult SuperMatch(MatchResult matchedResults)
    {
        //if we have a horizontal or long horizontal match
        if (matchedResults.direction == MatchDirection.Horizontal || 
            matchedResults.direction == MatchDirection.LongHorizontal)
        {
            foreach (Veggie veg in matchedResults.connectedVeggies)
            {
                List<Veggie> extraConnectedVeggies = new();
                //check up
                CheckDirection(veg, new Vector2Int(0, 1), extraConnectedVeggies);
                //check down
                CheckDirection(veg, new Vector2Int(0, -1), extraConnectedVeggies);

                //do we have 2 or more veggies that have been matched against this current veggie.
                if (extraConnectedVeggies.Count < 2)
                    continue;
                
                extraConnectedVeggies.AddRange(matchedResults.connectedVeggies);

                //return our super match
                return new MatchResult
                {
                    connectedVeggies = extraConnectedVeggies,
                    direction = MatchDirection.Super
                };
            }

            //we didn't have a super match, so return our normal match
            return new MatchResult
            {
                connectedVeggies = matchedResults.connectedVeggies,
                direction = MatchDirection.Super
            };
        }
        else if (matchedResults.direction == MatchDirection.Vertical || 
                 matchedResults.direction == MatchDirection.LongVertical)
        {
            foreach (Veggie veg in matchedResults.connectedVeggies)
            {
                List<Veggie> extraConnectedVeggies = new();
                //check right
                CheckDirection(veg, new Vector2Int(1, 0), extraConnectedVeggies);
                //check left
                CheckDirection(veg, new Vector2Int(-1, 0), extraConnectedVeggies);
                //do we have 2 or more veggies that have been matched against this current veggie.
                if (extraConnectedVeggies.Count < 2)
                    continue;
    
                extraConnectedVeggies.AddRange(matchedResults.connectedVeggies);
                //return our super match
                return new MatchResult
                {
                    connectedVeggies = extraConnectedVeggies,
                    direction = MatchDirection.Super
                };
            }

            //we didn't have a super match, so return our normal match
            return new MatchResult
            {
                connectedVeggies = matchedResults.connectedVeggies,
                direction = MatchDirection.Super
            };
        }

        //this shouldn't be possible, but a null return is required so the method is valid.
        return null;
    }
    public static MatchResult IsConnected(Veggie veggie)
    {
        List<Veggie> connectedVeggies = new();

        connectedVeggies.Add(veggie);

        CheckDirection(veggie, new Vector2Int(1, 0), connectedVeggies);
        CheckDirection(veggie, new Vector2Int(-1, 0), connectedVeggies);

        //have we made a 3 match? (Horizontal Match)
        if (connectedVeggies.Count == 3) return
        new MatchResult
        {
            connectedVeggies = connectedVeggies,
            direction = MatchDirection.Horizontal
        };

        //checking for more than 3 (Long horizontal Match)
        else if (connectedVeggies.Count > 3) return 
        new MatchResult
        {
            connectedVeggies = connectedVeggies,
            direction = MatchDirection.LongHorizontal
        };

        //clear out the connectedpotions
        connectedVeggies.Clear();
        //readd our initial potion
        connectedVeggies.Add(veggie);

        CheckDirection(veggie, new Vector2Int(0, 1), connectedVeggies);
        CheckDirection(veggie, new Vector2Int(0, -1), connectedVeggies);

        //have we made a 3 match? (Vertical Match)
        if (connectedVeggies.Count == 3) return
        new MatchResult
        {
            connectedVeggies = connectedVeggies,
            direction = MatchDirection.Vertical
        };

        //checking for more than 3 (Long Vertical Match)
        else if (connectedVeggies.Count > 3) return 
        new MatchResult
        {
            connectedVeggies = connectedVeggies,
            direction = MatchDirection.LongVertical
        };

        else return 
        new MatchResult
        {
            connectedVeggies = connectedVeggies,
            direction = MatchDirection.None
        };
    }
    private static void CheckDirection(Veggie veg, Vector2Int direction, List<Veggie> connectedVeggies)
    {
        BoardDataHandler dataHandler = BoardDataHandler.Instance;
        BoardData data = dataHandler.Data;

        VeggieType vegType = veg.veggiesType;
        int x = veg.xIndex + direction.x;
        int y = veg.yIndex + direction.y;

        //check that we're within the boundaries of the board
        while (x >= 0 && x < data.width && y >= 0 && y < data.height)
        {
            if (!dataHandler.NodeLayout[x, y].IsUsable)
                break;

            Veggie neighbourVeggie = dataHandler.NodeLayout[x, y].LinkedAction as Veggie;

            //does our potionType Match? it must also not be matched
            if (neighbourVeggie.isMatched || neighbourVeggie.veggiesType != vegType || vegType == VeggieType.bug)
                break;

            connectedVeggies.Add(neighbourVeggie);

            x += direction.x;
            y += direction.y;
        }
    }
    public static void SelectVeggie(Veggie veg)
    {
        if (BoardCycle.SelectedVeggie is null)
        {
            BoardCycle.SelectedVeggie = veg;
            return;
        }

        if (BoardCycle.SelectedVeggie != veg)
            BoardDataHandler.Instance.SwapVeggies(BoardCycle.SelectedVeggie, veg);

        BoardCycle.SelectedVeggie = null;
    }
    public static void DoSwap(Veggie currentVeg, Veggie targetVeg)
    {
        Debug.Log("do swap");
        BoardDataHandler dataHandler = BoardDataHandler.Instance;

        InvisibleSwap(currentVeg, targetVeg);

        currentVeg.MoveToTarget(dataHandler.NodeLayout[targetVeg.xIndex, targetVeg.yIndex].LinkedAction.transform.position);
        targetVeg.MoveToTarget(dataHandler.NodeLayout[currentVeg.xIndex, currentVeg.yIndex].LinkedAction.transform.position);
    }
    public static void InvisibleSwap(Veggie currentVeg, Veggie targetVeg)
    {
        BoardDataHandler dataHandler = BoardDataHandler.Instance;

        NodeAction temp = dataHandler.NodeLayout[currentVeg.xIndex, currentVeg.yIndex].LinkedAction;

        dataHandler.NodeLayout[currentVeg.xIndex, currentVeg.yIndex].LinkedAction = dataHandler.NodeLayout[targetVeg.xIndex, targetVeg.yIndex].LinkedAction;
        dataHandler.NodeLayout[targetVeg.xIndex, targetVeg.yIndex].LinkedAction = temp;

        int tempXIndex = currentVeg.xIndex;
        int tempYIndex = currentVeg.yIndex;
        currentVeg.xIndex = targetVeg.xIndex;
        currentVeg.yIndex = targetVeg.yIndex;
        targetVeg.xIndex = tempXIndex;
        targetVeg.yIndex = tempYIndex;
    }
    public static bool IsAdjacent(Veggie currentVeg, Veggie targetVeg) 
        => Mathf.Abs(currentVeg.xIndex - targetVeg.xIndex) + Mathf.Abs(currentVeg.yIndex - targetVeg.yIndex) == 1;
    public static bool IsWithinBounds(int x, int y)
    {
        BoardData data = BoardDataHandler.Instance.Data;
        return x >= 0 && x < data.width && y >= 0 && y < data.height;
    }
}

