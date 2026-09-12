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
    [SerializeField] private InputActionReference toggleAutoAimRef;
    #endregion

    private Rigidbody2D rb;
    private PlayerInput input;
    private PlayerStats gearHandler;
    #region Actions
    private InputAction xMovementAction;
    private InputAction yMovementAction;
    private InputAction heavyAttackAction;
    private InputAction lightAttackAction;
    private InputAction toggleThrowAction;
    private InputAction toggleAutoAimAction;
    #endregion

    [Header("Sounds")] // We'll put them here because they're movement afterall
    public AudioClip walk1;
    public AudioClip walk2;
    public AudioClip walk3;
    public AudioClip switchToMeleeSound;
    public AudioClip switchToThrowSound;
    public float timeBetweenFootsteps;
    float footstepTimer;

    [Header("Settings")]
    public float knockbackDecayRate = 4f;
    [HideInInspector] public Vector2 knockbackVelocity;

    [Header("Utils")]
    [SerializeField] private CameraController cameraUtil;

    private int xMovementDir;
    private int yMovementDir;
    private bool isThrowMode = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>();
        gearHandler = GetComponent<PlayerStats>();
        #region Action Assigning
        xMovementAction = input.actions.FindAction(xMovementRef.action.id);
        yMovementAction = input.actions.FindAction(yMovementRef.action.id);
        heavyAttackAction = input.actions.FindAction(heavyAttackRef.action.id);
        lightAttackAction = input.actions.FindAction(lightAttackRef.action.id);
        toggleThrowAction = input.actions.FindAction(toggleThrowRef.action.id);
        toggleAutoAimAction = input.actions.FindAction(toggleAutoAimRef.action.id);
        #endregion

    }

    #region Action Passing
    private void OnEnable()
    {
        toggleThrowAction.performed += ToggleThrow;
        toggleAutoAimAction.performed += ToggleAutoAim;
    }
    private void OnDisable()
    {
        toggleThrowAction.performed -= ToggleThrow;
        toggleAutoAimAction.performed -= ToggleAutoAim;
    }

    private void ToggleThrow(InputAction.CallbackContext _input) 
    {
        isThrowMode = !isThrowMode;
        cameraUtil.SwapCrosshair(isThrowMode);
        if (isThrowMode) GameUtils.instance.audioSource.PlayOneShot(switchToThrowSound);
        else GameUtils.instance.audioSource.PlayOneShot(switchToMeleeSound);
    } 
    private void ToggleAutoAim(InputAction.CallbackContext _input) => cameraUtil.isAutoAim = !cameraUtil.isAutoAim;
    #endregion

    private void Update()
    {
        if (heavyAttackAction.IsPressed()) gearHandler.Attack(true, isThrowMode);
        if (lightAttackAction.IsPressed()) gearHandler.Attack(false, isThrowMode);

        // Movement values update
        xMovementDir = (int)math.sign(xMovementAction.ReadValue<float>());
        yMovementDir = (int)math.sign(yMovementAction.ReadValue<float>());

        // Animations
        if (xMovementDir != 0 || yMovementDir != 0) Animator.state = EntityState.Walking;
        else Animator.state = EntityState.Idle;

        Animator.FlipCharacter(xMovementDir);
    }

    private void FixedUpdate()
    {
        footstepTimer += Time.deltaTime;
        // I can't use (rb.linearVelocity.magnitude != 0) because I need to make sure the reason
        // for movement is walking.
        if (footstepTimer > timeBetweenFootsteps && (xMovementDir != 0 || yMovementDir != 0))
        {
            GameUtils.instance.audioSource.PlayOneShot(RandomFootstepClip(), UnityEngine.Random.Range(.25f, 0.8f));
            footstepTimer = 0;
        }


        rb.linearVelocityX = xMovementDir * gearHandler.walkspeed * GameManager.instance.timeScale;
        rb.linearVelocityY = yMovementDir * gearHandler.walkspeed * GameManager.instance.timeScale;

        if (GameManager.instance.timeScale != 0)
        {
            knockbackVelocity *= Mathf.Exp(-knockbackDecayRate * Time.deltaTime);
        }
        rb.linearVelocity += knockbackVelocity * GameManager.instance.timeScale;
    }

    public void ApplyKnockback(Vector2 impulse) => knockbackVelocity += impulse;

    AudioClip RandomFootstepClip()
    {
        int random = UnityEngine.Random.Range(0, 3);

        if (random == 0) return walk1;
        if (random == 1) return walk2;
        else return walk3;
    }
}
