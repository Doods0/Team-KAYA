using UnityEngine;

// Heavy and light weapons inherit from WeaponSO
// WeaponSO holds the common funcs among all weapons, including slash considering both weapons can melee slash

public class GearHandler : MonoBehaviour
{
    [Header("Inventory")]
    public HeavyWeaponSO heavyWeapon;
    public LightWeaponSO lightWeapon;

    [Header("Components")]
    [SerializeField] private Collider2D hittingZone;
    [SerializeField] private EntityAnimator animator;

    private float currentCooldown;

    private void Update() => currentCooldown = Mathf.Max(0f, currentCooldown - Time.deltaTime);
    // Or fixedDeltaTime? should it change according to time speed?

    public void Attack(bool withHeavy, bool isThrowMode)
    {
        if (currentCooldown > 0) return;

        WeaponSO weaponInUse;
        WeaponSO otherWeapon;

        if (withHeavy)
        {
            weaponInUse = heavyWeapon;
            otherWeapon = lightWeapon;
        }
        else
        {
            weaponInUse = lightWeapon;
            otherWeapon = heavyWeapon;
        }

        // Adjust hittingArea here so attacks hit in the correct range

        float cooldown;

        if (withHeavy) cooldown = heavyWeapon.Slash();
        else
        {
            if (!isThrowMode) cooldown = lightWeapon.Slash();
            else cooldown = lightWeapon.Throw();
        }

        animator.triggerWeaponAnimation(weaponInUse, otherWeapon);
        currentCooldown = cooldown;
    }

}
