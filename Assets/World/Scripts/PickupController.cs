using UnityEngine;

public enum PickupType
{
    SpeedUp,
    SlowDown,
    Point,
    Shop,
}

public class PickupController : MonoBehaviour
{
    public string poolId;
    public PickupType type;

    [HideInInspector]
    public Vector3 pickupStartPos; // needs to be assigned by the spawner
    private float pickupElapsed;
    private const float pickupDuration = 0.5f;

    private void OnEnable()
    {
        pickupElapsed = 0f;
    }

    private void Update()
    {
        float mag = (GameUtils.instance.playerPosition - transform.position).magnitude;

        if (mag > GameUtils.instance.playerStats.pickupRange)
            return;

        pickupElapsed += Time.deltaTime;
        float t = Mathf.Clamp01(pickupElapsed / pickupDuration);
        float easedT = t * t;

        transform.position = Vector3.Lerp(
            pickupStartPos,
            GameUtils.instance.playerPosition,
            easedT
        );

        if (mag > 0.5f)
            return;

        GameManager.instance.AddInPool(poolId, gameObject);

        PlayerStats stats = GameUtils.instance.playerStats;

        if (type is PickupType.Point)
            stats.points++;
        else if (type is PickupType.SpeedUp)
        {
            GameManager.instance.runtimeScale += stats.speedupDropInterval;
        }
        else if (type is PickupType.SlowDown)
        {
            GameManager.instance.runtimeScale -= stats.slowdownDropInterval;
        }
        else if (type is PickupType.Shop)
            stats.hasShopAccess = true;

        gameObject.SetActive(false);
    }
}
