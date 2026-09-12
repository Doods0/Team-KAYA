using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private RectTransform crosshair;
    [SerializeField] private Image crosshairImage;
    [SerializeField] private Camera camera;
    [SerializeField] private float lookaheadAmount;
    [SerializeField] private float autoAimRange;
    [Header("Cursor Images")]
    [SerializeField] private Sprite meleeCrosshair;
    [SerializeField] private Sprite throwCrosshair;
    [HideInInspector] public bool isAutoAim = false;

    public static Vector3 cursorWorldPosition;
    public static Vector3 cursorDirectionVector;

    private Transform lockedAt;
    private readonly Collider2D[] enemyDetectorBuffer = new Collider2D[32];
    private ContactFilter2D enemyFilter;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        enemyFilter = new ContactFilter2D
        {
            layerMask = GameUtils.instance.enemyLayer,
            useLayerMask = true,
            useTriggers = false
        };

        StartCoroutine(UpdateAutoAim());
    }

    private void Update()
    {
        GameUtils utils = GameUtils.instance;
        Vector3 playerPos = utils.playerPosition;

        Vector2 mouseScreenPos = new();
        Vector2 mouseDirectionVector = new();
Vector2 mouseScreenPos;
Vector2 mouseDirectionVector;
Vector2 cameraLockOffset;

        if (utils.playerTransform != null)
        {
            if (!isAutoAim)
            {
                mouseScreenPos = Mouse.current.position.ReadValue();
                cursorWorldPosition = camera.ScreenToWorldPoint(mouseScreenPos);

                mouseDirectionVector = cursorWorldPosition - playerPos;

                cameraLockOffset = new(playerPos.x, playerPos.y + 0.5f);
                Vector2 lookaheadShift = new(mouseDirectionVector.x, mouseDirectionVector.y);
                cameraLockOffset += lookaheadShift * lookaheadAmount;

                crosshair.gameObject.SetActive(true);
            }
            else
            {
                if (lockedAt == null || !lockedAt.gameObject.activeInHierarchy)
                {
                    mouseScreenPos = new(1, 0);
                    crosshair.gameObject.SetActive(false);
                }
                else
                {
                    mouseScreenPos = camera.WorldToScreenPoint(lockedAt.position);

                    crosshair.gameObject.SetActive(true);
                }
                cursorWorldPosition = camera.ScreenToWorldPoint(mouseScreenPos);
                mouseDirectionVector = (cursorWorldPosition - playerPos);
                cameraLockOffset = playerPos + new Vector3(0, 0.5f, 0);
            }

            cursorDirectionVector = mouseDirectionVector.normalized;

            float cameraZ = camera.transform.position.z;
            camera.transform.position = new(cameraLockOffset.x, cameraLockOffset.y, cameraZ);
            crosshair.position = new(mouseScreenPos.x, mouseScreenPos.y);
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            crosshair.position = Vector3.zero;
        }
    }

    public IEnumerator UpdateAutoAim()
    {
        while (true)
        {
            if (isAutoAim) GetNearestEnemy();
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    public void GetNearestEnemy()
    {
        GameUtils utils = GameUtils.instance;
        Vector2 playerPos = utils.playerPosition;

        int count = Physics2D.OverlapCircle(playerPos, autoAimRange, enemyFilter, enemyDetectorBuffer);
        if (count == 0) return;

        Collider2D nearest = null;
        float nearestSqrDist = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Collider2D col = enemyDetectorBuffer[i];
            float sqrDist = ((Vector2)col.transform.position - playerPos).sqrMagnitude;
            if (sqrDist < nearestSqrDist)
            {
                nearestSqrDist = sqrDist;
                nearest = col;
            }
        }

        if (nearest == null) return;
        lockedAt = nearest.transform;
    }

    public void SwapCrosshair(bool isThrowMode)
    {
        if (isThrowMode) crosshairImage.sprite = throwCrosshair;
        else crosshairImage.sprite = meleeCrosshair;
    }
}
