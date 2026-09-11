using UnityEngine;

public class GameUtils : MonoBehaviour
{
    public static GameUtils instance;

    [Header("General")]
    public LayerMask enemyLayer;
    public LayerMask playerLayer;
    public AudioSource audioSource;

    [Header("Player")]
    public PlayerStats playerStats;
    public Transform playerTransform;
    public Vector3 playerPosition;


    private void Awake() => instance = this;
    private void Update() => UpdatePlayerData();

    private void UpdatePlayerData()
    {
        if (playerTransform != null) playerPosition = playerTransform.position;
        else playerPosition = Vector3.zero; // if the player was deleted or killed
    }
}
