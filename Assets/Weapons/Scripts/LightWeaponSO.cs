using UnityEngine;

// To create a special light weapon, please inherit from this SO

[CreateAssetMenu(menuName = "Weapons/Basic Light Weapon")]
public class LightWeaponSO : WeaponSO
{
    [Header("Throw")]
    public float throwDamage;
    public float throwCooldown;

    public override float Slash()
    {
        return slashCooldown;
    }

    public virtual float Throw()
    {
        return throwCooldown;
    }
}
