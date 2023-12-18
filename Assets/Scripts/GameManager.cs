using System.Collections;
using System.Collections.Generic;
using System;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
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
    public static GameManager Instance;
    public EndGameRequirements requirements;

    public int currentCounterValue;
    private float timerSeconds;
    public int points;
    public int currentGoal;

    public GameObject backgroundPanel;
    public GameObject victoryPanel;
    public GameObject losePanel;
    public GameObject movesLabel;
    public GameObject timeLabel;
    public Text counter;

    public bool IsGameEnded;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        currentCounterValue = requirements.counterValue;
        currentGoal = requirements.goal;
        if (requirements.gameType == GameType.moves)
        {
            movesLabel.SetActive(true);
            timeLabel.SetActive(false);
        }
        else
        {
            timerSeconds = 1;
            movesLabel.SetActive(false);
            timeLabel.SetActive(true);
        }
        counter.text = $"{currentCounterValue}";
    }

    public void ProcessTurn(int pointsToGain)
    {
        points += pointsToGain;

        if (requirements.gameType == GameType.moves)
        {
            currentCounterValue -= 1;
        }

        if (points >= currentGoal && currentCounterValue == 0)
        {
            IsGameEnded = true;
            backgroundPanel.SetActive(true);
            victoryPanel.SetActive(true);
            return;
        }
        else
        {
            IsGameEnded = false;
            backgroundPanel.SetActive(true);
            losePanel.SetActive(true);
            return;
        }
    }

    public void WinGame()
    {
        SceneManager.LoadScene(0);
    }

    public void LoseGame()
    {
        SceneManager.LoadScene(0);
    }

    void Update()
    {
        if (requirements.gameType == GameType.time && currentCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;

        }

        if (timerSeconds <= 0)
        {
            timerSeconds = 0;

            if (points >= currentGoal)
            {
                IsGameEnded = true;
                backgroundPanel.SetActive(true);
                victoryPanel.SetActive(true);
            }
            else
            {
                IsGameEnded = true;
                backgroundPanel.SetActive(true);
                losePanel.SetActive(true);
            }
            enabled = false;
        }
    }
}
