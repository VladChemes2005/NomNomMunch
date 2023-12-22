using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

public class VeggieBoard : MonoBehaviour
{
    public int FindByElement(VeggieType[] array, VeggieType veggie)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == veggie)
            {
                return i;
            }
        }
        return -1;
    }

    //defining board size
    public int width = 7;
    public int height = 7;
    
    // dynamic spacing to the board
    public float spacingX;
    public float spacingY;
    //getting a ref to veggies prefabs
    public GameObject[] veggiesPrefabs;
    public List<GameObject> veggiesToDestroy = new();
    public GameObject veggieParent;
    // getting a ref collection nodes Board+GameObjs
    public Node[,] veggieBoard;
    public GameObject veggieBoardGO;
    
    //tiles underneath nodes
    public GameObject tilePrefab;
    public List<GameObject> tilesToDestroy = new();
    public GameObject tileParent;

    //flipTiles
    public bool IsFlipMap;
    public GameObject flipTilePrefab;
    public List<GameObject> flipTilesToDestroy = new();
    public GameObject flipTileParent;


    public float energyLevel = 0f;
    public float maxEnergy = 500f;
    
    [SerializeField]
    public Veggie selectedVeggie;

    [SerializeField]
    private bool isProcessingMove;

    [SerializeField]
    public List<Veggie> veggiesToRemove = new();

    public GameManager gameManager;
    public HintManager hintManager;

    public ButtonClickHandler buttonClickHandler;

    //public Texture2D customCursor;

    //layoutArray
    public ArrayLayout arrayLayout;


    //Ice
    public ArrayLayout iceArrayLayout;
    public GameObject icePrefab;
    public List<GameObject> iceToDestroy = new();
    public List<GameObject> iceToRemove = new();
    public GameObject iceParent;


    //public static of board
    public static VeggieBoard Instance;

    public int streak = 1;
    public int pieceValue = 200;


    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        hintManager = FindObjectOfType<HintManager>();
        buttonClickHandler = FindObjectOfType<ButtonClickHandler>();
        InitializeBoard();
    }

    public void InitializeBoard()
    {
        DestroyVeggies();
        DestroyBGTiles();
        DestroyFlipTiles();
        DestroyIces();

        veggieBoard = new Node[width, height];

        spacingX = (float)(width - 1) / 2 - 2;
        spacingY = (float)((height - 1) / 2 + 0.5);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x - spacingX, y - spacingY);
                if (arrayLayout.rows[y].row[x])
                {
                    veggieBoard[x, y] = new Node(false, null);
                }
                else
                {
                    if (iceArrayLayout.rows[y].row[x])
                    {
                        // Initialize ice element
                        GameObject ice = Instantiate(icePrefab, position, Quaternion.identity);
                        ice.transform.SetParent(iceParent.transform);
                        ice.GetComponent<Ice>().SetIndicies(x, y);
                        veggieBoard[x, y] = new Node(false, ice);
                        iceToDestroy.Add(ice);
                    }
                    if (IsFlipMap)
                    {
                        GameObject flipTile = Instantiate(flipTilePrefab, position, Quaternion.identity);
                        flipTile.transform.SetParent(flipTileParent.transform);
                        flipTile.GetComponent<FlipTile>().SetIndicies(x, y);
                        veggieBoard[x, y] = new Node(false, flipTile);
                        flipTilesToDestroy.Add(flipTile);
                    }
                    int randomIndex = Random.Range(0, veggiesPrefabs.Length);
                    
                    GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
                    tile.transform.SetParent(tileParent.transform);
                    veggieBoard[x, y] = new Node(false, tile);

                    GameObject veggie = Instantiate(veggiesPrefabs[randomIndex], position, Quaternion.identity);
                    veggie.transform.SetParent(veggieParent.transform);
                    veggie.GetComponent<Veggie>().SetIndicies(x, y);
                    veggieBoard[x, y] = new Node(true, veggie);

                    tilesToDestroy.Add(tile);
                    veggiesToDestroy.Add(veggie);
                }
            }
        }
        if (CheckBoard())
        {
            //Debug.Log("We have matches let's re-create the board");
            InitializeBoard();
        }
        else
        {
            //Debug.Log("There are no matches, it's time to start the game!");
        }
    }

    void Update()
    {
        if(gameManager.IsGameEnded)
        {
            enabled = false;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                if(hit.collider.gameObject.GetComponent<Veggie>() && buttonClickHandler.bombType == BombType.None)
                {
                    if (isProcessingMove)
                    {
                        return;
                    }
                    
                    Veggie veggie = hit.collider.gameObject.GetComponent<Veggie>();
                    //Debug.Log($"I have a clicked a veggie it is: a {veggie.gameObject}");
                    //veggie.GetComponent<Renderer>().material.color = Color.yellow;

                    SelectVeggie(veggie);
                    //veggie.GetComponent<Renderer>().material.color = Color.white;

                }
                else if (hit.collider.gameObject.GetComponent<UtilsButton>())
                {
                    buttonClickHandler.bombType = hit.collider.gameObject.GetComponent<UtilsButton>().bombType;
                    Debug.Log("Clicked a button");
                    Cursor.SetCursor(hit.collider.gameObject.GetComponent<UtilsButton>().customCursor, Vector2.zero, CursorMode.Auto);
                }
                else if (hit.collider.gameObject.GetComponent<Veggie>() && buttonClickHandler.bombType != BombType.None)
                {
                    Veggie veggie = hit.collider.gameObject.GetComponent<Veggie>();
                    selectedVeggie = veggie;
                    buttonClickHandler.ExecuteRemoveSelectedVeggie();
                    selectedVeggie = null;
                }


            }

            if (hintManager.currentHint != null)
            {
                hintManager.DestroyHint();
            }
        }
    }




    public bool CheckBoard()
    {
        if (gameManager.IsGameEnded)
        {
            return false;
        }
        //Debug.Log("Checking Board");
        bool hasMatched = false;

        veggiesToRemove.Clear();

        foreach (Node nodeVeggie in veggieBoard)
        {
            if (nodeVeggie.veggie != null)
            {
                nodeVeggie.veggie.GetComponent<Veggie>().isMatched = false;
            }
        }


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //checking if position node is usable
                if (veggieBoard[x, y].isUsable)
                {
                    //then proceed to get potion class in node
                    Veggie veggie = veggieBoard[x, y].veggie.GetComponent<Veggie>();
                    

                    //ensure its not matched
                    if (!veggie.isMatched)
                    {
                        //run some matching logic

                        MatchResult matchedVeggies = IsConnected(veggie);

                        if (matchedVeggies.connectedVeggies.Count >= 3)
                        {
                            MatchResult superMatchedVeggies = SuperMatch(matchedVeggies);
                            veggiesToRemove.AddRange(superMatchedVeggies.connectedVeggies);

                            foreach (Veggie veg in superMatchedVeggies.connectedVeggies)
                                veg.isMatched = true;

                            hasMatched = true;
                        }
                    }
                }
            }
        }
        return hasMatched;
    }

    public bool InvisibleCheck()
    {
        if (gameManager.IsGameEnded)
        {
            return false;
        }
        //Debug.Log("Checking Board");
        bool hasMatched = false;

        veggiesToRemove.Clear();

        foreach (Node nodeVeggie in veggieBoard)
        {
            if (nodeVeggie.veggie != null)
            {
                nodeVeggie.veggie.GetComponent<Veggie>().isMatched = false;
            }
        }


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //checking if position node is usable
                if (veggieBoard[x, y].isUsable)
                {
                    //then proceed to get potion class in node
                    Veggie veggie = veggieBoard[x, y].veggie.GetComponent<Veggie>();


                    //ensure its not matched
                    if (!veggie.isMatched)
                    {
                        //run some matching logic

                        MatchResult matchedVeggies = IsConnected(veggie);

                        if (matchedVeggies.connectedVeggies.Count >= 3)
                        {
                            MatchResult superMatchedVeggies = SuperMatch(matchedVeggies);
                            hasMatched = true;
                        }
                    }
                }
            }
        }
        return hasMatched;
    }

    public IEnumerator ProcessTurnOnMatchesBoard(bool substractMoves)
    {
        foreach (Veggie veggieToRemove in veggiesToRemove)
        {
            veggieToRemove.isMatched = false;
        }
        HandleEnergyIncrease();
        
        RemoveAndRefill(veggiesToRemove);
        gameManager.ProcessTurn(veggiesToRemove.Count*pieceValue*streak, substractMoves);
        yield return new WaitForSeconds(0.4f);
        
        if (CheckBoard())
        {
            streak += 1;
            StartCoroutine(ProcessTurnOnMatchesBoard(false));
        }
        streak = 1;
    }

    #region BOMBA
    public void RemovePlusShape(int startX, int startY)
    {
        // Remove objects horizontally
        for (int i = 0; i < width; i++)
        {
            if (veggieBoard[i, startY].veggie != null)
            {
                Veggie veggieToRemove = veggieBoard[i, startY].veggie.GetComponent<Veggie>();
                veggiesToRemove.Add(veggieToRemove);

                veggieToRemove.isMatched = true;
                Destroy(veggieBoard[i, startY].veggie);
                veggieBoard[i, startY] = new Node(true, null);
            }
        }

        // Remove objects vertically
        for (int j = 0; j < height; j++)
        {
            if (veggieBoard[startX, j].veggie != null)
            {
                Veggie veggieToRemove = veggieBoard[startX, j].veggie.GetComponent<Veggie>();
                veggiesToRemove.Add(veggieToRemove);

                veggieToRemove.isMatched = true;
                Destroy(veggieBoard[startX, j].veggie);
                veggieBoard[startX, j] = new Node(true, null);
            }
        }
    }

    public void RemoveSameType(int startX, int startY)
    {
        VeggieType veggieTypeToRemove = veggieBoard[startX, startY].veggie.GetComponent<Veggie>().veggiesType;

        // Loop through the entire board
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (veggieBoard[i, j].veggie != null &&
                    veggieBoard[i, j].veggie.GetComponent<Veggie>().veggiesType == veggieTypeToRemove)
                {
                    Veggie veggieToRemove = veggieBoard[i, j].veggie.GetComponent<Veggie>();
                    veggiesToRemove.Add(veggieToRemove);

                    veggieToRemove.isMatched = true;
                    Destroy(veggieBoard[i, j].veggie);
                    veggieBoard[i, j] = new Node(true, null);
                }
            }
        }
    }
    
    public List<Veggie> GetVeggiesToRemove()
    {
        return veggiesToRemove;
    }
    
    public void 
        Remove3x3(int startX, int startY)
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

            if (IsWithinBounds(x, y) && veggieBoard[x, y].veggie != null)
            {
                Veggie veggieToRemove = veggieBoard[x, y].veggie.GetComponent<Veggie>();
                veggiesToRemove.Add(veggieToRemove);

                veggieToRemove.isMatched = true;
                Destroy(veggieBoard[x, y].veggie);
                veggieBoard[x, y] = new Node(true, null);
            }
        }
    }

    private bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }


    #endregion
    
    #region Energy
    public void HandleEnergyIncrease()
    {
        List<Veggie> veggiesToRemove = GetVeggiesToRemove();

        int matchCount = veggiesToRemove.Count;

        float amount = (matchCount >= 4) ? 5 * matchCount : 0;

        IncreaseEnergy(amount);
    }
    
    void IncreaseEnergy(float amount)
    {
        energyLevel += amount;
        Debug.Log("Energy Level Increased: " + energyLevel);
        //ensuring the energy level does not exceed the maximum
        energyLevel = Mathf.Clamp(energyLevel, 0f, maxEnergy);
    }
    
    #endregion
    
    #region Cascading Veggies
    //RemoveAndRefill
    private void RemoveAndRefill(List<Veggie> _veggiesToRemove)
    {
        // Create a list to store flipTiles to remove
        List<GameObject> flipTilesToRemove = new List<GameObject>();
        List<GameObject> icesToRemove = new List<GameObject>();

        // Removing the veggie and clearing the board at that location
        foreach (Veggie veggie in _veggiesToRemove)
        {
            //getting it's x and y indicies and storing them
            int _xIndex = veggie.xIndex;
            int _yIndex = veggie.yIndex;

            if (iceArrayLayout.rows[_yIndex].row[_xIndex])
            {
                GameObject Ice = null;
                foreach (GameObject ice in iceToDestroy)
                {
                    Ice iceComponent = ice.GetComponent<Ice>();
                    if (iceComponent.xIndex == _xIndex && iceComponent.yIndex == _yIndex)
                    {
                        Ice = ice;
                        break;
                    }
                }
                if (Ice != null)
                {
                    // Add the flipTile to the list for removal
                    iceToRemove.Add(Ice);
                }
                foreach (GameObject iceToRemove in icesToRemove)
                {
                    iceToDestroy.Remove(iceToRemove);
                    Destroy(iceToRemove);
                }

                iceArrayLayout.rows[_yIndex].row[_xIndex] = false;
            }
            else
            {

                // Check if there is a flipTile at the same position
                GameObject flipTile = null;

                // Iterate through the flipTilesToDestroy list to find the matching flipTile
                foreach (GameObject ft in flipTilesToDestroy)
                {
                    FlipTile flipTileComponent = ft.GetComponent<FlipTile>();
                    if (flipTileComponent.xIndex == _xIndex && flipTileComponent.yIndex == _yIndex)
                    {
                        flipTile = ft;
                        break;
                    }
                }

                if (flipTile != null)
                {
                    // Add the flipTile to the list for removal
                    flipTilesToRemove.Add(flipTile);
                }


                // Goal completion
                var goalIndex = FindByElement(gameManager.goalTile, veggie.veggiesType);
                if (goalIndex != -1)
                {
                    gameManager.goalTileGoals[goalIndex] -= 1;
                }


                //Destroy the veggie
                Destroy(veggie.gameObject);

                //Create a blank node on the veggie board.
                veggieBoard[_xIndex, _yIndex] = new Node(true, null);

                // Remove the flipTiles at the matched positions
                foreach (GameObject flipTileToRemove in flipTilesToRemove)
                {
                    flipTilesToDestroy.Remove(flipTileToRemove);
                    Destroy(flipTileToRemove);
                }

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (veggieBoard[x, y].veggie == null && veggieBoard[x, y].isUsable)
                        {
                            //Debug.Log("The location X: " + x + " Y: " + y + " is empty, attempting to refill it.");
                            RefillVeggie(x, y);
                        }
                    }
                }
            }
        }
    }

    //RemoveAndRefill
    


    //RefillVeggies
    private void RefillVeggie(int x, int y)
    {
        //y offset
        int yOffset = 1;

        //while the cell above our current cell is null and we're below the height of the board
        while (y + yOffset < height && veggieBoard[x, y + yOffset].veggie == null)
        {
            //increment y offset
            //Debug.Log("The veggie above me is null, but i'm not at the top of the board yet, so add to my yOffset and try again. Current Offset is: " + yOffset + " I'm about to add 1.");
            yOffset++;
        }

        //we've either hit the top of the board or we found a veggie

        if (y + yOffset < height && veggieBoard[x, y + yOffset].veggie != null)
        {
            //we`ve found a veggie

            Veggie veggieAbove = veggieBoard[x, y + yOffset].veggie.GetComponent<Veggie>();

            //Move it to the correct location
            Vector3 targetPos = new Vector3(x - spacingX, y - spacingY, veggieAbove.transform.position.z);
            //Debug.Log("I've found a veggie when refilling the board and it was in the location: [" + x + "," + (y + yOffset) + "] we have moved it to the location: [" + x + "," + y + "]");
            //Move to location
            veggieAbove.MoveToTarget(targetPos);
            //update incidices
            veggieAbove.SetIndicies(x, y);
            //update our potionBoard
            veggieBoard[x, y] = veggieBoard[x, y + yOffset];
            //set the location the potion came from to null
            veggieBoard[x, y + yOffset] = new Node(true, null);
        }

        //if we've hit the top of the board without finding a veggie
        if (y + yOffset == height)
        {
            //Debug.Log("I've reached the top of the board without finding a veggie");
            SpawnVeggieAtTop(x);
        }
    }

    //SpawnVeggieAtTop()
    private void SpawnVeggieAtTop(int x)
    {
        int index = FindIndexOfLowestNull(x);
        if(index == 99)
        {
            return;
        }
        int locationToMoveTo = 7 - index;
        //Debug.Log("About to spawn a veggie, ideally i'd like to put it in the index of: " + index);
        int randomIndex = Random.Range(0, veggiesPrefabs.Length);
        GameObject newVeggie = Instantiate(veggiesPrefabs[randomIndex], new Vector2(x - spacingX, height - spacingY), Quaternion.identity);
        newVeggie.transform.SetParent(veggieParent.transform);
        //set indicies
        newVeggie.GetComponent<Veggie>().SetIndicies(x, index);
        //set it on the veggie board
        veggieBoard[x, index] = new Node(true, newVeggie);
        //move it to that location
        Vector3 targetPosition = new Vector3(newVeggie.transform.position.x, newVeggie.transform.position.y - locationToMoveTo, newVeggie.transform.position.z);
        newVeggie.GetComponent<Veggie>().MoveToTarget(targetPosition);
    }

    //FindIndexOfLowestNull(x)
    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = 99;
        for (int y = 6; y >= 0; y--)
        {
            if (veggieBoard[x, y].veggie == null && veggieBoard[x, y].isUsable)
            {
                lowestNull = y;
            }
        }
        return lowestNull;
    }

    #endregion

    private MatchResult SuperMatch(MatchResult _matchedResults)
    {
        //if we have a horizontal or long horizontal match
        if (_matchedResults.direction == MatchDirection.Horizontal || _matchedResults.direction == MatchDirection.LongHorizontal)
        {
            //for each potion...
            foreach (Veggie veg in _matchedResults.connectedVeggies)
            {
                List<Veggie> extraConnectedVeggies = new();
                //check up
                CheckDirection(veg, new Vector2Int(0, 1), extraConnectedVeggies);
                //check down
                CheckDirection(veg, new Vector2Int(0, -1), extraConnectedVeggies);

                //do we have 2 or more veggies that have been matched against this current veggie.
                if (extraConnectedVeggies.Count >=2)
                {
                    //Debug.Log("I have a super Horizontal Match");
                    extraConnectedVeggies.AddRange(_matchedResults.connectedVeggies);

                    //return our super match
                    return new MatchResult
                    {
                        connectedVeggies = extraConnectedVeggies,
                        direction = MatchDirection.Super
                    };
                }
            }
            //we didn't have a super match, so return our normal match
            return new MatchResult
            {
                connectedVeggies = _matchedResults.connectedVeggies,
                direction = MatchDirection.Super
            };
        }
        else if (_matchedResults.direction == MatchDirection.Vertical || _matchedResults.direction == MatchDirection.LongVertical)
        {
            //for each veggie...
            foreach (Veggie veg in _matchedResults.connectedVeggies)
            {
                List<Veggie> extraConnectedVeggies = new();
                //check right
                CheckDirection(veg, new Vector2Int(1, 0), extraConnectedVeggies);
                //check left
                CheckDirection(veg, new Vector2Int(-1, 0), extraConnectedVeggies);
                //do we have 2 or more veggies that have been matched against this current veggie.
                if (extraConnectedVeggies.Count >=2)
                {
                    //Debug.Log("I have a super Vertical Match");
                    extraConnectedVeggies.AddRange(_matchedResults.connectedVeggies);
                    //return our super match
                    return new MatchResult
                    {
                        connectedVeggies = extraConnectedVeggies,
                        direction = MatchDirection.Super
                    };
                }
            }
            //we didn't have a super match, so return our normal match
            return new MatchResult
            {
                connectedVeggies = _matchedResults.connectedVeggies,
                direction = MatchDirection.Super
            };
        }
        //this shouldn't be possible, but a null return is required so the method is valid.
        return null;
    }

    //IsConnected
    MatchResult IsConnected(Veggie veggie)
    {
        List<Veggie> connectedVeggies = new();
        VeggieType vegType = veggie.veggiesType;

        connectedVeggies.Add(veggie);

        //check right
        CheckDirection(veggie, new Vector2Int(1, 0), connectedVeggies);
        //check left
        CheckDirection(veggie, new Vector2Int(-1, 0), connectedVeggies);
        //have we made a 3 match? (Horizontal Match)
        if (connectedVeggies.Count == 3)
        {
            //Debug.Log("I have a normal horizontal match, the color of my match is: " + connectedVeggies[0].veggiesType);

            return new MatchResult
            {
                connectedVeggies = connectedVeggies,
                direction = MatchDirection.Horizontal
            };
        }
        //checking for more than 3 (Long horizontal Match)
        else if (connectedVeggies.Count > 3)
        {
            //Debug.Log("I have a Long horizontal match, the color of my match is: " + connectedVeggies[0].veggiesType);

            return new MatchResult
            {
                connectedVeggies = connectedVeggies,
                direction = MatchDirection.LongHorizontal
            };
        }
        //clear out the connectedpotions
        connectedVeggies.Clear();
        //readd our initial potion
        connectedVeggies.Add(veggie);

        //check up
        CheckDirection(veggie, new Vector2Int(0, 1), connectedVeggies);
        //check down
        CheckDirection(veggie, new Vector2Int(0, -1), connectedVeggies);

        //have we made a 3 match? (Vertical Match)
        if (connectedVeggies.Count == 3)
        {
            //Debug.Log("I have a normal vertical match, the color of my match is: " + connectedVeggies[0].veggiesType);

            return new MatchResult
            {
                connectedVeggies = connectedVeggies,
                direction = MatchDirection.Vertical
            };
        }
        //checking for more than 3 (Long Vertical Match)
        else if (connectedVeggies.Count > 3)
        {
            //Debug.Log("I have a Long vertical match, the color of my match is: " + connectedVeggies[0].veggiesType);

            return new MatchResult
            {
                connectedVeggies = connectedVeggies,
                direction = MatchDirection.LongVertical
            };
        }
        else
        {
            return new MatchResult
            {
                connectedVeggies = connectedVeggies,
                direction = MatchDirection.None
            };
        }
    }


    //CheckDirection
    void CheckDirection(Veggie veg, Vector2Int direction, List<Veggie> connectedVeggies)
    {
        VeggieType vegType = veg.veggiesType;
        int x = veg.xIndex + direction.x;
        int y = veg.yIndex + direction.y;

        //check that we're within the boundaries of the board
        while (x >= 0 && x < width && y >= 0 && y < height)
        {
            if (veggieBoard[x, y].isUsable)
            {
                Veggie neighbourVeggie = veggieBoard[x, y].veggie.GetComponent<Veggie>();

                //does our potionType Match? it must also not be matched
                if (!neighbourVeggie.isMatched && neighbourVeggie.veggiesType == vegType)
                {
                    connectedVeggies.Add(neighbourVeggie);

                    x += direction.x;
                    y += direction.y;
                }
                else
                {
                    break;
                }

            }
            else
            {
                break;
            }
        }
    }

    #region
    #region DestroyALL
    //destroy veggies
    private void DestroyVeggies()
    {
        if (veggiesToDestroy != null)
        {
            foreach (GameObject veg in veggiesToDestroy)
            {
                Destroy(veg);
            }
            veggiesToDestroy.Clear();
        }
    }
    private void DestroyBGTiles()
    {
        if (tilesToDestroy != null)
        {
            foreach (GameObject tile in tilesToDestroy)
            {
                Destroy(tile);
            }
            tilesToDestroy.Clear();
        }
    }
    private void DestroyFlipTiles()
    {
        if (flipTilesToDestroy != null)
        {
            foreach (GameObject flipTile in flipTilesToDestroy)
            {
                Destroy(flipTile);
            }
            flipTilesToDestroy.Clear();
        }
    }
    private void DestroyIces()
    {
        if (iceToDestroy != null)
        {
            foreach (GameObject ice in iceToDestroy)
            {
                Destroy(ice);
            }
            iceToDestroy.Clear();
        }
    }

    private void RemoveIces()
    {
        if (iceToRemove != null)
        {
            foreach (GameObject ice in iceToRemove)
            {
                Destroy(ice);
            }
            iceToRemove.Clear();
        }
    }
    #endregion

    //select veggies
    public void SelectVeggie(Veggie veg)
    {
        if (selectedVeggie == null)
        {
            Debug.Log(veg);
            selectedVeggie = veg;
            selectedVeggie.GetComponent<Renderer>().material.color = Color.gray;
        }
        else if (selectedVeggie == veg)
        {
            selectedVeggie.GetComponent<Renderer>().material.color = Color.white;
            selectedVeggie = null;
        }
        else if (selectedVeggie != veg)
        {
            SwapVeggies(selectedVeggie, veg);
            selectedVeggie.GetComponent<Renderer>().material.color = Color.white;
            selectedVeggie = null;
        }
    }

    //swap veggies
    private void SwapVeggies(Veggie currentVeg, Veggie targetVeg)
    {
        if (!isAdjacent(currentVeg, targetVeg))
        {
            return;
        }

        DoSwap(currentVeg, targetVeg);

        isProcessingMove = true;

        StartCoroutine(ProcessMatches(currentVeg, targetVeg));
    }

    //do swap veggies
    private void DoSwap(Veggie currentVeg, Veggie targetVeg)
    {
        GameObject temp = veggieBoard[currentVeg.xIndex, currentVeg.yIndex].veggie;

        veggieBoard[currentVeg.xIndex, currentVeg.yIndex].veggie = veggieBoard[targetVeg.xIndex, targetVeg.yIndex].veggie;
        veggieBoard[targetVeg.xIndex, targetVeg.yIndex].veggie = temp;

        int tempXIndex = currentVeg.xIndex;
        int tempYIndex = currentVeg.yIndex;
        currentVeg.xIndex = targetVeg.xIndex;
        currentVeg.yIndex = targetVeg.yIndex;
        targetVeg.xIndex = tempXIndex;
        targetVeg.yIndex = tempYIndex;


        currentVeg.MoveToTarget(veggieBoard[targetVeg.xIndex, targetVeg.yIndex].veggie.transform.position);

        targetVeg.MoveToTarget(veggieBoard[currentVeg.xIndex, currentVeg.yIndex].veggie.transform.position);
    }

    //process matches
    private IEnumerator ProcessMatches(Veggie currentVeg, Veggie targetVeg)
    {
        yield return new WaitForSeconds(0.2f);

        if (CheckBoard())
        {
            StartCoroutine(ProcessTurnOnMatchesBoard(true));
        }
        else
        {
            DoSwap(currentVeg, targetVeg);
        }
        isProcessingMove = false;
    }

    // is adjacent
    private bool isAdjacent(Veggie currentVeg, Veggie targetVeg)
    {
        return Mathf.Abs(currentVeg.xIndex - targetVeg.xIndex) + Mathf.Abs(currentVeg.yIndex - targetVeg.yIndex) == 1;
    }
    #endregion

    #region

    public void InvisibleSwap(Veggie currentVeg, Veggie targetVeg)
    {
        GameObject temp = veggieBoard[currentVeg.xIndex, currentVeg.yIndex].veggie;

        veggieBoard[currentVeg.xIndex, currentVeg.yIndex].veggie = veggieBoard[targetVeg.xIndex, targetVeg.yIndex].veggie;
        veggieBoard[targetVeg.xIndex, targetVeg.yIndex].veggie = temp;

        int tempXIndex = currentVeg.xIndex;
        int tempYIndex = currentVeg.yIndex;
        currentVeg.xIndex = targetVeg.xIndex;
        currentVeg.yIndex = targetVeg.yIndex;
        targetVeg.xIndex = tempXIndex;
        targetVeg.yIndex = tempYIndex;

    }

    #endregion
}

public class MatchResult
{
    public List<Veggie> connectedVeggies;
    public MatchDirection direction;
}

public enum MatchDirection
{
    Vertical,
    Horizontal,
    LongVertical,
    LongHorizontal,
    Super,
    None
}
