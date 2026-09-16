using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopElementDriver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Image elementImage;

     public PurchasableElement assignedElement;
    
    public void UpdateLook()
    {
        elementImage.sprite = assignedElement.texture;
        string formatted = $"${assignedElement.price}";
        priceText.text = formatted;
        if (assignedElement.price > GameUtils.instance.playerStats.points) priceText.color = Color.red;
        else priceText.color = Color.black;
    }
}
