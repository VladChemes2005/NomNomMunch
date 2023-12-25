using UnityEngine;

public class ButtonClickHandler : MonoBehaviour
{
    public static ButtonClickHandler Instance;

    public BombType bombType = BombType.None;

    private void Awake() => Instance = this;
    public void ExecuteRemoveSelectedVeggie()
    {
        RemoveSelectedVeggie();
    }
    public void RemoveSelectedVeggie()
    {
        BoardDataHandler dataHandler = BoardDataHandler.Instance;
        BoardData data = dataHandler.Data;
        if (BoardCycle.SelectedVeggie != null)
        {
            Veggie selectedVeggie = BoardCycle.SelectedVeggie;

            switch (bombType)
            {
                case BombType.Tpick:
                    if (BoardCycle.Energy >= 100)
                    {
                        dataHandler.NodeActionToDestroy.Add(selectedVeggie);
                        StartCoroutine(dataHandler.ProcessTurnOnMatchesBoard(true));
                        BoardCycle.Energy = 0;
                    }
                    else
                    {
                        Debug.Log("Energy level is not sufficient for Tpick!");
                    }
                    break;
                case BombType.Knifes:
                    if (BoardCycle.Energy >= 400)
                    {
                        dataHandler.RemovePlusShape(selectedVeggie.xIndex, selectedVeggie.yIndex);
                        StartCoroutine(dataHandler.ProcessTurnOnMatchesBoard(true));
                        BoardCycle.Energy = 0;
                    }
                    else
                    {
                        Debug.Log("Energy level is not sufficient for Knifes!");
                    }
                    break;
                case BombType.Fork:
                    if (BoardCycle.Energy >= 300)
                    {
                        dataHandler.RemoveSameType(selectedVeggie.xIndex, selectedVeggie.yIndex);
                        StartCoroutine(dataHandler.ProcessTurnOnMatchesBoard(true));
                        BoardCycle.Energy = 0;
                    }
                    else
                    {
                        Debug.Log("Energy level is not sufficient for Fork!");
                    }
                    break;
                case BombType.Spoon:
                    if (BoardCycle.Energy >= 200)
                    {
                        dataHandler.Remove3x3(selectedVeggie.xIndex, selectedVeggie.yIndex);
                        StartCoroutine(dataHandler.ProcessTurnOnMatchesBoard(true));
                        BoardCycle.Energy = 0;
                    }
                    else
                    {
                        Debug.Log("Energy level is not sufficient for Spoon!");
                    }
                    break; 
            }
            
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            BoardCycle.SelectedVeggie = null;
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