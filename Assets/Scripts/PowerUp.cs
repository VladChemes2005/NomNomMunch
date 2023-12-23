using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class ButtonClickHandler : MonoBehaviour//, IPointerClickHandler
{
    public BombType bombType = BombType.None;
    public VeggieBoard veggieBoard;
    //public Texture2D customCursor;
    //private bool isButtonClicked = false;

    /*public bool IsButtonClicked => isButtonClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        // Change cursor on button click
        Cursor.SetCursor(customCursor, Vector2.zero, CursorMode.Auto);

        // Set the flag to true when the button is clicked
        isButtonClicked = true;
        //bombType = lastClickedButton.bombType;

        Debug.Log("Button Clicked!");
    }*/

    // Public method to be called from other parts of your code
    
    public void ExecuteRemoveSelectedVeggie()
    {
        if (veggieBoard != null)
        {
            // Call the private method to remove the selected veggie
            RemoveSelectedVeggie();
        }
        else
        {
            Debug.LogError("VeggieBoard is null!");
        }
    }

    public void RemoveSelectedVeggie()
    {
        if (veggieBoard.selectedVeggie != null)
        {
            Veggie selectedVeggie = veggieBoard.selectedVeggie;

            switch (bombType)
            {
                case BombType.Tpick:
                    if (veggieBoard.energyLevel >= 100)
                    {
                        veggieBoard.veggiesToRemove.Add(selectedVeggie);
                        //Destroy(selectedVeggie.gameObject);
                        veggieBoard.StartCoroutine(veggieBoard.ProcessTurnOnMatchesBoard(true));
                        veggieBoard.energyLevel = 0;
                    }
                    else
                    {
                        Debug.Log("Energy level is not sufficient for Tpick!");
                    }
                    break;
                case BombType.Knifes:
                    if (veggieBoard.energyLevel >= 400)
                    {
                        veggieBoard.RemovePlusShape(selectedVeggie.xIndex, selectedVeggie.yIndex);
                        veggieBoard.StartCoroutine(veggieBoard.ProcessTurnOnMatchesBoard(true));
                        veggieBoard.energyLevel = 0;
                    }
                    else
                    {
                        Debug.Log("Energy level is not sufficient for Knifes!");
                    }
                    break;
                case BombType.Fork:
                    if (veggieBoard.energyLevel >= 300)
                    {
                        veggieBoard.RemoveSameType(selectedVeggie.xIndex, selectedVeggie.yIndex);
                        veggieBoard.StartCoroutine(veggieBoard.ProcessTurnOnMatchesBoard(true));
                        veggieBoard.energyLevel = 0;
                    }
                    else
                    {
                        Debug.Log("Energy level is not sufficient for Fork!");
                    }
                    break;
                case BombType.Spoon:
                    if (veggieBoard.energyLevel >= 200)
                    {
                        veggieBoard.Remove3x3(selectedVeggie.xIndex, selectedVeggie.yIndex);
                        veggieBoard.StartCoroutine(veggieBoard.ProcessTurnOnMatchesBoard(true));
                        veggieBoard.energyLevel = 0;
                    }
                    else
                    {
                        Debug.Log("Energy level is not sufficient for Spoon!");
                    }
                    break; 
            }
            
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            veggieBoard.selectedVeggie = null;
            bombType = BombType.None;
        }
    }
}

public enum BombType
{
    Tpick,
    Knifes,
    Fork,
    Spoon,
    None
}