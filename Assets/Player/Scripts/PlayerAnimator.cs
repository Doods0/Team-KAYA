using Unity.Mathematics;
using UnityEngine;

public class PlayerAnimator : EntityAnimator
{
    [SerializeField] private SpatialTracker spatialTracker;
    [SerializeField] private Transform weaponPivot;

    public override void Update()
    {
        // Weapon mouse follow section

        Vector2 aimDirection = spatialTracker.cursorDirection;
        float targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

        weaponPivot.rotation = targetRotation;

        if (math.sign(aimDirection.x) < 0) { weaponPivot.transform.localScale = new Vector3(1, -1, 1); }
        else { weaponPivot.transform.localScale = new Vector3(1, 1, 1); }

        base.Update(); // Regular EntityAnimator behavior
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
