using UnityEngine;

[CreateAssetMenu(menuName ="Upgrades/Basic Player Upgrade")]
public class BasicAdditionUpgrade : UpgradeSO // WHY UPGRADE A WEAPON VIA NUMBER AND NOT PERCENTAGE??????
    /// MAKE A SEPARATE UPGRADE FOR WEAPONS
{
    public PlayerStats playerStatsChange;
    public MeleeStats meleeStatsChange;
    public ThrowStats throwStatsChange;

    public override void ApplyEffect(PlayerStats activePlayerStats, WeaponsBuffs activeWeaponBuffs)
    {        
        if (increaseType == PlayerStatsIncreaseType.Addition) activePlayerStats += playerStatsChange;
        else if (increaseType == PlayerStatsIncreaseType.Percentage) activePlayerStats *= playerStatsChange;

        activeWeaponBuffs.meleeBuffs += meleeStatsChange;
        activeWeaponBuffs.throwBuffs += throwStatsChange;
    }

}
