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

    private Rigidbody2D rb;
    private PlayerInput input;
    private PlayerStats gearHandler;
    #region Actions
    private InputAction xMovementAction;
    private InputAction yMovementAction;
    private InputAction heavyAttackAction;
    private InputAction lightAttackAction;
    private InputAction toggleThrowAction;
    #endregion

    [Header("Sounds (yes, I know they shouldn't be in this script. Sorry")]
    public AudioClip walk1;
    public AudioClip walk2;
    public AudioClip walk3;
    public float timeBetweenFootsteps;
    float footstepTimer;

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
        if (xMovementDir != 0 || yMovementDir != 0) Animator.state = EntityState.Walking;
        else Animator.state = EntityState.Idle;

        Animator.flipCharacter(xMovementDir);
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


        rb.linearVelocityX = xMovementDir * gearHandler.walkspeed;
        rb.linearVelocityY = yMovementDir * gearHandler.walkspeed;
    }

    AudioClip RandomFootstepClip()
    {
        int random = UnityEngine.Random.Range(0, 3);

        if (random == 0) return walk1;
        if (random == 1) return walk2;
        else return walk3;
    }
}
