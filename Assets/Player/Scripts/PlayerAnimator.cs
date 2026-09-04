using Unity.Mathematics;
using UnityEngine;

public class PlayerAnimator : EntityAnimator
{
    [SerializeField] private Transform weaponPivot;

    public override void Update()
    {
        // Weapon mouse follow section

        Vector2 aimDirection = GameUtils.instance.cursorWorldLocation;
        float targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

        weaponPivot.rotation = targetRotation;

        if (math.sign(aimDirection.x) < 0) weaponPivot.transform.localScale = new Vector3(1, -1, 1);
        else weaponPivot.transform.localScale = new Vector3(1, 1, 1);

        base.Update(); // Regular EntityAnimator behavior
    }

    // Melee slash animations are all the same, and are managed from here, later though

    public void AlignWeapons(WeaponSO weaponInUse, WeaponSO otherWeapon)
    {
        // Meant to align the weapon's grips to the character based on the values in the weapon's SO
    }

    public void SwapWeapons(WeaponSO weaponInUse, WeaponSO otherWeapon)
    {
        // Meant to swap between the current two weapons visually
        AlignWeapons(weaponInUse, otherWeapon);
    }

    public void TriggerWeaponAnimation(WeaponSO weaponInUse, WeaponSO otherWeapon)
    {

        SwapWeapons(weaponInUse, otherWeapon);
    }
}
