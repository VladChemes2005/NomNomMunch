using System.Collections;
using System.Collections.Generic;
using System;
//using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

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

}

public class GameManager : MonoBehaviour
{
    //public static GameManager Instance;
    public EndGameRequirements requirements;

    private float timerSeconds;
    public int points = 0;
    public int currentCounterValue;

    /*public GameObject backgroundPanel;
    public GameObject victoryPanel;
    public GameObject losePanel;*/
    public GameObject movesLabel;
    public GameObject timeLabel;
    public Text counter;
    public Text score;

    public bool IsGameEnded = false;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        currentCounterValue = requirements.counterValue;
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

        if (requirements.gameType == GameType.time && currentCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;
            currentCounterValue = (int)timerSeconds;
        }

        IsGameEnded = checkGameState();
        
        if (points >= requirements.goal && IsGameEnded)
        {
            IsGameEnded = true;
            /*backgroundPanel.SetActive(true);
            victoryPanel.SetActive(true);*/
            Debug.Log("WWWWIN");
            enabled = false;
        }
        else if (points < requirements.goal && IsGameEnded)
        {
            IsGameEnded = true;
            /*backgroundPanel.SetActive(true);
            losePanel.SetActive(true);*/
            Debug.Log("Loooose");
            enabled = false;
        }
            
        
    }
}
