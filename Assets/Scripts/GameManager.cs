using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum GameType
{
    moves,
    time
}

[System.Serializable]
public class EndGameRequirements
{
    public GameType gameType;
    public int counterValue;
    public int goal;
    public VeggieType[] goalTile;
    public int[] goalTileGoals;
}

public class GameManager : MonoBehaviour
{
    public bool IsZeroed(int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] > 0) { return false; }
        }
        return true;
    }

    //public static GameManager Instance;
    public EndGameRequirements requirements;

    private float timerSeconds;
    public int points = 0;
    public int currentCounterValue;
    public VeggieType[] goalTile;
    public int[] goalTileGoals;


    /*public GameObject backgroundPanel;
    public GameObject victoryPanel;
    public GameObject losePanel;*/
    public GameObject movesLabel;
    public GameObject timeLabel;
    public Text counter;
    public Text score;
    public VeggieBoard board;
    public int boardFlipTileAmount;

    public bool IsGameEnded = false;

    void Start()
    {
        board = FindObjectOfType<VeggieBoard>();
        Initialize();
    }

    public void Initialize()
    {
        currentCounterValue = requirements.counterValue;
        goalTileGoals = requirements.goalTileGoals;
        goalTileGoals = (int[])goalTileGoals.Clone();
        goalTile = requirements.goalTile;
        goalTile = (VeggieType[])goalTile.Clone();

        if (goalTileGoals.Length > goalTile.Length)
        {
            for (int i = goalTile.Length; i < goalTileGoals.Length; i++)
            {
                goalTileGoals[i] = 0;
            }
        }

        if (requirements.gameType == GameType.moves)
        {
            movesLabel.SetActive(true);
            timeLabel.SetActive(false);
        }
        else
        {
            timerSeconds = currentCounterValue;
            movesLabel.SetActive(false);
            timeLabel.SetActive(true);
        }

        if (board.IsFlipMap)
        {
            boardFlipTileAmount = board.flipTileAmount;
        }
        counter.text = $"{currentCounterValue}";
        score.text = "0";
    }

    public void ProcessTurn(int pointsToGain, bool substractMoves)
    {
        points += pointsToGain;

        if (requirements.gameType == GameType.moves && substractMoves)
        {
            currentCounterValue -= 1;
        }
    }

    public bool checkGameState()
    {
        if (currentCounterValue == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /*public void WinGame()
    {
        //SceneManager.LoadScene(0);
        Debug.Log("WWWWIN");
    }

    public void LoseGame()
    {
        //SceneManager.LoadScene(0);
        Debug.Log("Loooose");
    }*/

    void Update()
    {
        counter.text = $"{currentCounterValue}";
        score.text = $"{points}";

        if (board.IsFlipMap)
        {
            boardFlipTileAmount = board.flipTileAmount;
        }

        if (requirements.gameType == GameType.time && currentCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;
            currentCounterValue = (int)timerSeconds;
        }

        IsGameEnded = checkGameState();

        if (points >= requirements.goal && IsGameEnded && IsZeroed(goalTileGoals) && boardFlipTileAmount == 0)
        {
            //IsGameEnded = true;
            /*backgroundPanel.SetActive(true);
            victoryPanel.SetActive(true);*/
            Debug.Log("WWWWIN");
            enabled = false;
        }
        else if (points < requirements.goal && IsGameEnded && (!IsZeroed(goalTileGoals) || goalTileGoals.Length == 0 || boardFlipTileAmount == 0))
        {
            //IsGameEnded = true;
            /*backgroundPanel.SetActive(true);
            losePanel.SetActive(true);*/
            Debug.Log("Loooose");
            enabled = false;
        }


    }
}

