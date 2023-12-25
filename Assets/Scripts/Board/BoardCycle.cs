using UnityEngine;

public class BoardCycle : MonoBehaviour
{
    public static float Energy;
    public static float MaxEnergy;

    public static Veggie SelectedVeggie;
    public static bool IsProcessingMove;

    public static int Streak = 1;
    public static int PieceValue = 200;

    private void Awake()
    {
        Energy = 0f;
        MaxEnergy = 500f;
        SelectedVeggie = null;
        IsProcessingMove = false;
        BoardEventHandler.OnIsProcessingMove += GetProccesingMove;
    }
    public bool GetProccesingMove() => IsProcessingMove;

    private void Start()
    {
        BoardUtils.InitializeBoard();
    }
}

