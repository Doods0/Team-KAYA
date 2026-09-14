using UnityEngine;

public enum PlayerStatsIncreaseType { Addition, Percentage }
// weapons are always a percentage that stacks up with upgrades by addition
public enum Frequency { Once, Update } // Will this upgrade increase or decrease a stat once or link two stats together

public class UpgradeSO : ScriptableObject
{
    // Update would require a reference of the functional PlayerStats though
    // And a custom function too
    // Example : SpeedCorrespondToKnockback
    // ApplyEffect just takes the active playerStats and links two values with each other or something idk

    [Header("Basic Data")]
    public string title;
    public string description;
    public Sprite icon;
    public PlayerStatsIncreaseType increaseType;
    public Frequency frequency;

    // The other stats are determined by other SOs inheriting as this script won't be usable on its own
    public virtual void ApplyEffect(PlayerStats activePlayerStats, WeaponsBuffs activeWeaponBuffs) { }
    // Add to it the two other detached classes representing light and heavy weapons' stats

}

// OLD COMMENTS BELOW
// If weapon :

// Type
// Damage increase
// Range increase
// Chance of burn effect
// Burn damage (every 0.5)
// Chance of bleed effect
// Bleed damage (every 0.5)


// upgrades that modify a value once (whether the value itself is a rate or a constant)
// upgrades that update a value based on another
// upgrades that 