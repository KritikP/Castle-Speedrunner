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
    [SerializeField] private Button buyButton;
    [SerializeField] private ShopItem item;
    [SerializeField] private Player_Data playerData;

    void Start()
    {
        InitItem();
    }

    public void InitItem()
    {
        if (item != null)
        {
            nameText.text = item.itemName;
            descriptionText.text = item.description;
            itemImage.sprite = item.itemImage[item.level];

            if (item.level >= item.price.Length)
            {
                purchaseText.text = "Max";
                purchaseText.rectTransform.position = new Vector3(purchaseText.rectTransform.position.x, purchaseText.rectTransform.position.y - 10, purchaseText.rectTransform.position.z);
                priceText.text = "";
                buyButton.interactable = false;
            }
            else
            {
                priceText.text = item.price[item.level].ToString();
                purchaseText.text = "Buy";
            }
        }
        else
        {
            Debug.LogWarning("Empty item in shop, no item object inputted.");
        }
    }

    public void Buy()
    {
        if(item.level >= item.price.Length) //Already at highest tier upgrade
        {
            Debug.Log("Already purchased highest tier upgrade. (Button should not be interactable, so this message shouldn't be appearing.)");
        }
        else
        {
            if(playerData.coins >= item.price[item.level])
            {
                playerData.coins = playerData.coins - item.price[item.level];
                item.level++;
                InitItem();
            }
            else
            {
                Debug.Log("Not enough coins!");
            }
        }
    }
}
