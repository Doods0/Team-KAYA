using UnityEngine;

public class GameUtils : MonoBehaviour
{
    public static GameUtils instance;

    [Header("General")]
    public LayerMask enemyLayer;
    public LayerMask playerLayer;
    public AudioSource audioSource;

    [Header("Player")]
    public PlayerStatsHandler playerStats;
    public Transform playerTransform;
    public Vector3 playerPosition;

    [Header("Common Utils")]
    private MaterialPropertyBlock _flashingMaterial;
    public MaterialPropertyBlock flashingMaterial => _flashingMaterial;


    private void Awake()
    {
        instance = this;
        SetUpCommonUtils();
    }
    private void Update() => UpdatePlayerData();

    private void UpdatePlayerData()
    {
        if (playerTransform != null) playerPosition = playerTransform.position;
        else playerPosition = Vector3.zero; // if the player was deleted or killed
    }

    private void SetUpCommonUtils()
    {
        _flashingMaterial = new();
        _flashingMaterial.SetFloat("_FlashAmount", 1);
    }
}
