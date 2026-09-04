using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [ContextMenu("Start")]
    public void startGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    [ContextMenu("Quit")]
    public void endGame()
    {
        Application.Quit();
    }

}
