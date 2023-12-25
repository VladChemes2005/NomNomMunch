using UnityEngine;
using UnityEngine.UI;

public class GameManager : DDOLSingleton
{
    public static GameManager Instance;
   
    public SceneData Data;
     protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    private void Start() => Initialize();

    private void Update()
    {
        //Data.Counter.text = $"{Data.CurrentCounterValue}";
        //Data.Score.text = $"{Data.Points}";

        if (Data.Requirements.GameType == GameType.time && Data.CurrentCounterValue > 0)
        {
            Data.TimerSeconds -= Time.deltaTime;
            Data.CurrentCounterValue = (int)Data.TimerSeconds;
        }

        Data.IsGameEnded = CheckGameState();

        if (Data.Points >= Data.Requirements.Goal && Data.IsGameEnded && CheckIfZeroed(Data.GoalTileGoals))
            enabled = false;

        else if (Data.Points < Data.Requirements.Goal && Data.IsGameEnded && (!CheckIfZeroed(Data.GoalTileGoals) || Data.GoalTileGoals.Length == 0))
            enabled = false;
    }

    public void Initialize()
    {
        Data.GoalIntroParent = transform;
        Data.GoalGameParent = transform;

        InitGoals();
        ResetGoals();
        CheckRequirementPredicate();
        VisualiseProgress();
        InstantiateGoals();
    }

    private void InitGoals()
    {
        Data.CurrentCounterValue = Data.Requirements.CounterValue;
        Data.GoalTileGoals = Data.Requirements.GoalTileGoals;
        Data.GoalTileGoals = (int[])Data.GoalTileGoals.Clone();
        Data.GoalTile = Data.Requirements.GoalTile;
        Data.GoalTile = (VeggieType[])Data.GoalTile.Clone();
    }
    private void ResetGoals()
    {
        if (Data.GoalTileGoals.Length > Data.GoalTile.Length)
            for (int i = Data.GoalTile.Length; i < Data.GoalTileGoals.Length; i++)
                Data.GoalTileGoals[i] = 0;
    }
    private void CheckRequirementPredicate()
    {
        bool requirementPredicate = Data.Requirements.GameType != GameType.moves;

        if (requirementPredicate)
            Data.TimerSeconds = Data.CurrentCounterValue;

        //Data.MovesLabel.SetActive(!requirementPredicate);
        //Data.TimeLabel.SetActive(requirementPredicate);
    }
    private void VisualiseProgress()
    {
       // Data.Counter.text = $"{Data.CurrentCounterValue}";
       // Data.Score.text = "0";
    }
    private void InstantiateGoals()
    {
        for (int i = 0; i < Data.GoalTile.Length; i++)
        {
            GameObject goal = Instantiate(Data.GoalPrefab, Data.GoalIntroParent.transform.position, Quaternion.identity);
            goal.transform.SetParent(Data.GoalIntroParent.transform);

            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = Data.Requirements.GoalSprite[i];
            panel.thisString = "0/" + $"{Data.Requirements.GoalTileGoals}";

            GameObject gameGoal = Instantiate(Data.GoalPrefab, Data.GoalIntroParent.transform.position, Quaternion.identity);
            gameGoal.transform.SetParent(Data.GoalGameParent.transform);
        }
    }
    public void ProcessTurn(int pointsToGain, bool substractMoves)
    {
        Data.Points += pointsToGain;

        if (Data.Requirements.GameType == GameType.moves && substractMoves)
            Data.CurrentCounterValue--;
    }

    public bool CheckGameState() => Data.CurrentCounterValue == 0;

    public bool CheckIfZeroed(in int[] array)
    {
        for (int i = 0; i < array.Length; i++)
            if (array[i] > 0)
                return false;
        return true;
    }
}
