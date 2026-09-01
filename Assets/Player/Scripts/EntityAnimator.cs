using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class EntityAnimator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject character;
    [SerializeField] private Transform weaponPivot;
    [SerializeField] private Camera camera;

    [Header("Animations")]
    public EntityState state;
    [SerializeField] private AnimationRuleSO ruleSO;

    private Dictionary<EntityState,AnimationRule> rulesDictionary;

    private Animator animator;
    private int currentAnimation;

    private void Awake()
    {
        animator = character.GetComponent<Animator>();
        rulesDictionary = ruleSO.generateDictionary();
    }

    private void Update()
    {
        // Weapon mouse follow section
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = camera.ScreenToWorldPoint(mouseScreenPos);

        Vector2 aimDirection = (mouseWorldPos - weaponPivot.position).normalized;
        float targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

        weaponPivot.rotation = targetRotation;

        if (math.sign(aimDirection.x) < 0) weaponPivot.transform.localScale = new Vector3(1, -1, 1);
        else weaponPivot.transform.localScale = new Vector3(1, 1, 1);


        // Animations section
        var currentRule = rulesDictionary[state];
        if (currentRule == null) { return; }

        changeAnimation(currentRule.track_hash, currentRule.track, currentRule.fade);
    }

    public void changeAnimation(int animation_hash, AnimationClip track, float fade = 0f)
    {
        if (currentAnimation != animation_hash)
        {
            currentAnimation = animation_hash;
            animator.CrossFade(animation_hash, fade);
        }
    }

    public void flipCharacter(int direction)
    {
        if (direction == 0) { return; }
        transform.localScale = new Vector3(direction, transform.localScale.y, transform.localScale.z);
    }

    // Melee slash animations are all the same, and are managed from here, later though

    public void alignWeapons(WeaponSO weaponInUse, WeaponSO otherWeapon)
    {
        // Meant to align the weapon's grips to the character based on the values in the weapon's SO
    }

    public void swapWeapons(WeaponSO weaponInUse, WeaponSO otherWeapon)
    {
        // Meant to swap between the current two weapons visually
        alignWeapons(weaponInUse, otherWeapon);
    }

    public void triggerWeaponAnimation(WeaponSO weaponInUse, WeaponSO otherWeapon)
    {

        swapWeapons(weaponInUse, otherWeapon);
    }
}
