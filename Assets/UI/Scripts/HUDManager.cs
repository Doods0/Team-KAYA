using DG.Tweening;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("Loading")]
    [SerializeField] private Image loadingTransition;
    [SerializeField] private RectTransform timeOmeterMove;
    [SerializeField] private RectTransform timerMove;
    [SerializeField] private RectTransform healthBarMove;
    [Header("Menus")]
    [SerializeField] private GameObject lossMenu;
    [Header("Data")]
    [SerializeField] private TextMeshProUGUI timePassedText;
    [Header("Health")]
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject healthFull;
    [SerializeField] private GameObject healthEmpty;
    [Header("TimeOMeter")]
    [SerializeField] private RectTransform meterPin;
    [SerializeField] private RectTransform tickerPin;

    public void UpdateUI(float timeScale, float timePassed)
    {
        int totalSeconds = (int)timePassed;

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timePassedText.text = $"{minutes:D2}:{seconds:D2}";

        UpdateTimeOMeter(timeScale, GameManager.instance.maxTimeScale);
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

    public void UpdateTimeOMeter(float timeScale, float maxTimeScale)
    {
        float clampedValue = Mathf.Clamp(timeScale, 0f, maxTimeScale);
        float t = clampedValue / maxTimeScale;
        float angle = Mathf.Lerp(90f, 0f, t);

        meterPin.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    public IEnumerator HandleTickerMovement()
    {
        while (true)
        {
            float timeScale = GameManager.instance.timeScale;
            if (timeScale != 0)
            {
                Vector3 targetRotation = new(0, 0, tickerPin.rotation.eulerAngles.z - 20f);
                tickerPin.DOLocalRotate(targetRotation, 0.1f).SetEase(Ease.OutBack,3.5f);
                yield return new WaitForSeconds(1 / GameManager.instance.timeScale);
            }
            else yield return null;
        }
    }

    public async Task PlayIntro()
    {
        loadingTransition.color = new(1, 1, 1, 1);
        timeOmeterMove.DOLocalMoveY(-950, 0);
        timerMove.DOLocalMoveY(720, 0);
        healthBarMove.DOLocalMoveY(650, 0);

        Sequence introSequence = DOTween.Sequence();

        introSequence.Append(loadingTransition.DOFade(0, 1f));
        introSequence.AppendInterval(1f);
        introSequence.Append(timeOmeterMove.DOLocalMoveY(-540, 1).SetEase(Ease.OutCubic));
        introSequence.Join(timerMove.DOLocalMoveY(497f, 1).SetEase(Ease.OutCubic));
        introSequence.Join(healthBarMove.DOLocalMoveY(494, 1).SetEase(Ease.OutCubic));

        await introSequence.AsyncWaitForCompletion();

        StartCoroutine(HandleTickerMovement());
    }

    public void PlayLossAnimations() => lossMenu.SetActive(true);

    [ContextMenu("Load Menu")]
    public void MainMenu() => SceneManager.LoadScene("Menu");

    [ContextMenu("Retry")]
    public void Retry() => SceneManager.LoadScene("TTI");
}
