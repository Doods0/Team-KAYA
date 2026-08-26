using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerInput input;
    [SerializeField] private Camera camera;
    [SerializeField] private Transform weaponPivot;

    [Header("Action references")]
    [SerializeField] private InputActionReference xMovementRef;
    [SerializeField] private InputActionReference yMovementRef;

    [Header("Settings")]
    public float walkspeed;

    private InputAction xMovementAction;
    private InputAction yMovementAction;

    private int xMovementDir;
    private int yMovementDir;

    private void Awake()
    {
        xMovementAction = input.actions.FindAction(xMovementRef.action.id);
        yMovementAction = input.actions.FindAction(yMovementRef.action.id);
    }

    private void Update()
    {
        xMovementDir = (int)math.sign(xMovementAction.ReadValue<float>());
        yMovementDir = (int)math.sign(yMovementAction.ReadValue<float>());

        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = camera.ScreenToWorldPoint(mouseScreenPos);

        Vector2 aimDirection = (mouseWorldPos - weaponPivot.position).normalized;
        float targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

        weaponPivot.rotation = targetRotation;

        if (math.sign(aimDirection.x) < 0) weaponPivot.transform.localScale = new Vector3 (1, -1, 1);
        else weaponPivot.transform.localScale = new Vector3(1, 1, 1);
    }

    private void FixedUpdate()
    {
        rb.linearVelocityX = xMovementDir * walkspeed;
        rb.linearVelocityY = yMovementDir * walkspeed;
    }
}
