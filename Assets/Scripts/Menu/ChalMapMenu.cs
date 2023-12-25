using UnityEngine;
using UnityEngine.SceneManagement;

public class ChalMapMenu : MonoBehaviour
{
    public void BackGo() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
    public void StartScene(string sceneName) => SceneManager.LoadScene(sceneName);
}
