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
    }
}
