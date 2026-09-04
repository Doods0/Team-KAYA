using UnityEngine;
using UnityEngine.InputSystem;

public class GameUtils : MonoBehaviour
{
    public static GameUtils instance;

    [Header("General")]
    public Camera camera;
    public LayerMask enemyLayer;
    public AudioSource audioSource;

    [Header("Player")]
    public Transform playerTransform;
    public Vector3 playerPosition;
    public Vector2 cursorWorldLocation;


    private void Awake() => instance = this;

    private void Update() => UpdatePlayerData();

    private void UpdatePlayerData()
    {
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = camera.ScreenToWorldPoint(mouseScreenPos);

        cursorWorldLocation = (mouseWorldPos - playerTransform.position).normalized;
        playerPosition = playerTransform.position;
    }
}
