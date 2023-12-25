using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject _quitPanel;

    public void PlayGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    public void OpenAreYouSure() => _quitPanel.SetActive(true);
    public void CloseAreYouSure() => _quitPanel.SetActive(false);
    public void QuitGame() => Application.Quit();
}