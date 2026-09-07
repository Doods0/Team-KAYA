using UnityEngine;

// To create a special light weapon, please inherit from this SO

[CreateAssetMenu(menuName = "Weapons/Basic Light Weapon")]
public class LightWeaponSO : WeaponSO
{
    [Header("Throw")]
    [Header("Stats")]
    public float throwDamage;
    public float throwCooldown;
    [Header("Animations and Sounds")]
    public AnimationClip[] throwAnimations; // need to exist already in the AnimationController
    public AudioClip[] throwSounds;

    public virtual float Throw()
    {
        return throwCooldown;
    }
}
