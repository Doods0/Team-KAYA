using UnityEngine;

public class WeaponSO : ScriptableObject
{
    [Header("Visuals")]
    public Sprite texture;
    public float inUseGripOffset;
    public float concealedGripOffset;

    [Header("Stats")]
    [Header("Slash")]
    public float slashDamage;
    public float slashCooldown;
    public float slashOffset; // Meant to configure the damage area location for the weapon range
    public float slashSize; // Same but size

    // Function must return a float (Cooldown value)
    public virtual float Slash() { return 0; }
}
