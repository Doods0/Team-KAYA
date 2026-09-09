using UnityEngine;

public class GameUtils : MonoBehaviour
{
    public static GameUtils instance;

    [Header("General")]
    public LayerMask enemyLayer;
    public AudioSource audioSource;

    [Header("Player")]
    public PlayerStats playerStats;
    public Transform playerTransform;
    public Vector3 playerPosition;


    private void Awake() => instance = this;
    private void Update() => UpdatePlayerData();

    private void UpdatePlayerData()
    {
        playerPosition = playerTransform.position;
    }
}
