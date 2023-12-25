using UnityEngine;

[System.Serializable]
public class EndGameRequirements
{
    [Header("Parameters")]
    public GameType GameType;

    [Space(10)]

    public int CounterValue;

    [Space(10)]

    [Header("Goal")]
    public int Goal;
    public VeggieType[] GoalTile;
    public int[] GoalTileGoals;
    public Sprite[] GoalSprite;
    public bool IsFlipMap;
}

