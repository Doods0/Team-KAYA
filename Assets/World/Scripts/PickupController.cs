using UnityEngine;

public enum PickupType { SpeedUp, SlowDown, Point, Shop }

public class PickupController : MonoBehaviour
{
    public string poolId;
    public PickupType type;

    [HideInInspector] public Vector3 pickupStartPos; // needs to be assigned by the spawner
    private float pickupElapsed;
    private const float pickupDuration = 0.5f;
    private TrailRenderer trail;

    private void OnEnable()
    {
        pickupElapsed = 0f;
    }

    private void Awake() => trail = GetComponent<TrailRenderer>();

    private void Update()
    {
        trail.emitting = true;
        float mag = (GameUtils.instance.playerPosition - transform.position).magnitude;

        if (mag > GameUtils.instance.playerStats.stats.pickupRange) return;

        pickupElapsed += Time.deltaTime;
        float t = Mathf.Clamp01(pickupElapsed / pickupDuration);
        float easedT = t * t;

        transform.position = Vector3.Lerp(pickupStartPos, GameUtils.instance.playerPosition, easedT);

        if (mag > 0.5f) return;

        GameManager.instance.AddInPool(poolId, gameObject);

        GameUtils.instance.playerStats.CollectPickup(type);

        gameObject.SetActive(false);
        trail.emitting = false;
    }
}
