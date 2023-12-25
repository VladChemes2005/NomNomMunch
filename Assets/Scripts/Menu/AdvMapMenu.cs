using UnityEngine;
using UnityEngine.SceneManagement;

public class AdvMapMenu : MonoBehaviour
{
    public void BackGo() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    public void StartScene(string sceneName) => SceneManager.LoadScene(sceneName);
}
