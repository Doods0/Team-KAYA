using System;
using System.Collections.Generic;
using UnityEngine;

// Heavy and light weapons inherit from WeaponSO
// WeaponSO holds the common funcs among all weapons, including slash considering both weapons can melee slash
// This script is meant to hold the weapons and manage their damage and any active upgrades and buffs

// Things that are used by the weapons' SOs (because SOs can't store things)
// Meant to be stored here so independent weapon coding is possible
#region Pass To Weapon
[Serializable]
public class LocalWeaponsData
{
    public readonly List<Collider2D> hitBuffer = new List<Collider2D>();
    public ContactFilter2D contactFilter;

    public LocalWeaponsData()
    {
        contactFilter = new ContactFilter2D
        {
            layerMask = GameStatus.instance.enemyLayer,
            useLayerMask = true,
            useTriggers = true
        };
    }
}
#endregion

public class GearHandler : MonoBehaviour
{
    [Header("Inventory")]
    public HeavyWeaponSO heavyWeapon;
    public LightWeaponSO lightWeapon;

    [Header("Utils")]
    [SerializeField] private SpatialTracker localSpatialData;
    [SerializeField] private PlayerAnimator animator;
    // For now we keep getting the camera from the animator and the mouse here
    // We could write a third party script to track and return these values for both scripts independently, later though

    [HideInInspector] public LocalWeaponsData localWeaponsData;

    private float currentCooldown;
    private void Update() => currentCooldown = Mathf.Max(0f, currentCooldown - Time.deltaTime);
    // Or fixedDeltaTime? should it change according to time speed?

    private void Awake() => localWeaponsData = new LocalWeaponsData();

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

        float cooldown;

        if (withHeavy) cooldown = heavyWeapon.Slash(localWeaponsData, localSpatialData);
        else
        {
            if (!isThrowMode) cooldown = lightWeapon.Slash(localWeaponsData, localSpatialData);
            else cooldown = lightWeapon.Throw();
        }

        animator.triggerWeaponAnimation(weaponInUse, otherWeapon);
        currentCooldown = cooldown;
    }

}
