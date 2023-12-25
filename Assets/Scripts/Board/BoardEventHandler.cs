using System;
using System.Linq;
using UnityEngine;
public class BoardEventHandler : MonoBehaviour 
{
    public delegate void OnClearBoardHandler();
    public delegate void OnTileActionHandler(Tile tile);
    public delegate bool OnValidateHandler();
    public delegate void OnSetStringValueHandler(string value);
    public delegate string OnSendStringHandler();
    public delegate void OnSetValueHandler(float value);
    public delegate void OnModifyValueHandler(float value);
    public delegate float OnSendValueHandler();

    public delegate void MouseClickEventHandler(RaycastHit2D hit);
    public delegate void GameEndedEventHandler();

    public static OnClearBoardHandler OnClearBoard;
    public static OnValidateHandler OnGetMatch;
    public static OnSendValueHandler OnGetMaxEnergy;
    public static OnModifyValueHandler OnModifyEnergy;
    public static OnSendValueHandler OnSendEnergy;
    public static OnModifyValueHandler OnModifyStreak;
    

    public static event MouseClickEventHandler OnMouseClick;
    public static event GameEndedEventHandler OnGameEnded;
    public static event OnValidateHandler OnIsProcessingMove;

    public static void InvokeMouseClickEvent(RaycastHit2D hit) => OnMouseClick?.Invoke(hit);
    public static void InvokeGameEndedEvent() => OnGameEnded?.Invoke();

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            InvokeMouseClickEvent(hit);
        }
    }

    private void OnEnable()
    {
        OnMouseClick += HandleMouseClick;
        OnGameEnded += HandleGameEnded;
    }

    private void OnDisable()
    {
        OnMouseClick -= HandleMouseClick;
        OnGameEnded -= HandleGameEnded;
    }
    private void HandleMouseClick(RaycastHit2D hit)
    {
        SceneData data = GameManager.Instance.Data;
        if (data.IsGameEnded)
            enabled = false;

        ProcessMouseInput(hit);
        if (HintManager.Instance.currentHint != null)
            HintManager.Instance.DestroyHint();
    }

    private void HandleGameEnded() => enabled = false;

    private void ProcessMouseInput(RaycastHit2D hit)
    {
        Debug.Log("Click");
        hit = GetRayHit();
        if (hit.collider == null)
            return;
        if
        (
        hit.collider.gameObject.GetComponent<Veggie>() &&
        ButtonClickHandler.Instance.bombType == BombType.None &&
        hit.collider.gameObject.GetComponent<Veggie>() !=
        BoardDataHandler.Instance.Data.BoardLayout.Any
            (
            tile => tile.x == hit.collider.gameObject.GetComponent<Veggie>().xIndex &&
            tile.y == hit.collider.gameObject.GetComponent<Veggie>().yIndex &&
            tile.TileType == TileType.Ice
            )
        )
        {
            if (OnIsProcessingMove())
                return;
            Veggie veggie = hit.collider.gameObject.GetComponent<Veggie>();
            SelectVeggie(veggie);
        }
        else if (hit.collider.gameObject.GetComponent<UtilsButton>())
        {
            ButtonClickHandler.Instance.bombType = hit.collider.gameObject.GetComponent<UtilsButton>().bombType;
            Debug.Log("Clicked a button");
        }
        else if (hit.collider.gameObject.GetComponent<Veggie>() && ButtonClickHandler.Instance.bombType != BombType.None)
        {
            Veggie veggie = hit.collider.gameObject.GetComponent<Veggie>();
            BoardCycle.SelectedVeggie = veggie;
            ButtonClickHandler.Instance.ExecuteRemoveSelectedVeggie();
            //BoardCycle.SelectedVeggie = null;
        }
    }
    private static RaycastHit2D GetRayHit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        return hit;
    }
    public void SelectVeggie(Veggie veg)
    {
        if (BoardCycle.SelectedVeggie == null)
        {
            Debug.Log(BoardCycle.SelectedVeggie +" " +veg);
            BoardCycle.SelectedVeggie = veg;
        }
        else if (BoardCycle.SelectedVeggie == veg)
        {
            BoardCycle.SelectedVeggie = null;
        }
        else if (BoardCycle.SelectedVeggie != veg)
        {
            BoardDataHandler.Instance.SwapVeggies(BoardCycle.SelectedVeggie, veg);
            BoardCycle.SelectedVeggie = null;
        }
    }

}