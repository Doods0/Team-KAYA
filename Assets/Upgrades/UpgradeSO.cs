using UnityEngine;

public class UpgradeSO : ScriptableObject
{
    public enum IncreaseType { Addition, Percentage }
    public enum Frequency { Once, Update }
    // Update would require a reference of the functional PlayerStats though
    // And a custom function too
    // Example : SpeedCorrespondToKnockback

    // Upgrade name
    // Upgrade texture
    // Upgrade increase Type
    // Upgrade frequency

    // The other stats are determined by other SOs inheriting as this script won't be usable on its own

    public virtual void ApplyEffect(PlayerStats activeStats = null) { }
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