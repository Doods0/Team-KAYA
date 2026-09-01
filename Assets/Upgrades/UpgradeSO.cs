using UnityEngine;

public enum UpgradeType { Once, Passive }

public class UpgradeSO : ScriptableObject
{
    // Meant to be inherited by each speficic type of an upgrade whether world, weapon or player to reduce noise in the inspector
}


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