using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Image backgroundImage;

    [SerializeField]
    private RectTransform title;

    [SerializeField]
    private RectTransform buttons;

    [SerializeField]
    private RectTransform wholeFrame;

    private void Awake() => PlayIntroAnimations();

    private void PlayIntroAnimations()
    {
        backgroundImage.color = Color.black;
        buttons.localPosition = new(0, -550f, 0);
        title.localPosition = new(0, 700f, 0);

        Sequence menuSequence = DOTween.Sequence();

        menuSequence.AppendInterval(1f);
        menuSequence.Append(backgroundImage.DOColor(Color.white, 2f).SetEase(Ease.InOutSine));

        menuSequence.Append(title.DOLocalMove(new Vector3(0f, 332f, 0f), 1f).SetEase(Ease.OutBack));
        menuSequence.Append(buttons.DOLocalMove(Vector3.zero, 0.8f).SetEase(Ease.OutCubic));
    }

    private IEnumerator PlayStartAnimation()
    {
        wholeFrame.DOLocalMoveX(-2000f, 1f).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("MainGame");
    }

    [ContextMenu("Start")]
    public void startGame() => StartCoroutine(PlayStartAnimation());

    [ContextMenu("Quit")]
    public void endGame() => Application.Quit();
}
