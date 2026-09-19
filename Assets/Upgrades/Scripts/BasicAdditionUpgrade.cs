using UnityEngine;

public enum PlayerStatsIncreaseType { Addition, Percentage }

[CreateAssetMenu(menuName ="Upgrades/Basic Addition Upgrade")]
public class BasicAdditionUpgrade : UpgradeSO // WHY UPGRADE A WEAPON VIA NUMBER AND NOT PERCENTAGE??????
{
    public PlayerStatsIncreaseType increaseType;
    public PlayerStats playerStatsChange;
    public MeleeStats heavyMeleeStatsChange;
    public MeleeStats lightMeleeStatsChange;
    public ThrowStats lightThrowStatsChange;

    public override void ApplyUpgrade
    (ref PlayerStats activePlayerStats,
    ref WeaponsBuffs activeWeaponBuffs,
    ref LocalWeaponsData weaponData)
    {        
        if (increaseType == PlayerStatsIncreaseType.Addition) activePlayerStats += playerStatsChange;
        else if (increaseType == PlayerStatsIncreaseType.Percentage) activePlayerStats *= playerStatsChange;

        activeWeaponBuffs.heavyMeleeBuffs += heavyMeleeStatsChange;
        activeWeaponBuffs.lightMeleeBuffs += lightMeleeStatsChange;
        activeWeaponBuffs.lightThrowBuffs += lightThrowStatsChange;
    }

}
