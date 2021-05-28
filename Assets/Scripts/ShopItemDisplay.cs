using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopItemDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text purchaseText;
    [SerializeField] private Image itemImage;
    [SerializeField] private ShopItem item;

    void Start()
    {
        if (item != null)
        {
            nameText.text = item.itemName;
            descriptionText.text = item.description;
            priceText.text = item.price[item.level - 1].ToString();
            itemImage.sprite = item.itemImage[item.level - 1];

            if (item.level == item.price.Length)
            {
                purchaseText.text = "Purchased";
            }
            else
            {
                purchaseText.text = "Buy";
            }
        }
        else
        {
            Debug.LogWarning("Empty item in shop, no item object inputted.");
        }
    }
}
