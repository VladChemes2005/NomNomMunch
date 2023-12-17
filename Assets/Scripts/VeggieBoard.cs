using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VeggieBoard : MonoBehaviour
{
    
    //defining board size
    public int width = 7;
    public int height = 7;
    
    // dynamic spacing to the board
    public float spacingX;
    public float spacingY;
    //getting a ref to veggies prefabs
    public GameObject[] veggiesPrefabs;
    // getting a ref collection nodes Board+GameObjs
    public Node[,] veggieBoard;
    public GameObject veggieBoardGO;
    
    //tiles underneath nodes
    public GameObject tilePrefab;
    

    //layoutArray
    public ArrayLayout arrayLayout;
    
    //public static of board
    public static VeggieBoard Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeBoard();
    }

    void InitializeBoard()
    {
        veggieBoard = new Node[width, height];
        
        spacingX = (float)(width - 1) / 2;
        spacingY = (float)((height - 1) / 2) + 1;

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
                    int randomIndex = Random.Range(0, veggiesPrefabs.Length);

                    GameObject veggie = Instantiate(veggiesPrefabs[randomIndex], position, Quaternion.identity);
                    veggie.GetComponent<Veggies>().SetIndicies(x, y);
                    veggieBoard[x, y] = new Node(true, veggie);
                    
                    GameObject Tile = Instantiate(tilePrefab, position, Quaternion.identity);
                    veggieBoard[x, y] = new Node(false, Tile);
                }
            }
        }
        if (CheckBoard())
        {
            Debug.Log("We have matches let's re-create the board");
            InitializeBoard();
        }
        else
        {
            Debug.Log("There are no matches, it's time to start the game!");
        }
    }

    public bool CheckBoard()
    {
        Debug.Log("Checking Board");
        bool hasMatched = false;

        List<Veggies> veggiesToRemove = new();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //checking if position node is usable
                if (veggieBoard[x, y].isUsable)
                {
                    //then proceed to get potion class in node
                    Veggies veggie = veggieBoard[x, y].veg.GetComponent<Veggies>();

                    //ensure its not matched
                    if (!veggie.isMatched)
                    {
                        //run some matching logic

                        MatchResult matchedVeggies = IsConnected(veggie);

                        if (matchedVeggies.connectedVeggies.Count >= 3)
                        {
                            veggiesToRemove.AddRange(matchedVeggies.connectedVeggies);

                            foreach (Veggies veg in matchedVeggies.connectedVeggies)
                                veg.isMatched = true;

                            hasMatched = true;
                        }
                    }
                }
            }
        }

        return hasMatched;
    }


    //IsConnected
    MatchResult IsConnected(Veggies veggie)
    {
        List<Veggies> connectedVeggies = new();
        VegType vegType = veggie.veggiesType;

        connectedVeggies.Add(veggie);

        //check right
        CheckDirection(veggie, new Vector2Int(1, 0), connectedVeggies);
        //check left
        CheckDirection(veggie, new Vector2Int(-1, 0), connectedVeggies);
        //have we made a 3 match? (Horizontal Match)
        if (connectedVeggies.Count == 3)
        {
            Debug.Log("I have a normal horizontal match, the color of my match is: " + connectedVeggies[0].veggiesType);

            return new MatchResult
            {
                connectedVeggies = connectedVeggies,
                direction = MatchDirection.Horizontal
            };
        }
        //checking for more than 3 (Long horizontal Match)
        else if (connectedVeggies.Count > 3)
        {
            Debug.Log("I have a Long horizontal match, the color of my match is: " + connectedVeggies[0].veggiesType);

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
            Debug.Log("I have a normal vertical match, the color of my match is: " + connectedVeggies[0].veggiesType);

            return new MatchResult
            {
                connectedVeggies = connectedVeggies,
                direction = MatchDirection.Vertical
            };
        }
        //checking for more than 3 (Long Vertical Match)
        else if (connectedVeggies.Count > 3)
        {
            Debug.Log("I have a Long vertical match, the color of my match is: " + connectedVeggies[0].veggiesType);

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
    void CheckDirection(Veggies veg, Vector2Int direction, List<Veggies> connectedVeggies)
    {
        VegType vegType = veg.veggiesType;
        int x = veg.xIndex + direction.x;
        int y = veg.yIndex + direction.y;

        //check that we're within the boundaries of the board
        while (x >= 0 && x < width && y >= 0 && y < height)
        {
            if (veggieBoard[x, y].isUsable)
            {
                Veggies neighbourVeggie = veggieBoard[x, y].veg.GetComponent<Veggies>();

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



}

public class MatchResult
{
    public List<Veggies> connectedVeggies;
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
