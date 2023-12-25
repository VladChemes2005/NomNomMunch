using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "GameManagerData", menuName = "Data/SceneData")]
public class SceneData : ScriptableObject
{
    [Header("EndGame Requirements")]
    public EndGameRequirements Requirements = new();

    [Space(15)]

    [Header("Progress Counters")]
    public float TimerSeconds;
    public int Points = 0;
    public int CurrentCounterValue;
    [Space(5)]
    public bool IsGameEnded = false;

    [HideInInspector]
    public VeggieType[] GoalTile;

    [HideInInspector]
    public int[] GoalTileGoals;

    [Space(15)]

    [Header("Prefabs (Labels)")]
    public GameObject MovesLabel;
    public GameObject TimeLabel;

    [Space(5)]

    [Header("Prefabs (Goal)")]
    public GameObject GoalPrefab;
    [HideInInspector]
    public Transform GoalIntroParent;
    [HideInInspector]
    public Transform GoalGameParent;

    [Space(5)]

    [Header("Prefabs (Text)")]
    public Text Counter;
    public Text Score;

    private void OnValidate()
    {
        GoalTile = Requirements.GoalTile.Clone() as VeggieType[];
        GoalTileGoals = Requirements.GoalTileGoals.Clone() as int[];
    }

    public object Clone()
    {
        SceneData clonedData = new();

        clonedData.Requirements = Requirements;
        clonedData.TimerSeconds = TimerSeconds;
        clonedData.Points = Points;
        clonedData.CurrentCounterValue = CurrentCounterValue;
        clonedData.IsGameEnded = IsGameEnded;
        clonedData.GoalTile = (VeggieType[])GoalTile.Clone();
        clonedData.GoalTileGoals = (int[])GoalTileGoals.Clone();
        clonedData.MovesLabel = MovesLabel;
        clonedData.TimeLabel = TimeLabel;
        clonedData.GoalPrefab = GoalPrefab;
        clonedData.GoalIntroParent = GoalIntroParent;
        clonedData.GoalGameParent = GoalGameParent;
        clonedData.Counter = Counter;
        clonedData.Score = Score;

        return clonedData;
    }
}