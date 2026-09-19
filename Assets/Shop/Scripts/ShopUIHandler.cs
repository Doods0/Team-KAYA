using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopUIHandler : MonoBehaviour
{
    // Meant to pass purchase attempts and color out non-purchasable items in shop

    // Data is taken from ShopHandler after every shop encounter so the same data is passed to
    // other UI elements to display the next purchasable element on player's HUD

    [Header("Components")]
    public ShopHandler shopUtil;
    [SerializeField] private HUDManager hudUtil;
    [SerializeField] private ShopElementDriver[] upgrades = new ShopElementDriver[5];
    [SerializeField] private ShopElementDriver[] weapons = new ShopElementDriver[2];

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI description;

    [Header("Sounds")]
    [SerializeField] private AudioClip onSuccessfulPurchase;

    private PurchasableElement selectedElement;

    public void UpdateStockVisual()
    {
        itemName.text = "";
        description.text = "";

        int upgradeCount = Mathf.Min(upgrades.Length, shopUtil.availableUpgrades.Count);
        for (int i = 0; i < upgradeCount; i++)
        {
            if (upgrades[i] == null) return;
            upgrades[i].assignedElement = shopUtil.availableUpgrades[i];
            upgrades[i].UpdateLook();
        }

        int weaponCount = Mathf.Min(weapons.Length, shopUtil.availableWeapons.Count);
        for (int j = 0; j < weaponCount; j++)
        {
            if (weapons[j] == null) return;
            weapons[j].assignedElement = shopUtil.availableWeapons[j];
            weapons[j].UpdateLook();
        }
    }

    public void OnSlotClick()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        if (clickedButton == null) return;
        ShopElementDriver elementDriver = clickedButton.GetComponent<ShopElementDriver>();
        if (elementDriver == null) return;

        selectedElement = elementDriver.assignedElement;

        itemName.text = selectedElement.title;
        description.text = selectedElement.desc;
    }

    public void OnPurchase()
    {
        bool purchaseSuccess = shopUtil.EvaluatePurchase(selectedElement);
        if (!purchaseSuccess) return;
        selectedElement = new();
        GameUtils.instance.audioSource.PlayOneShot(onSuccessfulPurchase);
        StartCoroutine(hudUtil.HideShopMenu());
    }

    public void OnExit() => StartCoroutine(hudUtil.HideShopMenu());
}
