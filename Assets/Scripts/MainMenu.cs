using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject quitPanel;

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OpenAreYouSure()
    {
        quitPanel.SetActive(true);
    }

    public void CloseAreYouSure()
    {
        quitPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("QUUUUUUUUUUUUUUUUUUUUUUUIT!");
        Application.Quit();
    }
}