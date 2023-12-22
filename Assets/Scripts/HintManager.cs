using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    private VeggieBoard board;
    public float hintDelay;
    private float hintDelaySeconds;
    public GameObject hintParticle;
    public GameObject currentHint;
    public List<GameObject> possibleMoves = new List<GameObject>();
    public GameObject move;

    // Start is called before the first frame update
    void Start()
    {
       board = FindObjectOfType<VeggieBoard>();
       hintDelaySeconds = hintDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if(board.gameManager.IsGameEnded)
        {
            enabled = false;
            
        }
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
    
    
    bool Deadlocked()
    {
        possibleMoves.Clear();
        for(int x = 1; x < board.width-1; x++)
        { 
            for (int y = 1; y < board.height-1; y++)
            {
                if (board.veggieBoard[x, y].isUsable && board.veggieBoard[x, y].veggie != null)
                {
                    if(board.veggieBoard[x, y + 1].isUsable && board.veggieBoard[x, y + 1].veggie != null){
                        /*Debug.Log($"{i} {y}");
                        Debug.Log($"{i} {y + 1}");*/
                        board.InvisibleSwap(board.veggieBoard[x, y].veggie.GetComponent<Veggie>(), board.veggieBoard[x, y + 1].veggie.GetComponent<Veggie>());
                        /*Debug.Log($"{i} {y}");
                        Debug.Log($"{i} {y + 1}");*/
                        var boardValid = board.InvisibleCheck();
                        board.InvisibleSwap(board.veggieBoard[x, y + 1].veggie.GetComponent<Veggie>(), board.veggieBoard[x, y].veggie.GetComponent<Veggie>());

                        if (boardValid)
                        {
                            possibleMoves.Add(board.veggieBoard[x, y].veggie);
                            //Debug.Log($"{x} {y}");
                            return false;
                        }
                        
                    }

                    if (board.veggieBoard[x + 1, y].isUsable && board.veggieBoard[x + 1, y].veggie != null)
                    {
                        board.InvisibleSwap(board.veggieBoard[x, y].veggie.GetComponent<Veggie>(), board.veggieBoard[x + 1, y].veggie.GetComponent<Veggie>());

                        var boardValid = board.InvisibleCheck();
                        board.InvisibleSwap(board.veggieBoard[x + 1, y].veggie.GetComponent<Veggie>(), board.veggieBoard[x, y].veggie.GetComponent<Veggie>());

                        if (boardValid)
                        {
                            possibleMoves.Add(board.veggieBoard[x, y].veggie);
                            //Debug.Log($"{x} {y}");
                            return false;
                        }
                    }

                    if (board.veggieBoard[x - 1, y].isUsable && board.veggieBoard[x - 1, y].veggie != null)
                    {
                        /*Debug.Log($"{i} {y}");
                        Debug.Log($"{i} {y + 1}");*/
                        board.InvisibleSwap(board.veggieBoard[x, y].veggie.GetComponent<Veggie>(), board.veggieBoard[x - 1, y].veggie.GetComponent<Veggie>());
                        /*Debug.Log($"{i} {y}");
                        Debug.Log($"{i} {y + 1}");*/
                        var boardValid = board.InvisibleCheck();
                        board.InvisibleSwap(board.veggieBoard[x - 1, y].veggie.GetComponent<Veggie>(), board.veggieBoard[x, y].veggie.GetComponent<Veggie>());

                        if (boardValid)
                        {
                            possibleMoves.Add(board.veggieBoard[x, y].veggie);
                            //Debug.Log($"{x} {y}");
                            return false;
                        }

                    }

                    if (board.veggieBoard[x, y - 1].isUsable && board.veggieBoard[x, y - 1].veggie != null)
                    {
                        /*Debug.Log($"{i} {y}");
                        Debug.Log($"{i} {y + 1}");*/
                        board.InvisibleSwap(board.veggieBoard[x, y].veggie.GetComponent<Veggie>(), board.veggieBoard[x, y - 1].veggie.GetComponent<Veggie>());
                        /*Debug.Log($"{i} {y}");
                        Debug.Log($"{i} {y + 1}");*/
                        var boardValid = board.InvisibleCheck();
                        board.InvisibleSwap(board.veggieBoard[x, y - 1].veggie.GetComponent<Veggie>(), board.veggieBoard[x, y].veggie.GetComponent<Veggie>());

                        if (boardValid)
                        {
                            possibleMoves.Add(board.veggieBoard[x, y].veggie);
                            //Debug.Log($"{x} {y}");
                            return false;
                        }

                    }
                }
                
            }
        }
        return true;
    }

    

    void MarkHint()
    {
        if (!Deadlocked())
        {
            move = possibleMoves[0];
            currentHint = Instantiate(hintParticle, move.transform.position, Quaternion.identity);
            
        }
        else
        {
            board.InitializeBoard();
        }
    }

    public void DestroyHint()
    {
        if(currentHint != null)
        {
            Destroy(currentHint);
            currentHint = null;
            hintDelaySeconds = hintDelay;
        }
    }

}
