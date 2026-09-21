using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PurchasableElement
{
    public Sprite texture;
    public ScriptableObject element;
    public int price;
    public string title;
    public string desc;
}

public class ShopHandler : MonoBehaviour
{
    // Meant to randomize given purchasable items and pass them to UI elements that require them
    // Also handles evaluation of purchasing attempts
    // And handles assigning purchased items to the player

    public List<PurchasableElement> purchasableWeapons;
    public List<PurchasableElement> purchasableUpgrades;

    [HideInInspector] public List<PurchasableElement> availableWeapons = new();
    [HideInInspector] public List<PurchasableElement> availableUpgrades = new();

    public float pricingMultiplierPerPurchase;

    private void Awake() => UpdateStock();

    public void UpdateStock() // Called after shopping (after resuming the game)
    {
        availableWeapons.Clear();
        availableUpgrades.Clear();
        for (int i = 0; i < 5; i++)
        {
            PurchasableElement chosenElement = new();
            for (int j = 0; j < 100; j++)
            {
                int randomIndex = UnityEngine.Random.Range(0, purchasableUpgrades.Count);
                chosenElement = purchasableUpgrades[randomIndex];
                if (!availableUpgrades.Contains(chosenElement)) break;
            }
            availableUpgrades.Add(chosenElement);
        }

        for (int i = 0; i < 2; i++)
        {
            PurchasableElement chosenElement = new();
            for (int j = 0; j < 100; j++)
            {
                int randomIndex = UnityEngine.Random.Range(0, purchasableWeapons.Count);
                chosenElement = purchasableWeapons[randomIndex];
                if (!availableWeapons.Contains(chosenElement)) break;
            }
            availableWeapons.Add(chosenElement);
        }
    }

    public bool EvaluatePurchase(PurchasableElement product)
    {
        PlayerStatsHandler statsHandler = GameUtils.instance.playerStats;

        if (product.price == 0) return false;
        if (product.price > statsHandler.points) return false;
        statsHandler.points -= product.price;

        if (product.element is UpgradeSO upgrade)
        {
            upgrade.ApplyUpgrade(ref statsHandler.stats,ref statsHandler.weaponBuffs,ref statsHandler.localWeaponsData);
            statsHandler.upgrades.Add(upgrade);
        }
        else if (product.element is WeaponSO weapon)
        {
            if (weapon is HeavyWeaponSO heavyWeapon) statsHandler.heavyWeapon = heavyWeapon;
            else if (weapon is LightWeaponSO lightWeapon) statsHandler.lightWeapon = lightWeapon;
            statsHandler.animator.SwapWeapons(statsHandler.heavyWeapon, statsHandler.lightWeapon);
        }

        for (int i = 0; i < purchasableUpgrades.Count; i++)
        {
            PurchasableElement element = purchasableUpgrades[i];
            element.price = (int)(element.price * pricingMultiplierPerPurchase);
            purchasableUpgrades[i] = element;
        }
        for (int i = 0; i < purchasableWeapons.Count; i++)
        {
            PurchasableElement element = purchasableWeapons[i];
            element.price = (int)(element.price * pricingMultiplierPerPurchase);
            purchasableWeapons[i] = element;
        }
        return true;
    }

    public List<PurchasableElement> GetStock(bool isUpgrade) // called by both HUD and shop UI
    {
        if (isUpgrade) return availableUpgrades;
        else return availableWeapons;
    }

}
