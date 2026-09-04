using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Utils")]
    [SerializeField] PlayerAnimator Animator;

    [Header("Action references")]
    #region Action References
    [SerializeField] private InputActionReference xMovementRef;
    [SerializeField] private InputActionReference yMovementRef;
    [SerializeField] private InputActionReference heavyAttackRef;
    [SerializeField] private InputActionReference lightAttackRef;
    [SerializeField] private InputActionReference toggleThrowRef;
    #endregion

    [Header("Settings")]
    public float walkspeed;
    public int health;
    public int maxHealth;

    private Rigidbody2D rb;
    private PlayerInput input;
    private GearHandler gearHandler;
    #region Actions
    private InputAction xMovementAction;
    private InputAction yMovementAction;
    private InputAction heavyAttackAction;
    private InputAction lightAttackAction;
    private InputAction toggleThrowAction;
    #endregion

    private int xMovementDir;
    private int yMovementDir;
    private bool isThrowMode = false;
    private bool stunned = false;
    private Vector3 knockbackVector;
    private bool invulnerable = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>();
        gearHandler = GetComponent<GearHandler>();
        #region Action Assigning
        xMovementAction = input.actions.FindAction(xMovementRef.action.id);
        yMovementAction = input.actions.FindAction(yMovementRef.action.id);
        heavyAttackAction = input.actions.FindAction(heavyAttackRef.action.id);
        lightAttackAction = input.actions.FindAction(lightAttackRef.action.id);
        toggleThrowAction = input.actions.FindAction(toggleThrowRef.action.id);
        #endregion
    }

    #region Action Passing
    private void OnEnable()
    {
        heavyAttackAction.performed += HeavyAttack;
        lightAttackAction.performed += LightAttack;
        toggleThrowAction.performed += ToggleThrow;
    }
    private void OnDisable()
    {
        heavyAttackAction.performed -= HeavyAttack;
        lightAttackAction.performed -= LightAttack;
        toggleThrowAction.performed -= ToggleThrow;
    }

    private void HeavyAttack(InputAction.CallbackContext _input) => gearHandler.Attack(true, isThrowMode);
    private void LightAttack(InputAction.CallbackContext _input) => gearHandler.Attack(false, isThrowMode);
    private void ToggleThrow(InputAction.CallbackContext _input) => isThrowMode = !isThrowMode;
    #endregion

    private void Update()
    {
        // Movement values update
        xMovementDir = (int)math.sign(xMovementAction.ReadValue<float>());
        yMovementDir = (int)math.sign(yMovementAction.ReadValue<float>());

        // Animations
        if (xMovementDir != 0 || yMovementDir != 0) { Animator.state = EntityState.Walking; }
        else { Animator.state = EntityState.Idle; }

        Animator.flipCharacter(xMovementDir);
    }

    private void FixedUpdate()
    {
        if (!stunned)
        {
            rb.linearVelocityX = xMovementDir * walkspeed;
            rb.linearVelocityY = yMovementDir * walkspeed;
        }
        else
        {
            rb.linearVelocityX = knockbackVector.x;
            rb.linearVelocityY = knockbackVector.y;
        }
    }

    public void TakeDamage(int damageTaken, Vector3 source)
    {
        TakeDamage(damageTaken, source, 3.0f);
    }
    public void TakeDamage(int damageTaken, Vector3 source, float knockback)
    {
        if (invulnerable) { return; }

        health -= damageTaken;

        stunned = true;
        invulnerable = true;
        Invoke(nameof(UnStun), 1.0f);
        Invoke(nameof(MakeVulnerable), 2.0f);

        knockbackVector = (transform.position - source).normalized * knockback;
    }

    void UnStun()
    {
        stunned = false;

        rb.linearVelocityX = 0;
        rb.linearVelocityY = 0;
    }
    void MakeVulnerable()
    {
        invulnerable = false;
    }
}
