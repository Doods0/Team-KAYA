using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDManager : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject lossMenu;
    [SerializeField] TextMeshProUGUI timeScaleText;
    [Header("Health")]
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject healthFull;
    [SerializeField] private GameObject healthEmpty;

    public void ShowLossMenu() => lossMenu.SetActive(true);
    public void UpdateTimeScale(float timeScale)
    {
        timeScaleText.text = timeScale.ToString();
    }
    public void UpdateHealth(int health, int maxHealth)
    {
        foreach (Transform icon in healthBar.transform) Destroy(icon.gameObject);

        for (int i = 0; i < health; i++) Instantiate(healthFull, healthBar.transform);

        for (int i = 0; i < maxHealth - health; i++)
        {
            Instantiate(healthEmpty, healthBar.transform);
        }
    }

    [ContextMenu("Load Menu")]
    public void MainMenu() => SceneManager.LoadScene("Menu");

    [ContextMenu("Retry")]
    public void Retry() => SceneManager.LoadScene("MainGame");
}
