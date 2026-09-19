using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/EffectOnComboUpgrade")]
public class EffectOnComboUpgrade : UpgradeSO
{
    [Header("Effect Data")]
    [SerializeField] private Effect effect;
    [SerializeField] private int combo; // example : on 5th shot, apply effect
    [SerializeField] private int intensity;
    [SerializeField] private int damage;
    [Header("Works On")]
    [SerializeField] private bool heavyMelee;
    [SerializeField] private bool lightMelee;
    [SerializeField] private bool lightThrow;

    public override void ApplyUpgrade
        (ref PlayerStats activePlayerStats,
        ref WeaponsBuffs activeWeaponBuffs,
        ref LocalWeaponsData weaponData)
    {
        if ((heavyMelee && weaponData.heavyMeleeSlashes % combo == 0 && weaponData.heavyMeleeSlashes != 0)
            || (lightMelee && weaponData.lightMeleeSlashes % combo == 0 && weaponData.lightMeleeSlashes != 0)
            || (lightThrow && weaponData.lightThrows % combo == 0 && weaponData.lightThrows != 0))
        effect.ApplyEffect(intensity, damage, weaponData);
    }
}

public class Effect : ScriptableObject
{
    public virtual void ApplyEffect(int intensity, int damage, LocalWeaponsData weaponData) { }
}