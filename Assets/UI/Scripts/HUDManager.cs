using DG.Tweening;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("Loading")]
    [SerializeField] private Image loadingTransition;
    [SerializeField] private RectTransform timeOmeterMove;
    [SerializeField] private RectTransform timerMove;
    [SerializeField] private RectTransform healthBarMove;
    [SerializeField] private RectTransform pointsCounterMove;
    [Header("Menus")]
    [SerializeField] private GameObject lossMenu;
    [SerializeField] private RectTransform shopMenu;
    [SerializeField] private ShopUIHandler shopUIHandler;
    [Header("Data")]
    [SerializeField] private TextMeshProUGUI timePassedText;
    [SerializeField] private TextMeshProUGUI pointsText;
    [Header("Health")]
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject healthFull;
    [SerializeField] private GameObject healthEmpty;
    [Header("TimeOMeter")]
    [SerializeField] private RectTransform meterPin;
    [SerializeField] private RectTransform tickerPin;
    [SerializeField] private RectTransform shopTriggerX;

    public void UpdateUI(float timeScale, float timePassed)
    {
        int totalSeconds = (int)timePassed;

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timePassedText.text = $"{minutes:D2}:{seconds:D2}";
        pointsText.text = GameUtils.instance.playerStats.points.ToString();

        UpdateTimeOMeter(timeScale, GameManager.instance.minTimeScale, GameManager.instance.maxTimeScale);
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
    public void UpdateTimeOMeter(float timeScale, float minTimeScale, float maxTimeScale)
    {
        float clampedValue = Mathf.Clamp(timeScale, minTimeScale, maxTimeScale);
        float t = clampedValue / maxTimeScale;
        float angle = Mathf.Lerp(90f, 0f, t);

        meterPin.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void AssignShopTriggerPoint(float interval)
    {
        if (interval == Mathf.Infinity)
        {
            shopTriggerX.localPosition = new(-1000, -1000);
            return;
        }

        // using ticker pin position as a reference to the center point
        Vector3 frozenForward = tickerPin.right;
        Vector3 origin = tickerPin.localPosition;

        GameManager manager = GameManager.instance;

        float clampedValue = Mathf.Clamp(interval, manager.minTimeScale, manager.maxTimeScale);
        float t = clampedValue / manager.maxTimeScale;
        float angle = Mathf.Lerp(90f, 0f, t);

        Vector3 dir = Quaternion.Euler(0, 0, angle) * frozenForward;
        shopTriggerX.localPosition = origin + dir * 165;
    }

    public void CollectPoint()
    {
        pointsText.DOKill();
        pointsText.transform.DOScale(1.5f, 0).OnComplete(() => 
        {
            pointsText.transform.DOScale(1, 0.2f).SetEase(Ease.OutCubic);
        });
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
        pointsCounterMove.DOLocalMoveY(720,0);

        Sequence introSequence = DOTween.Sequence();

        introSequence.Append(loadingTransition.DOFade(0, 1f));
        introSequence.AppendInterval(1f);
        introSequence.Append(timeOmeterMove.DOLocalMoveY(-540, 1).SetEase(Ease.OutCubic));
        introSequence.Join(timerMove.DOLocalMoveY(497f, 1).SetEase(Ease.OutCubic));
        introSequence.Join(healthBarMove.DOLocalMoveY(494, 1).SetEase(Ease.OutCubic));
        introSequence.Join(pointsCounterMove.DOLocalMoveY(363.3f, 1).SetEase(Ease.OutCubic));

        await introSequence.AsyncWaitForCompletion();

        StartCoroutine(HandleTickerMovement());
    }

    public IEnumerator ShowShopMenu()
    {
        shopUIHandler.UpdateStockVisual();
        yield return new WaitForSeconds(1);
        shopMenu.DOLocalMoveY(0, 1).SetEase(Ease.OutCubic);
    }

    public IEnumerator HideShopMenu()
    {
        shopMenu.DOLocalMoveY(-1100, 1).SetEase(Ease.InCubic);
        yield return new WaitForSeconds(1);
        GameManager.instance.isGamePaused = false;
        GameManager.instance.isTimeBypassed = false;
    }

    public void PlayLossAnimations() => lossMenu.SetActive(true);

    [ContextMenu("Load Menu")]
    public void MainMenu() => SceneManager.LoadScene("Menu");

    [ContextMenu("Retry")]
    public void Retry() => SceneManager.LoadScene("MainGame");
}
