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

            // Видалення об'єкта в залежності від bombType
            switch (bombType)
            {
                case BombType.Tpick:
                    veggieBoard.veggiesToRemove.Add(selectedVeggie);
                    veggieBoard.StartCoroutine(veggieBoard.ProcessTurnOnMatchesBoard(true));
                    Destroy(selectedVeggie.gameObject);
                    break;
                case BombType.Knifes:
                    veggieBoard.RemovePlusShape(selectedVeggie.xIndex, selectedVeggie.yIndex);
                    veggieBoard.StartCoroutine(veggieBoard.ProcessTurnOnMatchesBoard(true));
                    break;
                case BombType.Fork:
                    veggieBoard.RemoveSameType(selectedVeggie.xIndex, selectedVeggie.yIndex);
                    veggieBoard.StartCoroutine(veggieBoard.ProcessTurnOnMatchesBoard(true));

                    break;
                case BombType.Spoon:
                    veggieBoard.Remove3x3(selectedVeggie.xIndex, selectedVeggie.yIndex);
                    veggieBoard.StartCoroutine(veggieBoard.ProcessTurnOnMatchesBoard(true));
                    break;
            }

            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            veggieBoard.selectedVeggie = null;
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